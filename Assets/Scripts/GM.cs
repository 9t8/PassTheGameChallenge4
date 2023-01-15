using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{
    public static GM instance;

    bool hasGameStarted = false;

    private void Awake()
    {
        instance = this;
    }

    public List<Corpse> corpsesInStorage = new List<Corpse>();
    //private int corpses;

    public float maxHunger;
    private float currentHunger;

    public float hungerDecreaseSpeed;

    public float waitToShowGameOver = 1.5f;
    public GameObject playerDeathEffect;
    public Player _player;

    public bool HasGameStarted() {
        return hasGameStarted;
    }

    private void Start()
    {
        currentHunger = maxHunger;
        _player = FindObjectOfType<Player>();

        UIController.instance.UpdateHungerBar(maxHunger, currentHunger);
        UIController.instance.UpdateHealthBar(_player.maxHealth, _player.health);
    }

    private void LateUpdate()
    {
        UIController.instance.UpdateHealthBar(_player.maxHealth, _player.health);
        if(currentHunger > 0 && (currentHunger - hungerDecreaseSpeed * Time.deltaTime) > 0)
        {
            currentHunger -= hungerDecreaseSpeed * Time.deltaTime;
            UIController.instance.UpdateHungerBar(maxHunger, currentHunger);
        }
    }

    public void AddCorpse(Corpse corpseToAdd)
    {
        corpsesInStorage.Add(corpseToAdd);
        WorldCanvasController.instance.UpdateCorpseCount(corpsesInStorage.Count);
    }

    

    public void RemoveCorpse(Corpse corpseToRemove)
    {
        if(corpsesInStorage.Contains(corpseToRemove))
        {
            corpsesInStorage.Remove(corpseToRemove);
        } else
        {
            Debug.LogError("Selected corpse not found in active corpses list");
        }

        WorldCanvasController.instance.UpdateCorpseCount(corpsesInStorage.Count);
    }

    public bool CanSpendExtraHunger(float extraHunger) {
        return currentHunger - extraHunger >= 0;
    }

    public void SpendExtraHunger(float extraHunger)
    {
            currentHunger -= extraHunger;
    }

    public void RestoreHunger(float hungerToRestore)
    {
        currentHunger += hungerToRestore;

        if(currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }
    }

    public void CallGameOver(){
        StartCoroutine(GameOverCo());
    }

    IEnumerator GameOverCo()
    {
        Player thePlayer = FindObjectOfType<Player>();
        thePlayer.gameObject.SetActive(false);
        Instantiate(playerDeathEffect, thePlayer.transform.position, Quaternion.identity);


        yield return new WaitForSeconds(waitToShowGameOver);

        UIController.instance.gameOverScreen.SetActive(true);
    } 
    
    public void CallGameWon(){
        StartCoroutine(GameWonCo());
    }

    public void StartGame() {
        hasGameStarted = true;
    }

    IEnumerator GameWonCo()
    {
        Player thePlayer = FindObjectOfType<Player>();
        thePlayer.invulnerable = true;

        yield return new WaitForSeconds(waitToShowGameOver);
        if (thePlayer.health > 0) {
            UIController.instance.gameWonScreen.SetActive(true);
        }
    }
}
