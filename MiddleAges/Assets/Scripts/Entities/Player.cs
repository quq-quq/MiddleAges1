using UnityEngine;
using System.Collections;

public class Player : EntityBehaviour
{
    public static Player instance;

    [System.NonSerialized] public Vector3 movementVector = Vector3.zero;
    [System.NonSerialized] public Quaternion MovingAngle = Quaternion.identity;//поможет вычислить место война, учитывая поворот игрока

    public float DashSpeed;
    public float DashDistance;
    public float DashReloadTime;
    private bool isDash;

    private delegate void FixedUpdateMethods();
    private FixedUpdateMethods State;

    private Transform nearestEnemyTransform;
    private bool isAttack = false;

    private void Awake()
    {
        GameController.CapitansScripts.Add(this);
        State = IdleState;
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.E) && !isDash && State != IdleState)
        {
            State = DashState;
            StartCoroutine(ToDash());
            isDash = true;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        State();
    }

    private void WasdState()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            movementVector = myTransform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
            MovingAngle = Quaternion.Euler(0, Mathf.Atan2(movementVector.x, movementVector.z) * Mathf.Rad2Deg, 0);
            controller.Move(movementVector * currentSpeed * Time.deltaTime);
        }
    }
    private void DashState()
    {
        movementVector = MovingAngle * Vector3.forward;
        controller.Move(movementVector * DashSpeed * Time.deltaTime);
    }
    private void IdleState()
    {
        if (isAttack)
        {
            movementVector = (nearestEnemyTransform.position - myTransform.position).normalized;
            movementVector.y = 0;
            controller.Move(movementVector * currentSpeed * Time.fixedDeltaTime);
        }
    }
    IEnumerator ToDash()
    {
        //начали dash
        yield return new WaitForSeconds(DashDistance / DashSpeed);//
        State = WasdState;
        yield return new WaitForSeconds(DashReloadTime);
        isDash = false;
    }
    public void SetCurse(int stacs)
    {
        targetSpeed = (1 - stacs / 120f) * defaultSpeed;
    }

    public void ActivatePlayer()
    {
        State = WasdState;
        CameraMoving.target = myTransform;
        StopCoroutine(FindGoal());
    }
    public void DeactivatePlayer()
    {
        State = IdleState;
        StartCoroutine(FindGoal());
    }

    private IEnumerator FindGoal()
    {
        float dist, minDistToEnemy;
        while (true)
        {
            minDistToEnemy = float.MaxValue;
            yield return new WaitForSeconds(0.5f);

            foreach (var enemy in GameController.EnemiesScript)//ищем ближайший дом
            {
                dist = Vector3.Distance(myTransform.position, enemy.transform.position);
                if (dist < minDistToEnemy)
                {
                    minDistToEnemy = dist;
                    nearestEnemyTransform = enemy.transform;
                    isAttack = Vector3.Distance(nearestEnemyTransform.position, myTransform.position) < 5f;
                }
            }
        }
    }

    public void ChangeSpeed()
    {
        targetSpeed = defaultSpeed;
    }
}