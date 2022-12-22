using System.Collections;
using UnityEngine;

public class Shaman : EntityBehaviour
{
    private readonly static float distStopGoingToPlace = 10;//��� ������ ��������� � enemy �� ������� �� �������

    //0 ������� � ���������� S ������ ��� ��������� ����������
    //1 ������� � ���������� S ������ ��� ��������� �����������(�������� �����) 
    public static int[] S_stacs = { 20, 20 };
    //[System.NonSerialized] 
    public int index = 0;

    public float curseRadius = 15f;                          //������ ���������
    public CurseType curseType = CurseType.Petrification;             //��� ���������
    [SerializeField] private float teleportRadius = 5f;         //������ �� ������� �� ������� �� ������
    [SerializeField] private float teleportReloadTime = 3f;     //"�����������" ���������


    private Transform playerTransform;
    private bool isPlayerInCureZone = false;
    [SerializeField] private GameObject cone;

    private Vector3 randomMovementVector = Vector3.zero;

    private delegate void FixedUpdateMethods();
    private FixedUpdateMethods State;

    private void Awake()
    {
    }
    private void Start()
    {
        BaseStart();

        index = GameController.ShamanScripts.Count;
        GameController.ShamanScripts.Add(this);

        foreach (var player in GameController.instance.PlayerScripts)
            if (player != null)
                playerTransform = player.transform;

        if (playerTransform == null) this.enabled = false;

        State = GoToEnemyState;
        StartCoroutine(SettingCurse());
        StartCoroutine(Teleporting());
        StartCoroutine(FindNearestEnemy());
        StartCoroutine(RandomMoving());
    }
    private void Update() => BaseUpdate();
    private void FixedUpdate()
    {
        State(); 
        BaseFixedUpdate();

        if ((movementVector.z > 0 && movementVector.x > 0) || (movementVector.z > 0 && movementVector.x < 0))
            transform.localScale = new Vector3(-startScale, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(startScale, transform.localScale.y, transform.localScale.z);
    }

    private void PlayerInCurseZoneState()
    {
        // ���� ����� ������ �� ���, �� ����� �� ���, ����� �������� ������ �����
        if (Vector3.Distance(playerTransform.position, myTransform.position) >= curseRadius)
            movementVector = (playerTransform.position - myTransform.position).normalized;
        else
            movementVector = Vector3.Lerp(movementVector, randomMovementVector, Time.fixedDeltaTime);
    }
    private void GoToEnemyState()
    {
        // ����� �� ��������� ������
        if (Vector3.Distance(myTransform.position, NearestEnemyTransform.position) > distStopGoingToPlace)
            movementVector = (NearestEnemyTransform.position - myTransform.position).normalized;
    }
    private void DieState()
    {
        myTransform.GetChild(1).transform.position += Vector3.down*Time.fixedDeltaTime*10;
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
        while (!isPlayerInCureZone)
        {
            NearestEnemyTransform = Player.instance.transform;
            yield return new WaitForSeconds(courotineTime);

            if (GameController.EnemyScripts.Count != 0)
            foreach (var enemy in GameController.EnemyScripts)
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


    private IEnumerator SettingCurse()
    {
        int stacs;
        while (true)
        {
            yield return new WaitForSeconds(courotineTime);

            Collider[] warriorsInZone = Physics.OverlapSphere(myTransform.position, curseRadius, 8); //��� ��� ����� � ���������� �� ���� ������
            for (int i = 0; i < warriorsInZone.Length; ++i)
            {
                if (warriorsInZone[i].CompareTag("Player"))
                {
                    if (!isPlayerInCureZone)
                    {
                        playerTransform = warriorsInZone[i].transform;
                        isPlayerInCureZone = true;
                        State = PlayerInCurseZoneState;
                        cone.SetActive(true);
                    }
                    stacs = warriorsInZone[i].GetComponent<Player>().SetCurse(index);

                    if (stacs == S_stacs[(int)curseType])//��� ���������� S ������
                    {
                        cone.GetComponent<MeshRenderer>().enabled = false;
                        myTransform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                        myTransform.GetComponent<SpriteRenderer>().enabled = false;
                    } 
                    else if (stacs == S_stacs[(int)curseType] * 2)//��� ���������� 2 S ������
                    {
                        GameController.instance.VignetteIntensity = 0;
                    }

                }
                else
                    warriorsInZone[i].GetComponent<Warrior>().SetCurse(index);
                
            }
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
    public override void Die()
    {
        GameController.instance.RemoveCurse(index);
        GameController.ShamanScripts[index] = null;
        GameController.instance.CheckWin();

        StopAllCoroutines();
        movementVector = Vector3.zero;
        State = DieState;

        myTransform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        myTransform.GetComponent<SpriteRenderer>().enabled = false;

        Destroy(cone);
        Destroy(myTransform.parent.gameObject, 5f);
    }
}

public enum CurseType
{
    Petrification = 0,//idx in S_Pstacs
    Clumsiness = 1
}