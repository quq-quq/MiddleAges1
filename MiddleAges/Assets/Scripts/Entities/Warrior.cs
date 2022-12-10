using System.Collections;
using UnityEngine;

public class Warrior : EntityBehaviour
{

    private static float distGoAttack = 15f;//как далеко может уйти воин от своего командира преследуя врага
    private static float distStopGoingToPlace = 1f;//воин перестанет бежать к своему месту как только расстояние до него станет равным distStopGoingToPlace

    //Если дистанция до игрока меньше чем distToRunOutofPlaer и мы попадаем в угол обзора (angleToGoOutFromPlayer - косинус этого угла) то мы убегаем от игрока
    private static float distToRunOutofPlaer = 10f;
    private static float angleToGoOutFromPlayer = 0.5f;

    [System.NonSerialized] public int index;
    public int MyCapitanIndex = 0;

    private Transform nearestEnemyTransform;
    private bool isAttack = false;

    private int idxClumsinessShaman = 0;
    private int idxPetrificationShaman = 0;
    private bool cursePetrification = false;

    private void Awake() => GameController.WarriorsScript.Add(this);
    private void Start()
    {
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
                if (Vector3.Distance(nearestEnemyTransform.position, myTransform.position) < weapon.radiusAttack)
                {
                    movementVector = Vector3.zero;
                    weapon.Attack();
                }
                else
                    movementVector = (nearestEnemyTransform.position - myTransform.position).normalized;
            }
            else
            {
                warriorPos = GameController.PlayersScript[MyCapitanIndex].MovingAngle *
                    GameController.WarriorPositions[MyCapitanIndex][index] + GameController.PlayersScript[MyCapitanIndex].myTransform.position;

                distToPlayer = Vector3.Distance(myTransform.position, warriorPos);
                rotationVector = (Quaternion.Inverse(GameController.PlayersScript[MyCapitanIndex].MovingAngle) *
                    (myTransform.position - GameController.PlayersScript[MyCapitanIndex].transform.position)).normalized;//положение война относительно направления движения игрока

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

            if (GameController.EnemiesScript.Count == 0) isAttack = false;
            foreach (var enemy in GameController.EnemiesScript)//ищем ближайший дом
            {
                dist = Vector3.Distance(myTransform.position, enemy.transform.position);
                if (dist < minDistToEnemy)
                {
                    minDistToEnemy = dist;
                    nearestEnemyTransform = enemy.transform;
                    weapon.AttackTransform = nearestEnemyTransform;
                    isAttack = MyCapitanIndex >= GameController.PlayersScript.Count || Vector3.Distance(nearestEnemyTransform.position, GameController.PlayersScript[MyCapitanIndex].transform.position) < distGoAttack;
                }
            }
        }
    }

    public void SetCurse(int shamanIdx)
    {
        switch (GameController.ShamansScript[shamanIdx].curseType)
        {
            case CurseType.Petrification:

                idxPetrificationShaman = shamanIdx;
                speedTarget = 0f;
                cursePetrification = true;

                anim.speed = 0f;
                gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
                transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
                break;
            case CurseType.Clumsiness:
                idxClumsinessShaman = shamanIdx;
                weapon.damage = 0;
                break;
        }
    }
    public void RemoveCurse(int shamanIdx)
    {
        if (idxPetrificationShaman == shamanIdx)
        {
            cursePetrification = false;
            speedTarget = speedDefault;
            anim.speed = 1f;
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }

        if (idxClumsinessShaman == shamanIdx) 
            weapon.damage = weapon.damageDefault;
    }

    public override void Die()
    {
        GameController.WarriorsScript.Remove(this);
        GameController.instance.CalculateWarriorsPos(MyCapitanIndex);
        StopAllCoroutines();
        weapon.GetComponent<SpriteRenderer>().enabled = false;
        myTransform.GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, courotineTime+0.1f);
    }
}
