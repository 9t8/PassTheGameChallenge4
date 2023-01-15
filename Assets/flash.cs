using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class flash : MonoBehaviour
{
    
    float flashRate = 1f;
    float currentTimer;
    TMP_Text corpseText;

    // Start is called before the first frame update
    void Start()
    {
        corpseText = GetComponent<TMP_Text>();
        currentTimer = flashRate;
    }

    void Update() {
        currentTimer -=Time.deltaTime;

        if (currentTimer < 0) {
            currentTimer = flashRate;
            corpseText.enabled = !corpseText.enabled;
        }
    }
}
