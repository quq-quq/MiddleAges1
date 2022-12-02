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

    private void Awake()
    {
        playerTransform = transform;
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        if ((Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)) && !isDash)
        {
            StartDash();
        }
        else if (!isDash)
        {
            movementVector = playerTransform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), -2, Input.GetAxis("Vertical")));
            controller.Move(movementVector * speed * Time.deltaTime);
            dashPos.localEulerAngles = new Vector3(0, Mathf.Atan2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Mathf.Rad2Deg, 0);
        }
    }

    private void StartDash()
    {
        disToMyPlace = Vector3.Distance(transform.position, dashPos.GetChild(0).position);
        while (disToMyPlace >= 1f)
        {
            movementVector = (dashPos.GetChild(0).position - transform.position).normalized;
            movementVector = new Vector3(movementVector.x, -2f, movementVector.z);
            controller.Move(movementVector * speedOfDash * Time.deltaTime);

        }
        StartCoroutine("ToDash");
    }

    IEnumerator ToDash()
    {
        isDash = true;
        yield return new WaitForSeconds(2f);
        isDash = false;
    }
}
