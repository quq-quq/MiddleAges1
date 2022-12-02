using UnityEngine;

public class Player : EntityBehaviour
{ 
    public static Transform playerTransform;
    public static Vector3 movementVector;

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
        movementVector = playerTransform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        controller.Move(movementVector * speed * Time.deltaTime);
    }
}
