using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodStorage : MonoBehaviour
{
    public static FoodStorage instance;
    private void Awake()
    {
        instance = this;
    }

    public Food foodToSpawn;

    public Transform minPos, maxPos, spawnPoint;

    public GameObject spawnEffect;

    public void SpawnFood()
    {
        Food newFood = Instantiate(foodToSpawn, spawnPoint.position, foodToSpawn.transform.rotation);
        newFood.targetPos = new Vector3(Random.Range(minPos.position.x, maxPos.position.x), Random.Range(minPos.position.y, maxPos.position.y), 0f);

        if(spawnEffect != null)
        {
            Instantiate(spawnEffect, spawnEffect.transform.position, spawnEffect.transform.rotation).SetActive(true);
        }
    }
}
