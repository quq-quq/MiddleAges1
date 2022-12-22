using System.Collections;
using UnityEngine;

public class Enemy : EntityBehaviour
{

    private delegate void FixedUpdateMethods();
    private FixedUpdateMethods State;

    void Start()
    {
        GameController.EnemyScripts.Add(this);
        BaseStart();
        State = GoToPlayerTeamState;
        StartCoroutine(FindGoal());
    }
    private void Update() => BaseUpdate();

    void FixedUpdate()
    {
        State();
        BaseFixedUpdate();

        if (CameraMoving.CamTransform.rotation.y > -90 && CameraMoving.CamTransform.rotation.y < 90)
        {
            if ((movementVector.z > 0 && movementVector.x > 0) || (movementVector.z > 0 && movementVector.x < 0))
                transform.localScale = new Vector3(-startScale, transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(startScale, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            if ((movementVector.z > 0 && movementVector.x > 0) || (movementVector.z > 0 && movementVector.x < 0))
                transform.localScale = new Vector3(startScale, transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(-startScale, transform.localScale.y, transform.localScale.z);
        }
    }

    private Transform nearestHouseTransform;
    private Transform nearestWarriorTransform;
    private IEnumerator FindGoal()
    {
        float dist, minDistToHouse;
        float nearestPlayer;
        while (true)
        {
            minDistToHouse = float.MaxValue;
            nearestPlayer = float.MaxValue;

            yield return new WaitForSeconds(courotineTime);

            if(GameController.HouseScripts.Count != 0)
            foreach (var house in GameController.HouseScripts)//ищем ближайший дом
            {
                dist = Vector3.Distance(myTransform.position, house.transform.position);
                if (dist < minDistToHouse)
                {
                    minDistToHouse = dist;
                    nearestHouseTransform = house.transform;
                }
            }
            foreach (var player in GameController.instance.PlayerScripts)
                if (player != null)
                nearestPlayer = Mathf.Min(nearestPlayer, Vector3.Distance(myTransform.position, player.myTransform.position));

            if (nearestPlayer < minDistToHouse)//если игрок ближе чем ближайший дом то идем к ближайшему войну
            {
                State = GoToPlayerTeamState;
            }
            else if (minDistToHouse != float.MaxValue)
            {
                State = GoToHomeState;
                weapon.AttackTransform = nearestHouseTransform;
            }
            else
            {
                State = null;
                weapon.AttackTransform = null;
            }
        }
    }
    private float minDistToWarrior, dist;
    private void GoToPlayerTeamState()
    {
        minDistToWarrior = float.MaxValue;
        nearestWarriorTransform = Player.instance.myTransform;
        foreach (var Player in GameController.instance.PlayerScripts)
        {
            if (Player != null)
            {
                dist = Vector3.Distance(myTransform.position, Player.myTransform.position);
                if (dist < minDistToWarrior)
                {
                    minDistToWarrior = dist;
                    nearestWarriorTransform = Player.myTransform;
                }
            }
        }
        if(GameController.WarriorScripts.Count != 0)
        foreach (var warrior in GameController.WarriorScripts)
        {
            dist = Vector3.Distance(myTransform.position, warrior.myTransform.position);
            if (dist < minDistToWarrior)
            {
                minDistToWarrior = dist;
                nearestWarriorTransform = warrior.myTransform;
            }
        }

        if (minDistToWarrior < weapon.radiusAttack)
        {
            movementVector = Vector3.zero;
            weapon.AttackTransform = nearestWarriorTransform;
            weapon.Attack();
        }
        else
            movementVector = (nearestWarriorTransform.position - myTransform.position).normalized;
    }
    private void GoToHomeState()
    {
        if (Physics.OverlapSphere(myTransform.position, 1.5f, 1 << 7).Length > 0)
        {
            movementVector = Vector3.zero;
            weapon.Attack();
        }
        else
            movementVector = (nearestHouseTransform.position - myTransform.position).normalized;
    }
    public override void Die()
    {
        print("EnemyDie");
        StopAllCoroutines();
        GameController.EnemyScripts.Remove(this);
        GameController.instance.CheckWin();
        weapon.GetComponent<SpriteRenderer>().enabled = false;
        myTransform.GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, courotineTime + 0.1f);
    }
}
