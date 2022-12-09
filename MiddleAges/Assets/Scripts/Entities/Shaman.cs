using System.Collections;
using UnityEngine;

public class Shaman : EntityBehaviour
{
    private readonly static float distStopGoingToPlace = 10;//как близко подбегать к enemy за которым мы следуем

    //0 элемент Ч количество S стаков дл€ прокл€ть€ окаменени€
    //1 элемент Ч количество S стаков дл€ прокл€ть€ неуклюжести(снижение урона) 
    public static int[] S_stacs = { 10, 10 };
    [System.NonSerialized] public int index = 0;

    public readonly float curseRadius = 15f;                             //радиус прокл€ть€
    public CurseType curseType = CurseType.Petrification;       //тип прокл€ть€
    [SerializeField] private const float teleportRadius = 5f;         //радиус на котором мы тпшимс€ от игрока
    [SerializeField] private const float teleportReloadTime = 3f;     //"перезар€дка" телепорта


    private Transform playerTransform;
    private bool isPlayerInCureZone = false;
    [SerializeField] private GameObject cone;


    [System.NonSerialized] public Vector3 movementVector = Vector3.zero;
    private Vector3 randomMovementVector = Vector3.zero;

    private delegate void FixedUpdateMethods();
    private FixedUpdateMethods fixedUpdate;

    private void Awake() => GameController.ShamansScript.Add(this);
    protected override void Start()
    {
        playerTransform = GameController.CapitansScript[0].transform;
        base.Start();
        fixedUpdate = GoToEnemy;
        StartCoroutine(SettingCurse());
        StartCoroutine(Teleporting());
        StartCoroutine(FindNearestEnemy());
        StartCoroutine(RandomMoving());
    }

    protected override void Update() { base.Update(); }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        fixedUpdate();
    }

    private void PlayerInCurseZone()
    {
        if (Vector3.Distance(playerTransform.position, myTransform.position) >= curseRadius - 2)
            movementVector = (playerTransform.position - myTransform.position).normalized;
        else
            movementVector = Vector3.Lerp(movementVector, randomMovementVector, Time.fixedDeltaTime);

        movementVector.y = 0;
        controller.Move(movementVector * speedCurrent * Time.deltaTime);
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
        NearestEnemyTransform = Player.instance.transform;//чтобы было за кем бежать первое врем€, иначе ошибка вылазит
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
            controller.Move(movementVector * speedCurrent * Time.deltaTime);
        }
    }

    private IEnumerator SettingCurse()
    {
        int stacs;
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
                    stacs = warriorsInZone[i].GetComponent<Player>().SetCurse(index);

                    if (stacs == S_stacs[(int)curseType])//при достижении S стаков
                    {
                        myTransform.GetComponent<SpriteRenderer>().sprite = null;
                        cone.GetComponent<MeshRenderer>().enabled = false;
                    }
                    else if (stacs == S_stacs[(int)curseType] * 2) //при достижении 2 S стаков
                    {
                    }

                }
                else
                    warriorsInZone[i].GetComponent<Warrior>().SetCurse(index);
                
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
        else//если он собралс€ тпшитьс€ на крышу дома или еще куда не надо
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

public enum CurseType
{
    Petrification = 0,//idx in S_Pstacs
    Clumsiness = 1
}