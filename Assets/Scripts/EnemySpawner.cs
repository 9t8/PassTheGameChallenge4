using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform minSpawn, maxSpawn;

    public GameObject enemyToSpawn;

    public float timeBetweenSpawns;
    private float spawnCounter;
    public bool spawnInside;

    // Start is called before the first frame update
    void Start()
    {
        spawnCounter = timeBetweenSpawns;
    }

    // Update is called once per frame
    void Update()
    {
        spawnCounter -= Time.deltaTime;

        if(spawnCounter <= 0)
        {
            SpawnEnemy();

            spawnCounter = timeBetweenSpawns;
        }
    }

    void SpawnEnemy()
    {
        
        //spawn left, top or right
        int spawnChoice = Random.Range(0, 2);
        Vector3 spawnPos = Vector3.zero;

        
        if (spawnInside)
        {
            spawnPos = new Vector3(Random.Range(minSpawn.position.x, maxSpawn.position.x),
                0,
                Random.Range(minSpawn.position.z, maxSpawn.position.z));
        }
else
        switch(spawnChoice)
        {
            case 0: //spawn on left side

                spawnPos.x = minSpawn.position.x;
                spawnPos.y = Random.Range(minSpawn.position.y, maxSpawn.position.y);

                break;

            case 1: //spawn on left side

                spawnPos.x = Random.Range(minSpawn.position.x, maxSpawn.position.x);
                spawnPos.y = maxSpawn.position.y;

                break;

            case 2: //spawn on right side

                spawnPos.x = maxSpawn.position.x;
                spawnPos.y = Random.Range(minSpawn.position.y, maxSpawn.position.y);

                break;
        }

        var newEnemy = Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
        newEnemy.gameObject.SetActive(true);
    }
}
