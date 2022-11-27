using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : EntityBehaniour
{
    [SerializeField] private static float distGoToPlayer = 5;//��������� �� ������ �� ������� ���� ����������� � ���������� ������ � ������
    [SerializeField] private static float distRunOutFromPlayer = 2;//��������� �� ������ �� ������� ���� ������� �� ������

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    private Vector3 movementVector;
    private float distToPlayer;
    void FixedUpdate()
    {
        distToPlayer = Vector3.Distance(transform.position, Player.playerTransform.position);
        movementVector = (Player.playerTransform.position - transform.position).normalized;

        if (distToPlayer <= distRunOutFromPlayer)
        {
            movementVector = new Vector3(-movementVector.x, -2f, -movementVector.z);
            controller.Move(movementVector * speed * Time.deltaTime);
        }
        else if (distToPlayer >= distGoToPlayer)
        {
            movementVector = new Vector3(movementVector.x, -2f, movementVector.z);
            controller.Move(movementVector * speed * Time.deltaTime);
        }



    }
}
