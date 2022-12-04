using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Player : EntityBehaviour
{ 
    public static Transform plTransform;
    public static Vector3 MovementVector = Vector3.zero;
    public static Quaternion MovingAngle = Quaternion.identity;//������� ��������� ����� �����, �������� ������� ������

    public Transform dashPos;
    public int speedOfDash;

    private bool isDash;
    private float disToMyPlace;
    [SerializeField]private Vector3 toDashPos;

    private delegate void Moving();
    private Moving WhatMove;

    private void Awake()
    {
        plTransform = transform;
    }

    protected override void Start()
    {
        base.Start();
        WhatMove = Wasd;
    }

    protected override void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        dashPos.localEulerAngles = new Vector3(0, Mathf.Atan2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Mathf.Rad2Deg, 0);
        WhatMove();

        if (Input.GetKeyDown(KeyCode.E) && !isDash)
        {
            toDashPos = new Vector3(dashPos.GetChild(0).transform.position.x, -2f, dashPos.GetChild(0).transform.position.z);
            WhatMove = StartDash;
        }
    }


    

    private void Wasd()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            MovementVector = plTransform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
            MovingAngle = Quaternion.Euler(0, Mathf.Atan2(MovementVector.x, MovementVector.z) * Mathf.Rad2Deg, 0);
            controller.Move(MovementVector * speed * Time.deltaTime);
        }

    }

    private void StartDash()
    {
        disToMyPlace = Vector3.Distance(new Vector3(transform.position.x, -2f, transform.position.z), toDashPos);
        if (disToMyPlace >= 1f)
        {
            MovementVector = (toDashPos - transform.position).normalized;
            MovementVector = new Vector3(MovementVector.x, -2f, MovementVector.z);
            controller.Move(MovementVector * speedOfDash * Time.deltaTime);
            isDash = true;
        }
        else
        {
            StartCoroutine("ToDash");
        }
    }

    IEnumerator ToDash()
    {
        yield return new WaitForSeconds(1f);
        isDash = false;
        WhatMove = Wasd;
    }
}