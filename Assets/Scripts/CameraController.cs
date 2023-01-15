using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    private void Awake()
    {
        instance = this;
    }

    public bool showingOutside;

    public Transform player;
    public float yCameraChangePoint;
    public Vector3 cameraOutdoor, cameraIndoor;

    public float moveSpeed;
    private Vector3 targetPos;

    // Start is called before the first frame update
    void Start()
    {
        targetPos = cameraIndoor;
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null){
            if (player.position.y > yCameraChangePoint)
            {
                targetPos = cameraOutdoor;
                showingOutside = true;
            }
            else
            {
                targetPos = cameraIndoor;

                showingOutside = false;
            }

            transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
        
    }
}
