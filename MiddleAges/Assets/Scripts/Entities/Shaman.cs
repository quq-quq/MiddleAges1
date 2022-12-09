using System.Collections;
using UnityEngine;

public class Shaman : EntityBehaviour
{
    private readonly static float distStopGoingToPlace = 10;//��� ������ ��������� � enemy �� ������� �� �������

    //0 ������� � ���������� S ������ ��� ��������� ����������
    //1 ������� � ���������� S ������ ��� ��������� �����������(�������� �����) 
    public static int[] S_stacs = { 10, 10 };
    [System.NonSerialized] public int index = 0;

    public readonly float curseRadius = 15f;                             //������ ���������
    public CurseType curseType = CurseType.Petrification;       //��� ���������
    [SerializeField] private const float teleportRadius = 5f;         //������ �� ������� �� ������� �� ������
    [SerializeField] private const float teleportReloadTime = 3f;     //"�����������" ���������


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
        NearestEnemyTransform = Player.instance.transform;//����� ���� �� ��� ������ ������ �����, ����� ������ �������
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
            Collider[] warriorsInZone = Physics.OverlapSphere(myTransform.position, curseRadius, 8); //��� ��� ����� � ���������� �� ���� ������
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

                    if (stacs == S_stacs[(int)curseType])//��� ���������� S ������
                    {
                        myTransform.GetComponent<SpriteRenderer>().sprite = null;
                        cone.GetComponent<MeshRenderer>().enabled = false;
                    }
                    else if (stacs == S_stacs[(int)curseType] * 2) //��� ���������� 2 S ������
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
        else//���� �� �������� �������� �� ����� ���� ��� ��� ���� �� ����
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