using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : EntityBehaviour
{
    public static Player instance;

    [System.NonSerialized] public Vector3 movementVector = Vector3.zero;
    [System.NonSerialized] public Quaternion MovingAngle = Quaternion.identity;//поможет вычислить место война, учитывая поворот игрока


    public float DashSpeed;
    public float DashDistance;
    public float DashReloadTime;
    private bool isDash;

    private Dictionary<int, int> Stacs = new();
    private int allPetrificationStacs = 0;
    private int allClumsinessStacs = 0;

    private Transform nearestEnemyTransform;
    private bool isAttack = false;

    private delegate void FixedUpdateMethods();
    private FixedUpdateMethods State;
    private void Awake()
    {
        GameController.CapitansScript.Add(this);
        State = IdleState;
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.E) && !isDash && State == WasdState)
        {
            State = DashState;
            StartCoroutine(ToDash());
            isDash = true;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        State();
    }

    private void WasdState()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            movementVector = myTransform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
            MovingAngle = Quaternion.Euler(0, Mathf.Atan2(movementVector.x, movementVector.z) * Mathf.Rad2Deg, 0);
            controller.Move(movementVector * speedCurrent * Time.deltaTime);
        }
    }
    private void DashState()
    {
        movementVector = MovingAngle * Vector3.forward;
        controller.Move(movementVector * DashSpeed * Time.deltaTime);
    }
    private void IdleState()
    {
        if (isAttack)
        {
            movementVector = (nearestEnemyTransform.position - myTransform.position).normalized;
            movementVector.y = 0;
            controller.Move(movementVector * speedCurrent * Time.fixedDeltaTime);
        }
    }
    IEnumerator ToDash()
    {
        //начали dash
        yield return new WaitForSeconds(DashDistance / DashSpeed);
        State = WasdState;
        yield return new WaitForSeconds(DashReloadTime);
        isDash = false;
    }

    public void ActivatePlayer()
    {
        State = WasdState;
        CameraMoving.target = myTransform;
        GameController.instance.VignetteIntensity =
            Mathf.Lerp(0, 0.75f, (allPetrificationStacs + allClumsinessStacs) / (float)(Shaman.S_stacs[1] + Shaman.S_stacs[0]));
        StopCoroutine(FindGoal());
    }
    public void DeactivatePlayer()
    {
        State = IdleState;
        StartCoroutine(FindGoal());
    }

    private IEnumerator FindGoal()
    {
        float dist, minDistToEnemy;
        while (true)
        {
            minDistToEnemy = float.MaxValue;
            yield return new WaitForSeconds(0.5f);

            foreach (var enemy in GameController.EnemiesScript)
            {
                dist = Vector3.Distance(myTransform.position, enemy.transform.position);
                if (dist < minDistToEnemy)
                {
                    minDistToEnemy = dist;
                    nearestEnemyTransform = enemy.transform;
                    isAttack = Vector3.Distance(nearestEnemyTransform.position, myTransform.position) < 5f;
                }
            }
        }
    }

    public int SetCurse(int shamanIdx)
    {
        if (!Stacs.ContainsKey(shamanIdx)) Stacs.Add(shamanIdx, 0);
        Stacs[shamanIdx] += 1;


        if (State == WasdState)
            GameController.instance.VignetteIntensity =
                Mathf.Lerp(0, 0.75f, (allPetrificationStacs + allClumsinessStacs) / (float)(Shaman.S_stacs[1] + Shaman.S_stacs[0]));

        switch (GameController.ShamansScript[shamanIdx].curseType)
        {
            case CurseType.Petrification:
                allPetrificationStacs += 1;
                speedTarget = Mathf.Clamp(1f - allPetrificationStacs / (Shaman.S_stacs[(int)CurseType.Petrification] * 3f), 0, 1) * speedDefault;
                return Stacs[shamanIdx];

            case CurseType.Clumsiness:
                allClumsinessStacs += 1;
                damageCurrent = Mathf.Clamp(1f - allClumsinessStacs / (Shaman.S_stacs[(int)CurseType.Clumsiness] * 3f), 0, 1) * speedDefault;
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
                    damageCurrent = speedDefault;
                    break;
            }
            Stacs.Remove(shamanIdx);
        }
    }
}