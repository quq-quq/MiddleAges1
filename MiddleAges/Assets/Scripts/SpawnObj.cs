using System.Collections;
using UnityEngine;

public class SpawnObj : MonoBehaviour
{
    [Range(50f, 200f)] 
    [SerializeField] private float grassSpawnRadius = 100;    // радиус круга дальше которого начинает спавниться трава
    [Range(10f, 100f)]
    [SerializeField] private float grassCircleWide = 60;      // ширина круга травы
    [Range(0.1f, 20f)]
    [SerializeField] private float grassSpacing = 0.2f;       // расстояние между травинками

    [SerializeField] private float treeInGrassShance = 0.001f;// шанс того что в траве заспавнится дерево (при значение 0.01 на 100 травы будет примерно 1 дерево)

    [Range(10f, 100f)]
    [SerializeField] private float treeCircleWide = 100;      // ширина круга деревьев
    [Range(1f, 30f)]
    [SerializeField] private float treeSpacing = 6;           // расстояние между деревьями

    [SerializeField] GameObject[] enemiesObj;
    [SerializeField] int countAllEnemiesSpawn;
    [SerializeField] float intervalSpawn;

    void Start()
    {
        CreateGrass();
        CreateTrees();
        StartCoroutine(Spawner());
    }

    IEnumerator Spawner()
    {
        for (int i = 0; i < countAllEnemiesSpawn; i++)
        {
            yield return new WaitForSeconds(5);
            Vector2 randCircle = Random.insideUnitCircle.normalized;
            Vector3 SpawnPoint = Vector3.zero;
            SpawnPoint.x = randCircle.x * (grassSpawnRadius + grassCircleWide + 10) + Random.Range(1, 5f);
            SpawnPoint.z = randCircle.y * (grassSpawnRadius + grassCircleWide + 10) + Random.Range(1, 5f);
            foreach (var enemy in enemiesObj)
            {
                Vector3 spawnPos = new Vector3(SpawnPoint.x + Random.Range(0, 10f), 5, SpawnPoint.z + Random.Range(0, 10f));
                Instantiate(enemy, spawnPos, transform.rotation);
            }
            if (i % 2 == 0)
                yield return new WaitForSeconds(5); 
            else
                yield return new WaitUntil(() => (GameController.EnemiesScript.Count == 0));
        }
    }

    private void CreateGrass()
    {
        Vector3 pos = Vector3.zero;
        Vector3 scale = Vector3.one;
        Quaternion rot = new Quaternion(0, 0, 0, 180);

        GameObject [] grassObj, treeObj;
        grassObj = Resources.LoadAll<GameObject>("Prefabs/Grass/");
        treeObj = Resources.LoadAll<GameObject>("Prefabs/Trees/");


        float iter = 0;
        for (float rad = grassSpawnRadius; rad < grassSpawnRadius + grassCircleWide; rad += grassSpacing) 
        {
            iter = 2f * Mathf.PI * rad / grassSpacing;
            for (float i = 0; i < 360; i += 360 / rad)
            {
                pos.x = rad * Mathf.Cos(i) + Random.Range(-grassSpacing, grassSpacing);
                pos.z = rad * Mathf.Sin(i) + Random.Range(-grassSpacing, grassSpacing);

                rot.y = Random.Range(-360, 360);
                scale.y = Random.Range(0.75f, 1.25f);

                if (Random.value > treeInGrassShance)
                    Instantiate( grassObj [Random.Range(0, grassObj.Length)], pos, rot).transform.localScale = scale;
                else
                    Instantiate( treeObj[Random.Range(0, treeObj.Length)], new Vector3(pos.x, 0, pos.z), rot).transform.localScale = scale;
            }
        }
    }
    private void CreateTrees()
    {
        Vector3 pos = Vector3.zero;
        Vector3 scale = Vector3.one;
        Quaternion rot = new Quaternion(0, 0, 0, 180);

        GameObject[] obj;
        obj = Resources.LoadAll<GameObject>("Prefabs/Trees/");


        float iter = 0;
        for (float rad = grassSpawnRadius + grassCircleWide; rad < grassSpawnRadius + grassCircleWide + treeCircleWide; rad += treeSpacing) 
        {
            iter = 2f * Mathf.PI * rad / treeSpacing;

            for (float i = 0; i < 360; i += 360 / rad)
            {
                pos.x = rad * Mathf.Cos(i) + Random.Range(-treeSpacing / 2, treeSpacing / 2);
                pos.z = rad * Mathf.Sin(i) + Random.Range(-treeSpacing / 2, treeSpacing / 2);

                rot.y = Random.Range(-360, 360);
                scale.y = Random.Range(0.75f, 1.25f);

                Instantiate(obj[Random.Range(0, obj.Length)], pos, rot).transform.localScale = scale;
            }
        }
    }

}
