using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Slider> hungerBars;
    public List<Slider> healthBars;

    public bool shakeHungerBar;
    public float hungerGrowSpeed;
    public float hungerGrowAmount;
    private bool hungerGrowing;
    public TMP_Text corpseText;



    public Color fullColour, lowColor, dangerColor;
    public Color fullHealthColour, lowHealthColor, dangerHealthColor;
    [Range(0f, 100f)] public float lowHungerLevel, dangerHungerLevel;
    public List<Image> hungerBarFillImages;

    public GameObject pickupCorpseMessage, placeInMasherMessage, commandSoldierToHealMessage, feedLarvaMessage;

    public GameObject gameOverScreen;
    public GameObject gameWonScreen;

    void Start() {
        SetCorpseUIActive(false);
    }

    public void UpdateHungerBar(float maxHunger, float currentHunger)
    {
        foreach (var bar in hungerBars)
        {
            bar.maxValue = maxHunger;
            if (currentHunger < 1) {
                return;
            }
            bar.value = maxHunger - currentHunger;
        }

        foreach (var fillImage in hungerBarFillImages)
        {
            //update bar colour
            if (fillImage != null)
            {
                float hungerPercent = (currentHunger / maxHunger) * 100f;

                if (hungerPercent > lowHungerLevel)
                {
                    fillImage.color = fullColour;
                }
                else if (hungerPercent > dangerHungerLevel)
                {
                    fillImage.color = lowColor;
                }
                else
                {
                    fillImage.color = dangerColor;
                }
            }
        }
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        foreach (var bar in healthBars)
        {
            bar.maxValue = maxHealth;
            bar.value =  Mathf.Max(maxHealth - currentHealth, 0);
        }
    }

    private void Update()
    {
        foreach (var hungerBar in hungerBars)
            if (shakeHungerBar)
            {
                if (hungerGrowing)
                {
                    hungerBar.transform.localScale = Vector3.MoveTowards(hungerBar.transform.localScale,
                        new Vector3(1f, 1f + hungerGrowAmount, 1f), hungerGrowSpeed * Time.deltaTime);
                    if (hungerBar.transform.localScale.y >= 1f + hungerGrowAmount - .001f)
                    {
                        hungerGrowing = false;
                    }
                }
                else
                {
                    hungerBar.transform.localScale = Vector3.MoveTowards(hungerBar.transform.localScale, Vector3.one,
                        hungerGrowSpeed * Time.deltaTime);
                    if (hungerBar.transform.localScale.y <= 1f + .001f)
                    {
                        hungerGrowing = true;
                    }
                }
            }
            else
            {
                hungerBar.transform.localScale = Vector3.one;
            }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetCorpseUIActive(bool isActive) {
        corpseText.gameObject.SetActive(isActive);
    }

    public void UpdateCorpseUICount(float corpseCount)
    {
        corpseText.text = "Deliver " + corpseCount + " corpses to Masher!";
    }
}