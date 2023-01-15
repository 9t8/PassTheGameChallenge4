using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBarController : MonoBehaviour
{
    public float maxValue = 1f, currentValue;

    public Transform fillBar;

    public void SetupBar(float max, float current)
    {
        maxValue = max;

        currentValue = current;
    }

    public void UpdateBar(float current)
    {
        currentValue = current;

        if (maxValue != 0)
        {
            float barSize = currentValue / maxValue;
            fillBar.localScale = new Vector3(barSize, fillBar.localScale.y, fillBar.localScale.z);
        } else
        {
            fillBar.localScale = new Vector3(0, fillBar.localScale.y, fillBar.localScale.z);
            Debug.LogError("No maxvalue set");
        }
    }
}
