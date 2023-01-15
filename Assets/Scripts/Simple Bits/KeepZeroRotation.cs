using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepZeroRotation : MonoBehaviour
{
    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
