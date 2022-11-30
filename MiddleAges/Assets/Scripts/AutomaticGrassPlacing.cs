using System.Collections;
using UnityEngine;

public class AutomaticGrassPlacing : MonoBehaviour
{
    [Range(50f, 200f)] 
    [SerializeField] private float grassSpawnRadius = 100;
    [Range(10f, 100f)]
    [SerializeField] private float grassCircleWide = 60;
    [Range(0.1f, 20f)]
    [SerializeField] private float grassSpacing = 0.2f;// расстояние между травинками

    [SerializeField] private float treeInGrassShance = 0.001f;// шанс того что в траве заспавнится дерево (при значение 0.01 на 100 травы будет примерно 1 дерево)

    [Range(10f, 100f)]
    [SerializeField] private float treeCircleWide = 100;
    [Range(1f, 30f)]
    [SerializeField] private float treeSpacing = 6;

    void Start()
    {
        CreateGrass();
        CreateTrees();
    }
    private void CreateGrass()
    {
        Vector3 pos = Vector3.zero;
        //Vector3 scale = new Vector3(1.4f, 1.4f, 1.4f);
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
                //scale.y = 1.4f + Random.Range(-0.25f, 0.25f);

                if (Random.value > treeInGrassShance)
                    Instantiate( grassObj [Random.Range(0, grassObj.Length)], pos, rot);
                else
                    Instantiate( treeObj[Random.Range(0, treeObj.Length)], new Vector3(pos.x, 0, pos.z), rot);
            }
        }
    }
    private void CreateTrees()
    {
        Vector3 pos = Vector3.zero;
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

                Instantiate(obj[Random.Range(0, obj.Length)], pos, rot);
            }
        }
        Debug.Log("Created " + (int)(iter + treeCircleWide / treeSpacing) + " trees");
    }

}
