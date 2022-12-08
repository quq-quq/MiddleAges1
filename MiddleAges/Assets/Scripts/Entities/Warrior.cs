using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : EntityBehaviour
{
    private static float distStopGoingToPlace = 1f;//���� ���������� ������ � ������ ����� ��� ������ ���������� �� ���� ������ ������ distStopGoingToPlace

    //���� ��������� �� ������ ������ ��� distToRunOutofPlaer � �� �������� � ���� ������ (angleToGoOutFromPlayer - ������� ����� ����) �� �� ������� �� ������
    private static float distToRunOutofPlaer = 10f;
    private static float angleToGoOutFromPlayer = 0.5f;

    [System.NonSerialized] public int index;

    protected override void Start() {
        base.Start(); 
        GameController.instance.WarriorsScript.Add(this);
    }

    protected override void Update() => base.Update();

    private Vector3 movementVector, warriorPos;
    private Vector3 rotationVector;
    private float distToPlayer;
    void FixedUpdate()
    {
        warriorPos = Player.instance.MovingAngle * GameController.instance.WarriorPositions[index] + Player.instance.plTransform.position;
        distToPlayer = Vector3.Distance(myTransform.position, warriorPos);

        rotationVector = (Quaternion.Inverse(Player.instance.MovingAngle) * (myTransform.position - Player.instance.plTransform.position)).normalized;//��������� ����� ������������ ����������� �������� ������

        if (rotationVector.z >= angleToGoOutFromPlayer && distToPlayer < distToRunOutofPlaer)
        {
            movementVector = Vector3.Lerp(movementVector, myTransform.rotation * Vector3.right + Player.instance.movementVector, Time.fixedDeltaTime);

            if (rotationVector.x >= 0) movementVector.x *= -1;

            movementVector.y = 0;
            controller.Move(movementVector * currentSpeed * Time.deltaTime);

        }
        else if (distToPlayer >= distStopGoingToPlace)//����� � ������ ����� ���� �� ������� ������ �� ����
        {
            movementVector = (warriorPos - myTransform.position).normalized;
            movementVector.y = 0;
            controller.Move(movementVector * currentSpeed * Time.deltaTime);
        }
    }

    public void SetSlowlingCurse()
    {
        targetSpeed = 0;
    }

}
