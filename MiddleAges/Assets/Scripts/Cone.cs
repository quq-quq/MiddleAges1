using System.Collections;
using UnityEngine;

public class Cone : EntityBehaviour
{
    [SerializeField] Shaman shaman;
    private Vector3 targetPos;

    private Vector2 movementVector;
    protected override void Start()
    {
        base.Start();
        controller.enabled = false;
        targetPos = (shaman.transform.position - Player.instance.plTransform.position).normalized * shaman.curseRadius;
        targetPos.y = 30f;
        myTransform.position = shaman.transform.position + targetPos;
    }
    protected override void Update()
    {
    }

    private void FixedUpdate()
    {
        transform.position = shaman.myTransform.position + targetPos;
        controller.enabled = true;
        controller.Move(Vector3.down * 100f);
        controller.enabled = false;
    }

    public void Teleport(Vector2 pos)
    {
        RaycastHit hit;
        Vector3 teleportPos = new Vector3(pos.x + myTransform.position.x, 30, pos.y + myTransform.position.z);
        Physics.Raycast(teleportPos, Vector3.down, out hit);
        teleportPos.y = hit.point.y + controller.height / 2f;
        controller.enabled = false;
        myTransform.position = shaman.myTransform.position + teleportPos;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Player"))
        {
            GameController.instance.RemoveCurse();
            shaman.Die();
        }
    }
}
