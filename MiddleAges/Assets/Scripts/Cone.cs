using System.Collections;
using UnityEngine;

public class Cone : EntityBehaviour
{
    [SerializeField] Shaman shaman;
    private Vector3 targetPos;

    private void Start()
    {
        myTransform = transform;

        targetPos = (shaman.transform.position - Player.instance.transform.position).normalized * shaman.curseRadius;
        myTransform.position = shaman.transform.position + targetPos;
    }

    private void FixedUpdate()
    {
        transform.position = shaman.myTransform.position + targetPos;
    }

    public void Teleport(Vector2 pos)
    {
        RaycastHit hit;
        Vector3 teleportPos = new Vector3(pos.x + myTransform.position.x, 30, pos.y + myTransform.position.z);
        Physics.Raycast(teleportPos, Vector3.down, out hit);
        teleportPos.y = hit.point.y + 0.5f;
        myTransform.position = shaman.myTransform.position + teleportPos;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            shaman.Die();
    }
}
