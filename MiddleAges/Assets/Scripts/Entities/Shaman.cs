using System.Collections;
using UnityEngine;

public class Shaman : EntityBehaviour
{
    [SerializeField] private float curseRadius;
    [SerializeField] private float teleportRadius;
    [SerializeField] private float teleportReloadTime;
    [SerializeField] private LayerMask playerLayer;
    protected override void Start()
    {
        base.Start();
        StartCoroutine(SettingCurse());
        StartCoroutine(Teleporting());
    }

    protected override void Update()
    {
        base.Update();
    }

    Vector3 movementVector = new Vector3(1, -2, 1);
    void FixedUpdate()
    {
        //movementVector.x = 5 * Mathf.Sin(Time.deltaTime*1000000);//ходьба просто так
        //movementVector.z = 5 * Mathf.Cos(Time.deltaTime*1000000);//можно спокойно убрать, если делать новый алгоритм ходьбы
        
        //controller.Move(movementVector * speed * Time.deltaTime);
    }

    private IEnumerator SettingCurse()
    {
        while (true)
        {
            Collider[] warriorsInZone = Physics.OverlapSphere(transform.position, curseRadius, playerLayer);
            for (int i = 0; i < warriorsInZone.Length; ++i)
                warriorsInZone[i].GetComponent<EntityBehaviour>().SetCurse();
            
            yield return new WaitForSeconds(0.5f);
        }
    }
    private IEnumerator Teleporting()
    {
        while (true)
        {
            yield return new WaitUntil(() => Vector3.Distance(Player.playerTransform.position, transform.position) <= teleportRadius);
            Teleport();
            yield return new WaitForSeconds(teleportReloadTime);
        }
    }

    private void Teleport()
    {
        RaycastHit hit;
        Vector2 pos = curseRadius * Random.insideUnitCircle;
        Physics.Raycast(new Vector3(pos.x, 30, pos.y), Vector3.down, out hit);
        transform.Translate(new Vector3(pos.x, hit.point.y+0.5f, pos.y));
    }
}
