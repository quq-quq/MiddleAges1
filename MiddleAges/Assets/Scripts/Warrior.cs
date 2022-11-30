using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : EntityBehaviour
{
    [SerializeField] private static float distGoToPos = 5;//дистанция от игрока на которой воин остановится и перестанет бежать к игроку
    [SerializeField] private static float distRunOutFromPos = 2;//дистанция от игрока на которой воин убегает от игрока
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

    private Vector3 movementVector;
    private float distToPlayer;
    void FixedUpdate()
    {
        warriorPos = GameObject.Find("Player").transform.GetChild(index + 1).position;
        distToPlayer = Vector3.Distance(transform.position, warriorPos);
        movementVector = (warriorPos - transform.position).normalized;

        if (distToPlayer <= distRunOutFromPos)
        {
            movementVector = new Vector3(-movementVector.x, -2f, -movementVector.z);
            controller.Move(movementVector * speed * Time.deltaTime);
        }
        else if (distToPlayer >= distGoToPos)
        {
            movementVector = new Vector3(movementVector.x, -2f, movementVector.z);
            controller.Move(movementVector * speed * Time.deltaTime);
        }



    }
}
