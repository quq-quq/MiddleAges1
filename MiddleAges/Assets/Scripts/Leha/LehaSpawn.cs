using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LehaSpawn : MonoBehaviour
{
    [SerializeField] GameObject[] enemiesObj;
    [SerializeField] int countAllEnemiesSpawn;
    // [SerializeField] int countEnemiesForOneWave;
    [SerializeField] float intervalSpawn;
    [SerializeField] float radiusSpawn;
    public int countSpawnedEnemy;
    void Start()
    {
        StartCoroutine(Spawner());
    }

    IEnumerator Spawner()
    {
        for(int i = 0; i < countAllEnemiesSpawn; i++)
        {
            Spawn();
            yield return new WaitForSeconds(intervalSpawn);
        }
    }
    void Spawn()
    {
        Vector3 spawnPos = new Vector3();
        
        foreach (var enemy in enemiesObj)
        {
            spawnPos = new Vector3(Random.insideUnitCircle.x * radiusSpawn, 0, Random.insideUnitCircle.y * radiusSpawn) + transform.position;

            Instantiate(enemy, spawnPos, transform.rotation);
            countSpawnedEnemy++;
        }
        
    }
}
