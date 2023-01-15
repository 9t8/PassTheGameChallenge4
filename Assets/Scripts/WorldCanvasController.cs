using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldCanvasController : MonoBehaviour
{
    public static WorldCanvasController instance;

    private void Awake()
    {
        instance = this;
    }


    public TMP_Text corpseText;

    public TMP_Text killKingText;
    public TMP_Text hurtQueenText;
    public TMP_Text foodText;
    public GameObject food;
    public float feedFadeOutSpeed, feedFadeInSpeed;
    public float foodFadeOutSpeed = 2.0f;
    public float waitToFade;
    private float currentWaitToFade;
    bool shouldFade = false;
    bool hurtQueenTextFaded = false;
    bool hurtQueenTextShown = false;
    bool killKingTextShown = false;
    

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IntroCutsceneCo());
    }

    // Update is called once per frame
    void Update()
    {
        if (!hurtQueenTextShown && shouldFade) {
            hurtQueenText.color = new Color(hurtQueenText.color.r, hurtQueenText.color.g, hurtQueenText.color.b, Mathf.MoveTowards(hurtQueenText.color.a, 1.0f, feedFadeOutSpeed * Time.deltaTime));
        }
        else if (hurtQueenTextShown && !hurtQueenTextFaded) {
            hurtQueenText.color = new Color(hurtQueenText.color.r, hurtQueenText.color.g, hurtQueenText.color.b, Mathf.MoveTowards(hurtQueenText.color.a, 0f, feedFadeOutSpeed * Time.deltaTime));
        }

        if (hurtQueenTextFaded && !killKingTextShown) {
            killKingText.color = new Color(killKingText.color.r, killKingText.color.g, killKingText.color.b, Mathf.MoveTowards(killKingText.color.a, 1.0f, feedFadeOutSpeed * Time.deltaTime));
        } else if (hurtQueenTextFaded && killKingTextShown) {
            killKingText.color = new Color(killKingText.color.r, killKingText.color.g, killKingText.color.b, Mathf.MoveTowards(killKingText.color.a, 0f, feedFadeOutSpeed * Time.deltaTime));
        }

        if (foodText && !food) {
            foodText.color = new Color(foodText.color.r, foodText.color.g, foodText.color.b, Mathf.MoveTowards(foodText.color.a, 0f, feedFadeOutSpeed * Time.deltaTime));
        }
    }

    
    IEnumerator IntroCutsceneCo()
    {
        yield return new WaitForSeconds(1.0f);
        shouldFade = true;
        yield return new WaitUntil(() =>  hurtQueenText.color.a >= 1);
        yield return new WaitForSeconds(1.0f);
        hurtQueenTextShown = true;
        GM.instance.StartGame();
        yield return new WaitUntil(() =>  hurtQueenText.color.a <= 0);
        hurtQueenTextFaded = true;

        yield return new WaitUntil(() =>  killKingText.color.a >= 1);
        killKingTextShown = true;
        yield return null;
    } 


    public void UpdateCorpseCount(float corpseCount)
    {
        corpseText.text = corpseCount + " corpses";
    }
}
