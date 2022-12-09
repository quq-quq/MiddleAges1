using System.Collections;
using UnityEngine;

public class Warrior : EntityBehaviour
{

    private static float distGoAttack = 15f;//��� ������ ����� ���� ���� �� ������ ��������� ��������� �����
    private static float distStopGoingToPlace = 1f;//���� ���������� ������ � ������ ����� ��� ������ ���������� �� ���� ������ ������ distStopGoingToPlace

    //���� ��������� �� ������ ������ ��� distToRunOutofPlaer � �� �������� � ���� ������ (angleToGoOutFromPlayer - ������� ����� ����) �� �� ������� �� ������
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
    protected override void Start()
    {
        base.Start();
        StartCoroutine(FindGoal());
    }
    protected override void Update() => base.Update();


    private Vector3 movementVector, warriorPos;
    private Vector3 rotationVector;
    private float distToPlayer;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!cursePetrification)
        {
            if (isAttack)
            {
                movementVector = (nearestEnemyTransform.position - myTransform.position).normalized;
                movementVector.y = 0;
                controller.Move(movementVector * speedCurrent * Time.fixedDeltaTime);
            }
            else
            {
                warriorPos = GameController.CapitansScript[MyCapitanIndex].MovingAngle * GameController.WarriorPositions[MyCapitanIndex, index] + GameController.CapitansScript[MyCapitanIndex].myTransform.position;
                distToPlayer = Vector3.Distance(myTransform.position, warriorPos);

                rotationVector = (Quaternion.Inverse(GameController.CapitansScript[MyCapitanIndex].MovingAngle) * (myTransform.position - GameController.CapitansScript[MyCapitanIndex].transform.position)).normalized;//��������� ����� ������������ ����������� �������� ������

                if (rotationVector.z >= angleToGoOutFromPlayer && distToPlayer < distToRunOutofPlaer)
                {
                    movementVector = Vector3.Lerp(movementVector, myTransform.rotation * Vector3.right + Player.instance.movementVector, Time.fixedDeltaTime);

                    if (rotationVector.x >= 0) movementVector.x *= -1;

                    movementVector.y = 0;
                    controller.Move(movementVector * speedCurrent * Time.deltaTime);

                }
                else if (distToPlayer >= distStopGoingToPlace)//����� � ������ ����� ���� �� ������� ������ �� ����
                {
                    movementVector = (warriorPos - myTransform.position).normalized;
                    movementVector.y = 0;
                    controller.Move(movementVector * speedCurrent * Time.deltaTime);
                }
            }
        }
    }

    private IEnumerator FindGoal()
    {
        float dist, minDistToEnemy;
        while (true)
        {
            minDistToEnemy = float.MaxValue;
            yield return new WaitForSeconds(0.5f);

            foreach (var enemy in GameController.EnemiesScript)//���� ��������� ���
            {
                dist = Vector3.Distance(myTransform.position, enemy.transform.position);
                if (dist < minDistToEnemy)
                {
                    minDistToEnemy = dist;
                    nearestEnemyTransform = enemy.transform;
                    isAttack = Vector3.Distance(nearestEnemyTransform.position, GameController.CapitansScript[MyCapitanIndex].transform.position) < distGoAttack;
                }
            }
        }
    }

    public void SetCurse(int shamanIdx)
    {
        switch (GameController.ShamansScript[shamanIdx].curseType)
        {
            case CurseType.Petrification:
                cursePetrification = true;
                idxPetrificationShaman = shamanIdx;
                break;
            case CurseType.Clumsiness:
                damageCurrent = 0;
                idxClumsinessShaman = shamanIdx;
                break;
        }
    }
    public void RemoveCurse(int shamanIdx)
    {
        if (idxPetrificationShaman == shamanIdx)
            cursePetrification = false;

        if (idxClumsinessShaman == shamanIdx)
            damageCurrent = damageDefault;
    }
}
