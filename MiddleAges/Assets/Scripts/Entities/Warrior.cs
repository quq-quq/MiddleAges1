using System.Collections;
using UnityEngine;

public class Warrior : EntityBehaviour
{

    private static float distGoAttack = 20f;//как далеко может уйти воин от своего командира преследуя врага
    private static float distStopGoingToPlace = 1f;//воин перестанет бежать к своему месту как только расстояние до него станет равным distStopGoingToPlace

    //Если дистанция до игрока меньше чем distToRunOutofPlaer и мы попадаем в угол обзора (angleToGoOutFromPlayer - косинус этого угла) то мы убегаем от игрока
    private static float distToRunOutofPlaer = 10f;
    private static float angleToGoOutFromPlayer = 0.5f;

    [System.NonSerialized] public int index;
    [System.NonSerialized] public int MyCapitanIndex;

    private Transform nearestEnemyTransform;
    private bool isAttack = false;

    private int idxClumsinessShaman = -1;
    private int idxPetrificationShaman = -1;
    private bool cursePetrification = false;
    private bool curseClumsines = false;

    private void Start()
    {
        GameController.WarriorScripts.Add(this);
        BaseStart();
        StartCoroutine(FindGoal());
    }
    private void Update() => BaseUpdate();

    private Vector3 warriorPos;
    private Vector3 rotationVector;
    private float distToPlayer;
    private void FixedUpdate()
    {
        if (!cursePetrification)
        {
            if (isAttack)
            {
                if (nearestEnemyTransform != null && Vector3.Distance(nearestEnemyTransform.position, myTransform.position) < weapon.radiusAttack)
                {
                    movementVector = Vector3.zero;
                    weapon.Attack();
                }
                else
                    movementVector = (nearestEnemyTransform.position - myTransform.position).normalized;
            }
            else
            {
                warriorPos = GameController.instance.PlayerScripts[MyCapitanIndex].MovingAngle *
                    GameController.WarriorPositions[MyCapitanIndex][index] + GameController.instance.PlayerScripts[MyCapitanIndex].myTransform.position;

                distToPlayer = Vector3.Distance(myTransform.position, warriorPos);
                rotationVector = (Quaternion.Inverse(GameController.instance.PlayerScripts[MyCapitanIndex].MovingAngle) *
                    (myTransform.position - GameController.instance.PlayerScripts[MyCapitanIndex].transform.position)).normalized;//положение война относительно направления движения игрока

                if (rotationVector.z >= angleToGoOutFromPlayer && distToPlayer < distToRunOutofPlaer)
                {
                    movementVector = Vector3.Lerp(movementVector, myTransform.rotation * Vector3.right + Player.instance.movementVector, Time.fixedDeltaTime);
                    if (rotationVector.x >= 0) movementVector.x *= -1;
                }
                else if (distToPlayer >= distStopGoingToPlace)//бежим к своему месту если мы слишком далеко от него
                    movementVector = (warriorPos - myTransform.position).normalized;
                else
                    movementVector = Vector3.zero;
            }
        }

        anim.SetBool("IsRunning", movementVector * speedCurrent != Vector3.zero);
        BaseFixedUpdate();

        if (myTransform.InverseTransformDirection(movementVector).x < -0.1f)
            transform.localScale = new Vector3(-startScale, transform.localScale.y, transform.localScale.z);
        else if (myTransform.InverseTransformDirection(movementVector).x > 0.1f)
            transform.localScale = new Vector3(startScale, transform.localScale.y, transform.localScale.z);

    }

    private IEnumerator FindGoal()
    {
        float dist, minDistToEnemy;
        while (true)
        {
            minDistToEnemy = float.MaxValue;
            yield return new WaitForSeconds(0.5f);

            if (GameController.EnemyScripts.Count != 0)
                foreach (var enemy in GameController.EnemyScripts)//ищем ближайший дом
                {
                    dist = Vector3.Distance(myTransform.position, enemy.transform.position);
                    if (dist < minDistToEnemy)
                    {
                        minDistToEnemy = dist;
                        nearestEnemyTransform = enemy.transform;
                        weapon.AttackTransform = nearestEnemyTransform;
                        if (GameController.instance.PlayerScripts[MyCapitanIndex] == null)
                            isAttack = Vector3.Distance(nearestEnemyTransform.position, myTransform.position) < 10f;
                        else
                            isAttack = Vector3.Distance(nearestEnemyTransform.position, GameController.instance.PlayerScripts[MyCapitanIndex].transform.position) < distGoAttack;
                    }
                }
            else
                isAttack = false;
            
        }
    }

    public void SetCurse(int shamanIdx)
    {
        switch (GameController.ShamanScripts[shamanIdx].curseType)
        {
            case CurseType.Petrification:
                if (idxPetrificationShaman == shamanIdx) break;

                StartCoroutine(CurseEffect(Color.gray, true));
                idxPetrificationShaman = shamanIdx;
                cursePetrification = true;
                speedTarget = 0f;
                anim.speed = 0f;
                break;

            case CurseType.Clumsiness:
                if (idxClumsinessShaman == shamanIdx) break;

                idxClumsinessShaman = shamanIdx;
                StartCoroutine(CurseEffect(Color.gray, false));
                curseClumsines = true;
                weapon.damage = 0;
                break;
        }
    }
    public void RemoveCurse(int shamanIdx)
    {
        if (idxPetrificationShaman == shamanIdx)
        {
            StartCoroutine(CurseEffect(Color.white, true));
            cursePetrification = false;
            speedTarget = speedDefault;
            anim.speed = 1f;
        }
        else if (idxClumsinessShaman == shamanIdx)
        {
            weapon.damage = weapon.damageDefault;
            StartCoroutine(CurseEffect(Color.white, false));
            curseClumsines = false;
        }
    }
    private IEnumerator CurseEffect(Color targetColor, bool isPetrificationCurse)
    {
        SpriteRenderer mySprite = gameObject.GetComponent<SpriteRenderer>();
        SpriteRenderer gunSprite = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        int counter = 0;
        while (counter++ < 30)
        {
            gunSprite.color = Color.Lerp(mySprite.color, targetColor, 0.1f);
            if (isPetrificationCurse)
                mySprite.color = Color.Lerp(mySprite.color, targetColor, 0.1f);
            yield return new WaitForSeconds(0.01f);
        }
        gunSprite.color = targetColor;
        if (isPetrificationCurse)
            mySprite.color = targetColor;
    }
    public override void Die()
    {
        GameController.WarriorScripts.Remove(this);
        GameController.instance.CalculateWarriorsPos(MyCapitanIndex);
        StopAllCoroutines();
        weapon.GetComponent<SpriteRenderer>().enabled = false;
        myTransform.GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, courotineTime+0.1f);
    }
}
