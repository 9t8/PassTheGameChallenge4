using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseRoomPoint : MonoBehaviour
{
    public float minX, maxX, minY, maxY;

    private void Start()
    {
        
    }

    public void MoveToRandomPoint()
    {
        transform.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(minX, minY, 0f), new Vector3(minX, maxY, 0f));
        Gizmos.DrawLine(new Vector3(maxX, maxY, 0f), new Vector3(minX, maxY, 0f));
        Gizmos.DrawLine(new Vector3(minX, minY, 0f), new Vector3(maxX, minY, 0f));
        Gizmos.DrawLine(new Vector3(maxX, maxY, 0f), new Vector3(maxX, minY, 0f));
    }
}
