using System.Collections;
using UnityEngine;

public class Shaman : EntityBehaviour
{
    [SerializeField] private float distStopGoingToPlace = 10;//как близко подбегать к enemy за которым мы следуем

    public float curseRadius; //радиус проклятья
    [SerializeField] private float teleportRadius; //радиус на котором мы тпшимся от игрока
    [SerializeField] private float teleportReloadTime; //"перезарядка" телепорта

    private int stacs;
    [SerializeField] private int SStacs = 30;

    private delegate void FixedUpdateMethods();
    private FixedUpdateMethods fixedUpdate;

    [System.NonSerialized] public Vector3 movementVector = Vector3.zero;
    private Vector3 randomMovementVector = Vector3.zero;

    private Transform playerTransform;
    private bool isPlayerInCureZone = false;
    [SerializeField] private GameObject cone;

    protected override void Start()
    {
        playerTransform = GameController.CapitansScripts[0].transform;
        base.Start();
        fixedUpdate = GoToEnemy;
        StartCoroutine(SettingCurse());
        StartCoroutine(Teleporting());
        StartCoroutine(FindNearestEnemy());
        StartCoroutine(RandomMoving());
    }

    protected override void Update() { base.Update(); }

    void FixedUpdate()
    {
        fixedUpdate();
    }

    private void PlayerInCurseZone()
    {

        if (Vector3.Distance(playerTransform.position, myTransform.position) >= curseRadius - 2)
            movementVector = (playerTransform.position - myTransform.position).normalized;
        else
            movementVector = Vector3.Lerp(movementVector, randomMovementVector, Time.fixedDeltaTime);

        movementVector.y = 0;
        controller.Move(movementVector * currentSpeed * Time.deltaTime);
    }
    private IEnumerator RandomMoving()
    {
        yield return new WaitUntil(() => isPlayerInCureZone);
        while (true)
        {
            randomMovementVector = Random.insideUnitSphere.normalized;
            yield return new WaitForSeconds(Random.Range(1f, 5f));
        }
    }

    private Transform NearestEnemyTransform;
    private IEnumerator FindNearestEnemy()
    {
        float dist, minDist = float.MaxValue;
        NearestEnemyTransform = Player.instance.myTransform;//чтобы было за кем бежать первое время, иначе ошибка вылазит
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
            controller.Move(movementVector * currentSpeed * Time.deltaTime);
        }
    }

    private IEnumerator SettingCurse()
    {
        while (true)
        {
            Collider[] warriorsInZone = Physics.OverlapSphere(myTransform.position, curseRadius, 8); //все кто попал в окружность на слое войнов
            for (int i = 0; i < warriorsInZone.Length; ++i)
            {
                if (warriorsInZone[i].CompareTag("Player"))
                {
                    if (!isPlayerInCureZone)
                    {
                        playerTransform = warriorsInZone[i].transform;
                        isPlayerInCureZone = true;
                        fixedUpdate = PlayerInCurseZone;
                        cone.SetActive(true);
                    }
                    warriorsInZone[i].GetComponent<Player>().SetCurse(++stacs);

                    if(stacs == SStacs)
                    {
                        myTransform.GetComponent<SpriteRenderer>().sprite = null;
                        cone.GetComponent<MeshRenderer>().enabled = false;
                    }
                    if(stacs < SStacs * 2)
                        GameController.instance.VignetteIntensity = stacs / (SStacs*2f);
                    else if(stacs == SStacs*2)
                        GameController.instance.VignetteIntensity = 0;

                }
                else
                    warriorsInZone[i].GetComponent<Warrior>().isNotPetrified = false;
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator Teleporting()
    {
        while (true)
        {
            yield return new WaitUntil(() => Vector3.Distance(playerTransform.position, myTransform.position) <= teleportRadius);
            Teleport();
            yield return new WaitForSeconds(teleportReloadTime);
        }
    }
    private void Teleport()
    {
        RaycastHit hit;
        Vector2 pos = Random.insideUnitCircle.normalized*curseRadius;
        Vector3 teleportPos = new Vector3(pos.x + myTransform.position.x, 30, pos.y + myTransform.position.z);
        Physics.Raycast(teleportPos, Vector3.down, out hit);

        if (Mathf.Abs(hit.point.y - myTransform.position.y) < 6f)
        {
            teleportPos.y = hit.point.y + controller.height / 2f;
            controller.Move(myTransform.position - teleportPos);
            cone.GetComponent<Cone>().Teleport(pos);
        }
        else//если он собрался тпшиться на крышу дома или еще куда не надо
        {
            Teleport();
        }
    }
    private void DieFixedUpdate()
    {
        myTransform.GetChild(0).transform.position += Vector3.down*Time.fixedDeltaTime*5;
    }
    public void Die()
    {
        StopAllCoroutines();
        fixedUpdate = DieFixedUpdate;
        myTransform.GetComponent<SpriteRenderer>().sprite = null;
        Destroy(cone);
        Destroy(myTransform.parent.gameObject, 5f);
    }
}
