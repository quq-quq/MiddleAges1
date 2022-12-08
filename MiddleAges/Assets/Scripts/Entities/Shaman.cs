using System.Collections;
using UnityEngine;

public class Shaman : EntityBehaviour
{
    [SerializeField] private float distStopGoingToPlace = 10;//��� ������ ��������� � enemy �� ������� �� �������

    public float curseRadius; //������ ���������
    [SerializeField] private float teleportRadius; //������ �� ������� �� ������� �� ������
    [SerializeField] private float teleportReloadTime; //"�����������" ���������


    private delegate void FixedUpdateMethods();
    private FixedUpdateMethods fixedUpdate;

    [System.NonSerialized] public Vector3 movementVector = Vector3.zero;
    private Vector3 randomMovementVector = Vector3.zero;

    private bool isPlayerInCureZone = false;
    [SerializeField] private GameObject cone;

    private int stacs;
    public int SStacs;

    protected override void Start()
    {
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
        if (Vector3.Distance(Player.instance.plTransform.position, myTransform.position) >= curseRadius - 2)
            movementVector = (Player.instance.plTransform.position - myTransform.position).normalized;
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
        NearestEnemyTransform = Player.instance.plTransform;//����� ���� �� ��� ������ ������ �����, ����� ������ �������
        while (!isPlayerInCureZone)
        {
            yield return new WaitForSeconds(1);

            foreach(var enemy in GameController.instance.EnemiesScript)
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
            yield return new WaitForSeconds(0.5f);

            Collider[] warriorsInZone = Physics.OverlapSphere(myTransform.position, curseRadius, 8); //��� ��� ����� � ���������� �� ���� ������
            for (int i = 0; i < warriorsInZone.Length; ++i)
            {
                if (warriorsInZone[i].CompareTag("Player"))
                {
                    if (!isPlayerInCureZone)
                    {
                        isPlayerInCureZone = true;
                        fixedUpdate = PlayerInCurseZone;
                        cone.SetActive(true);
                    }
                    Player.instance.SetCurse(++stacs);
                } else
                    warriorsInZone[i].GetComponent<Warrior>().SetSlowlingCurse();
            }

            if (stacs == SStacs)
            {
                transform.GetComponent<SpriteRenderer>().enabled = false;
                cone.GetComponent<MeshRenderer>().enabled = false;
            }

            // if (stacs < 2 * SStacs)
            //     GameController.instance.Vignette.Intensity = stacs / (SStacs * 2f);
            // else if (stacs == 2 * SStacs)
            // {
            //     GameController.instance.Vignette.Intensity = 0f;
            // }

        }
    }

    private IEnumerator Teleporting()
    {
        while (true)
        {
            yield return new WaitUntil(() => Vector3.Distance(Player.instance.plTransform.position, myTransform.position) <= teleportRadius);
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
