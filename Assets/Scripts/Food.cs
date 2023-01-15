using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float hungerToFill;

    public float speed;

    public GameObject effect;
    private Animator camAnim;

    [HideInInspector]
    public Vector3 targetPos;

    private void Start()
    {

        camAnim = Camera.main.GetComponent<Animator>();

        if(targetPos == Vector3.zero)
        {
            targetPos = transform.position;
        }
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            Soldier s = other.GetComponent<Soldier>();
            if (s != null)
            {
                if (s.state == Soldier.State.Larva)
                {
                    s.Feed();
                    Instantiate(effect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
            }
            else
            {
                camAnim.SetTrigger("shake");
                Instantiate(effect, transform.position, Quaternion.identity);
                GM.instance.RestoreHunger(hungerToFill);
                Destroy(gameObject);
            }
        }
    }
}
