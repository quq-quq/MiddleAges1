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
        base.Update();
    }

    protected override void Update()
    {
        base.Update();
        

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
        yield return new WaitUntil(() => Vector3.Distance(Player.playerTransform.position, transform.position) <= teleportRadius);
        Teleport();
        yield return new WaitForSeconds(teleportReloadTime);
    }

    private void Teleport()
    {
        RaycastHit hit;
        Vector2 pos = curseRadius * Random.insideUnitCircle;
        Physics.Raycast(new Vector3(pos.x, 30, pos.y), Vector3.down, out hit);
        transform.Translate(new Vector3(pos.x, hit.point.y+0.5f, pos.y));
    }
}
