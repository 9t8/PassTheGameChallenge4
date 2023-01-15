using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MonoBehaviour
{
    public Collider2D myCol;

    private bool isCarried;
    public bool inStorage = false;

    public float snapSpeed;

    public bool isVisualOnly = false;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        if (isVisualOnly)
        {
            startPosition = transform.localPosition;
            startRotation = transform.localRotation;
            myCol.enabled = false;
            isCarried = true;
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (inStorage)
            GM.instance.AddCorpse(this);
    }

    public void PlaceForPickup(bool placeInRoom)
    {
        myCol.enabled = true;
        isCarried = false;
        transform.SetParent(null);
        if (placeInRoom)
        {
            inStorage = true;
            GM.instance.AddCorpse(this);
        }
    }

    public void Pickup()
    {
        if (inStorage)
            GM.instance.RemoveCorpse(this);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (isCarried)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, snapSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, startRotation, snapSpeed * Time.deltaTime);
        }
    }
}
