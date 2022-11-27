using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : EntityBehaniour
{
    [SerializeField] private static float distGoToPlayer = 5;//дистанция от игрока на которой воин остановится и перестанет бежать к игроку
    [SerializeField] private static float distRunOutFromPlayer = 2;//дистанция от игрока на которой воин убегает от игрока

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
