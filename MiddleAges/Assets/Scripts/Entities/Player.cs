using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Player : EntityBehaviour
{ 

    public static Transform playerTransform;
    public static Vector3 movementVector;
    public Transform dashPos;
    public int speedOfDash;

    private bool isDash;
    private float disToMyPlace;
    [SerializeField]private Vector3 toDashPos;

    private delegate void Moving();
    private Moving WhatMove;

    private void Awake()
    {
        playerTransform = transform;
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
            WhatMove =  StartDash;
        }
    }

    private void Wasd()
    {
        movementVector = playerTransform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), -2, Input.GetAxis("Vertical")));
        controller.Move(movementVector * speed * Time.deltaTime);
    }

    private void StartDash()
    {
        disToMyPlace = Vector3.Distance(new Vector3(transform.position.x, -2f, transform.position.z), toDashPos);
        if (disToMyPlace >= 1f)
        {
            movementVector = (toDashPos - transform.position).normalized;
            movementVector = new Vector3(movementVector.x, -2f, movementVector.z);
            controller.Move(movementVector * speedOfDash * Time.deltaTime);
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
