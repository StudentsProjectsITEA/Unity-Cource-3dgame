using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

    public Transform[] spawnPoints;
    public Enemy enemyPrefab;
    public Player player;
    public int enemyCount;
    private Enemy[] enemies;

	void Start () {
        /*
        foreach (Transform item in spawnPoints)
        {
            Enemy temp = Instantiate(enemyPrefab, item.position, Quaternion.identity);
            temp.target = player.transform;
        }
        */
        enemies = new Enemy[enemyCount];
        Spawn();
	}

    public void Spawn()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Enemy temp = Instantiate(enemyPrefab, new Vector3(Random.Range(-10f, 10f), 0.1f, Random.Range(-10f, 10f)), Quaternion.identity);
            temp.target = player.transform;
            enemies[i] = temp;
        }
    }

    public void Clear()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy item in enemies)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
    }

    public void SpawnOnDeath()
    {
        Enemy temp = Instantiate(enemyPrefab, spawnPoints[Random.Range(0,3)].position, Quaternion.identity);
        temp.target = player.transform;
    }
}
