using UnityEngine;

public class Player : EntityBehaviour
{ 
    public static Transform playerTransform;
    protected override void Start()
    {
        base.Start();
        playerTransform = transform;
    }

    protected override void Update()
    {
        base.Update();
    }

    private Vector3 movementVector;
    private void FixedUpdate()
    {
        movementVector = playerTransform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), -2, Input.GetAxis("Vertical")));
        controller.Move(movementVector * speed * Time.deltaTime);

    }
}
