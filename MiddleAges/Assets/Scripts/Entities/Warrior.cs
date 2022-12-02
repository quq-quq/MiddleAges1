using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : EntityBehaviour
{
    [SerializeField] private static float distStopGoinrToPlace = 1;//��������� �� ������ �� ������� ���� ����� � ������ �����
    private Vector3 warriorPos;

    public int index;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    private Vector3 movementVector, playerMonementVector = Vector3.one;
    private float disToMyPlace, playerMovingAngle;
    void FixedUpdate()
    {
        if (Player.movementVector.x* Player.movementVector.z != 0)//���� ����� �� z � �� x ������ �� ��������� 
            playerMonementVector = Player.movementVector;
        //����� ����������, ������ ��������� ���� � ����
        playerMovingAngle = Mathf.Atan2(playerMonementVector.x, playerMonementVector.z) * Mathf.Rad2Deg;
        warriorPos = Quaternion.Euler(0, playerMovingAngle, 0) * WarriorsMoving.WarriorPositions[index] + Player.playerTransform.position;

        disToMyPlace = Vector3.Distance(transform.position, warriorPos);

        if (disToMyPlace >= distStopGoinrToPlace)//����� � ������ ����� ���� �� ������� ������ �� ����
        {
            movementVector = (warriorPos - transform.position).normalized;
            movementVector.y = 0;
            controller.Move(movementVector * speed * Time.deltaTime);
        }
    }
}
