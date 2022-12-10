using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : EntityBehaviour
{
    public static Player instance;

    [System.NonSerialized] public Quaternion MovingAngle = Quaternion.identity;//поможет вычислить место война, учитывая поворот игрока

    public float DashSpeed;
    public float DashDistance;
    public float DashReloadTime;
    private bool isDash;

    public int index;

    private Dictionary<int, int> Stacs = new();
    private int allPetrificationStacs = 0;
    private int allClumsinessStacs = 0;

    private Transform nearestEnemyTransform;
    private bool isAttack = false;

    [SerializeField] private Image HealthImg;
    [SerializeField] private Scrollbar HealthScrol;
    [SerializeField] private Image SpeedImg;
    [SerializeField] private Scrollbar SpeedScrol;
    [SerializeField] private Image Cross;

    private delegate void FixedUpdateMethods();
    private FixedUpdateMethods State;
    private void Awake()
    {
        if (index < GameController.PlayersScript.Count)
            GameController.PlayersScript[index] = this;
        while (index >= GameController.PlayersScript.Count)
                GameController.PlayersScript.Add(this);
        
        State = IdleState;
        instance = this;
    }

    private void Start()
    {
        BaseStart();
        StartCoroutine(FindGoal());
    }
    private void Update()
    {
        BaseUpdate();
        if (Input.GetKeyDown(KeyCode.E) && !isDash && State == WasdState)
        {
            State = DashState;
            StartCoroutine(ToDash());
            anim.SetTrigger("IsDash");
            isDash = true;
        }

        if (myTransform.InverseTransformDirection(movementVector).x < -0.1f)
            transform.localScale = new Vector3(-startScale, transform.localScale.y, transform.localScale.z);
        else if (myTransform.InverseTransformDirection(movementVector).x > 0.1f)
            transform.localScale = new Vector3(startScale, transform.localScale.y, transform.localScale.z);

        HealthScrol.value = HealthImg.fillAmount = Mathf.Lerp(HealthScrol.value, (float)hpCurrent / hpDefault, Time.deltaTime);
        SpeedScrol.value = SpeedImg.fillAmount = Mathf.Lerp(SpeedScrol.value, speedCurrent / speedDefault, Time.deltaTime);

    }
    private void FixedUpdate()
    {
        if (isAttack)
            weapon.Attack();
        
        State();
        BaseFixedUpdate();
    }

    private void WasdState()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            movementVector = myTransform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
            MovingAngle = Quaternion.Euler(0, Mathf.Atan2(movementVector.x, movementVector.z) * Mathf.Rad2Deg, 0);
            anim.SetBool("IsRunning", true);
        }
        else
        {
            movementVector = Vector3.zero;
            anim.SetBool("IsRunning", false);
        }
    }
    private void DashState() => movementVector = MovingAngle * Vector3.forward * DashSpeed;
    private void IdleState()
    {
        if (isAttack)
        {
            movementVector = (nearestEnemyTransform.position - myTransform.position).normalized;
            MovingAngle = Quaternion.Euler(0, Mathf.Atan2(movementVector.x, movementVector.z) * Mathf.Rad2Deg, 0);
            anim.SetBool("IsRunning", true);
        }
        else
        {
            movementVector = Vector3.zero;
            anim.SetBool("IsRunning", false);
        }
    }
    private IEnumerator FindGoal()
    {
        float dist, minDistToEnemy;
        while (true)
        {
            minDistToEnemy = float.MaxValue;
            yield return new WaitForSeconds(courotineTime);

            if (GameController.EnemiesScript.Count == 0) isAttack = false;
            foreach (var enemy in GameController.EnemiesScript)
            {
                dist = Vector3.Distance(myTransform.position, enemy.transform.position);
                if (dist < minDistToEnemy)
                {
                    minDistToEnemy = dist;
                    nearestEnemyTransform = enemy.transform;
                    weapon.AttackTransform = nearestEnemyTransform;
                    isAttack = Vector3.Distance(nearestEnemyTransform.position, myTransform.position) < weapon.radiusAttack;
                }
            }
        }
    }

    public void ActivatePlayer()
    {
        State = WasdState;
        CameraMoving.target = myTransform;
        GameController.instance.VignetteIntensity =
            Mathf.Lerp(0, 0.75f, (allPetrificationStacs + allClumsinessStacs) / (float)(Shaman.S_stacs[1] + Shaman.S_stacs[0]));
    }
    public void DeactivatePlayer()
    {
        State = IdleState;
    }
    IEnumerator ToDash()
    {
        //начали dash
        yield return new WaitForSeconds(DashDistance / DashSpeed);
        State = WasdState;
        yield return new WaitForSeconds(DashReloadTime);
        isDash = false;
    }


    public int SetCurse(int shamanIdx)
    {
        if (!Stacs.ContainsKey(shamanIdx)) Stacs.Add(shamanIdx, 0);
        Stacs[shamanIdx] += 1;


        if (State == WasdState && Stacs[shamanIdx] < Shaman.S_stacs[(int)GameController.ShamansScript[shamanIdx].curseType]*2)
            GameController.instance.VignetteIntensity =
                Mathf.Lerp(0, 0.75f, (allPetrificationStacs + allClumsinessStacs) / (float)(Shaman.S_stacs[1] + Shaman.S_stacs[0]));

        switch (GameController.ShamansScript[shamanIdx].curseType)
        {
            case CurseType.Petrification:
                allPetrificationStacs += 1;
                speedTarget = (int)(Mathf.Clamp(1f - allPetrificationStacs / (Shaman.S_stacs[(int)CurseType.Petrification] * 3f), 0, 1) * speedDefault);
                return Stacs[shamanIdx];

            case CurseType.Clumsiness:
                allClumsinessStacs += 1;
                weapon.damage = (int)(Mathf.Clamp(1f - allClumsinessStacs / (Shaman.S_stacs[(int)CurseType.Clumsiness] * 3f), 0, 1) * speedDefault);
                return Stacs[shamanIdx];
        }
        return 0;
    }

    public void RemoveCurse(int shamanIdx)
    {
        if (Stacs.ContainsKey(shamanIdx))
        {
            switch (GameController.ShamansScript[shamanIdx].curseType)
            {
                case CurseType.Petrification:
                    allPetrificationStacs -= Stacs[shamanIdx];
                    speedTarget = speedDefault;
                    break;
                case CurseType.Clumsiness:
                    allClumsinessStacs -= Stacs[shamanIdx];
                    weapon.damage = weapon.damageDefault;
                    break;
            }
            Stacs.Remove(shamanIdx);
        }
    }
    public override void Die()
    {
        speedCurrent = 0;
        HealthScrol.value = HealthImg.fillAmount = 0;
        SpeedScrol.value = SpeedImg.fillAmount = 0;
        Cross.enabled = true;
        GameController.PlayersScript.Remove(this);
        GameController.instance.PlayerDie();
        StopAllCoroutines();
        myTransform.GetComponent<SpriteRenderer>().enabled = false;
        myTransform.GetComponentInChildren<SpriteRenderer>().enabled = false;
        Destroy(gameObject, courotineTime + 0.1f);
    }
}