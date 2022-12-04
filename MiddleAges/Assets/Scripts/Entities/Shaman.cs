using System.Collections;
using UnityEngine;

public class Shaman : EntityBehaviour
{
    [SerializeField] private float distStopGoingToPlace = 10;//как близко подбегать к enemy за которым мы следуем

    [SerializeField] private float curseRadius; //радиус проклятья
    [SerializeField] private float teleportRadius; //радиус на котором мы тпшимся от игрока
    [SerializeField] private float teleportReloadTime; //"перезарядка" телепорта
    [SerializeField] private float Slowling; //на сколько будем уменьшать скорость всех в радиусе проклятья каждые 0,5 сек


    private delegate void FixedUpdateMethods();
    private FixedUpdateMethods fixedUpdate;

    Vector3 movementVector = Vector3.zero;
    private bool isPlayerInCureZone = false;

    protected override void Start()
    {
        base.Start();
        fixedUpdate = GoToEnemy;
        StartCoroutine(SettingCurse());
        StartCoroutine(Teleporting());
        StartCoroutine(FindNearestEnemy());
    }

    protected override void Update() { base.Update(); }

    void FixedUpdate()
    {
        fixedUpdate();
    }
    private void PlayerInCurseZone()
    {

    }
    private Transform NearestEnemyTransform;
    private IEnumerator FindNearestEnemy()
    {
        float dist, minDist = float.MaxValue;
        NearestEnemyTransform = Player.plTransform;//чтобы было за кем бежать первое время, иначе ошибка вылазит
        while (!isPlayerInCureZone)
        {
            yield return new WaitForSeconds(1);

            foreach(var enemy in GameController.EnemiesScript)
            {
                dist = Vector3.Distance(myTransform.position, enemy.myTransform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    NearestEnemyTransform = enemy.myTransform;
                }
            }
        }
    }
    private void GoToEnemy()
    {
        if (Vector3.Distance(myTransform.position, NearestEnemyTransform.position) > distStopGoingToPlace)
        {
            movementVector = (NearestEnemyTransform.position - myTransform.position).normalized;
            movementVector.y = 0;
            controller.Move(movementVector * speed * Time.deltaTime);
        }
    }

    private IEnumerator SettingCurse()
    {
        while (true)
        {
            Collider[] warriorsInZone = Physics.OverlapSphere(myTransform.position, curseRadius, 8); //все кто попал в окружность на слое войнов
            for (int i = 0; i < warriorsInZone.Length; ++i)
            {
                if (warriorsInZone[i].CompareTag("Player")) {
                    isPlayerInCureZone = true;
                    fixedUpdate = PlayerInCurseZone;
                    warriorsInZone[i].GetComponent<EntityBehaviour>().SetSlowlingCurse(Slowling/2f);
                } else
                    warriorsInZone[i].GetComponent<EntityBehaviour>().SetSlowlingCurse(Slowling);
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator Teleporting()
    {
        while (true)
        {
            yield return new WaitUntil(() => Vector3.Distance(Player.plTransform.position, myTransform.position) <= teleportRadius);
            Teleport();
            yield return new WaitForSeconds(teleportReloadTime);
        }
    }
    private void Teleport()
    {
        RaycastHit hit;
        float rx = Random.value > 0.5f ? Random.Range(teleportRadius * 2, curseRadius) : Random.Range(-curseRadius, -teleportRadius*2);
        float ry = Random.value > 0.5f ? Random.Range(teleportRadius * 2, curseRadius) : Random.Range(-curseRadius, -teleportRadius * 2);
        Vector3 teleportPos = new Vector3(rx + myTransform.position.x, 30, ry + myTransform.position.z);
        Physics.Raycast(teleportPos, Vector3.down, out hit);//чтобы не затпшиться внутрь горы
        teleportPos.y = hit.point.y + controller.height/2f;
        controller.Move(myTransform.position - teleportPos);
    }
}
