using UnityEngine;
using UnityEngine.UI;

public class Egg : MonoBehaviour
{
    public bool playerNear;

    public float waitTime;
    float nextWaitTime;
    public GameObject soldierPrefab;
    public Slider slider;
    public Transform spawnPoint;

    public float hungerPerSecond;

    void Start()
    {
        nextWaitTime = waitTime;
        slider.maxValue = waitTime;
        slider.value = waitTime;
    }

    void Update()
    {
        //if (playerNear)
        {
            nextWaitTime -= Time.deltaTime;
            slider.value = nextWaitTime;
            if (nextWaitTime <= 0)
            {
                nextWaitTime = waitTime;
                SpawnSoldier();
            }

            GM.instance.SpendExtraHunger(hungerPerSecond * Time.deltaTime);
        }
    }

    void SpawnSoldier()
    {
        Instantiate(soldierPrefab, spawnPoint.position, Quaternion.identity);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            //playerNear = true;
            UIController.instance.shakeHungerBar = true;
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            //playerNear = false;
            UIController.instance.shakeHungerBar = false;
        }
    }


}
