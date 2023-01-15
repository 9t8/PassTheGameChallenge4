using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealRoom : MonoBehaviour
{
    public static HealRoom instance;
    private Animator anim;


     private List<Collider2D> colliders = new List<Collider2D>();
     public List<Collider2D> GetColliders () { return colliders; }
 


    private void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();        
    }

    public bool IsHealing()
    {
        return anim.GetBool("isHealing");
    }

    public void SetIsHungry(bool isHungry) {
        anim.SetBool("isHungry", isHungry);
    }

    // Update is called once per frame
    void Update()
    {
        if (colliders.Count > 0 && !anim.GetBool("isHealing")) {
            anim.SetBool("isHealing", true);
        } else if (colliders.Count <= 0) {
            anim.SetBool("isHealing", false);
        }

        if (colliders.Count > 0) {
            foreach (Collider2D spider in colliders) {    
                spider.gameObject.GetComponent<IHealable>().Heal();
            }
        }
    }

    private void OnTriggerEnter2D (Collider2D other) {
        //Debug.Log("HIT HERE");
         if (!colliders.Contains(other)) { colliders.Add(other); }
     }
 
     private void OnTriggerExit2D (Collider2D other) {
         colliders.Remove(other);
     }
}
