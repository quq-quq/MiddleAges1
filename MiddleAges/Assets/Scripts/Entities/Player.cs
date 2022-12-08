using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Player : EntityBehaviour
{
    public static Player instance;

    [System.NonSerialized] public Transform plTransform;
    [System.NonSerialized] public Vector3 movementVector = Vector3.zero;
    [System.NonSerialized] public Quaternion MovingAngle = Quaternion.identity;//поможет вычислить место война, учитывая поворот игрока

    public float DashSpeed;
    public float DashDistance;
    public float DashReloadTime;
    private bool isDash;


    private delegate void Moving();
    private Moving MoveType;

    private void Awake()
    {
        instance = this;
        plTransform = transform;
    }

    protected override void Start()
    {
        base.Start();
        MoveType = Wasd;
    }

    protected override void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        MoveType();

        if (Input.GetKeyDown(KeyCode.E) && !isDash)
        {
            MoveType = StartDash;
            StartCoroutine(ToDash());
            isDash = true;
        }
    }

    private void Wasd()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            movementVector = plTransform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
            MovingAngle = Quaternion.Euler(0, Mathf.Atan2(movementVector.x, movementVector.z) * Mathf.Rad2Deg, 0);
            controller.Move(movementVector * currentSpeed * Time.deltaTime);
        }
    }
    private void StartDash()
    {
        movementVector = MovingAngle * Vector3.forward;
        controller.Move(movementVector * DashSpeed * Time.deltaTime);
    }
    IEnumerator ToDash()
    {
        //начали dash
        yield return new WaitForSeconds(DashDistance / DashSpeed);//
        MoveType = Wasd;
        yield return new WaitForSeconds(DashReloadTime);
        isDash = false;
    }
    public void SetCurse(int stacs)
    {
        targetSpeed = (1 - stacs / 120f) * defaultSpeed;
    }

}