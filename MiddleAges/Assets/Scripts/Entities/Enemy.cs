using System.Collections;
using UnityEngine;

public class Enemy : EntityBehaviour
{
    private delegate void FixedUpdateMethods();
    private FixedUpdateMethods fixedUpdate;

    private Vector3 movementVector;


    private void Awake() => GameController.EnemiesScript.Add(this);
    protected override void Start()
    {
        base.Start();
        fixedUpdate = GoToPlayerTeam;
        StartCoroutine(FindGoal());
    }


    protected override void Update() { base.Update(); }
    void FixedUpdate()
    {
        fixedUpdate();
        controller.Move(movementVector * speed * Time.deltaTime);
    }


    private Transform nearestHouseTransform;
    private Transform nearestWarriorTransform;
    private IEnumerator FindGoal()
    {
        float dist, minDistToHouse = float.MaxValue;
        while (true)
        {
            yield return new WaitForSeconds(0.4f);

            foreach (var house in GameController.HousesScripts)//ищем ближайший дом
            {
                dist = Vector3.Distance(myTransform.position, house.transform.position);
                if (dist < minDistToHouse)
                {
                    minDistToHouse = dist;
                    nearestHouseTransform = house.transform;
                }
            }

            if (Vector3.Distance(Player.plTransform.position, myTransform.position) < minDistToHouse)//если игрок ближе чем ближайший дом то идем к ближайшему войну
                fixedUpdate = GoToPlayerTeam;
            else
                fixedUpdate = GoToHome;
        }
    }
    private float minDistToWarrior, dist;
    private void GoToPlayerTeam()
    {
        minDistToWarrior = Vector3.Distance(myTransform.position, Player.plTransform.position);
        nearestWarriorTransform = Player.plTransform;
        foreach (var warrior in GameController.WarriorsScript)
        {
            dist = Vector3.Distance(myTransform.position, warrior.myTransform.position);
            if (dist < minDistToWarrior)
            {
                minDistToWarrior = dist;
                nearestWarriorTransform = warrior.myTransform;
            }
        }
        movementVector = (nearestWarriorTransform.position - myTransform.position).normalized;
    }
    private void GoToHome() => movementVector = (nearestHouseTransform.position - myTransform.position).normalized;

}
