using System.Collections;
using System.Collections.Generic;
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

    [System.NonSerialized] public bool SlowlingCurse = true;

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
        if (!SlowlingCurse)
        {
            if (isAttack)
            {
                movementVector = (nearestEnemyTransform.position - myTransform.position).normalized;
                movementVector.y = 0;
                controller.Move(movementVector * speedCurrent * Time.fixedDeltaTime);
            }
            else
            {
                warriorPos = GameController.CapitansScripts[MyCapitanIndex].MovingAngle * GameController.WarriorPositions[MyCapitanIndex, index] + GameController.CapitansScripts[MyCapitanIndex].myTransform.position;
                distToPlayer = Vector3.Distance(myTransform.position, warriorPos);

                rotationVector = (Quaternion.Inverse(GameController.CapitansScripts[MyCapitanIndex].MovingAngle) * (myTransform.position - GameController.CapitansScripts[MyCapitanIndex].transform.position)).normalized;//��������� ����� ������������ ����������� �������� ������

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
                    isAttack = Vector3.Distance(nearestEnemyTransform.position, GameController.CapitansScripts[MyCapitanIndex].transform.position) < distGoAttack;
                }
            }
        }
    }

    private void SetClumsinessCurse()
    {
        damageCurrent = (1f - ++stacsClumsiness / (Shaman.S_stacs[(int)curseType] * 3f)) * speedDefault;
    }
}
