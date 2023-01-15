using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutdoorArea : MonoBehaviour
{
    public static OutdoorArea instance;

    private void Awake()
    {
        instance = this;
    }

    public Transform minPatrolPoint, maxPatrolPoint;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(minPatrolPoint.position.x, minPatrolPoint.position.y, 0f), new Vector3(minPatrolPoint.position.x, maxPatrolPoint.position.y, 0f));
        Gizmos.DrawLine(new Vector3(maxPatrolPoint.position.x, maxPatrolPoint.position.y, 0f), new Vector3(minPatrolPoint.position.x, maxPatrolPoint.position.y, 0f));
        Gizmos.DrawLine(new Vector3(minPatrolPoint.position.x, minPatrolPoint.position.y, 0f), new Vector3(maxPatrolPoint.position.x, minPatrolPoint.position.y, 0f));
        Gizmos.DrawLine(new Vector3(maxPatrolPoint.position.x, maxPatrolPoint.position.y, 0f), new Vector3(maxPatrolPoint.position.x, minPatrolPoint.position.y, 0f));
    }
}
