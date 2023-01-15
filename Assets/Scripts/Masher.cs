using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Masher : MonoBehaviour
{
    public static Masher instance;

    private Animator anim;

    private Animator camAnim;

    public Transform effectSpawnPoint;

    private void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();
        camAnim = Camera.main.GetComponent<Animator>();
        
    }

    public int requiredToFill;

    private bool isMashingFood = false;
    private float corpsesInMasherCount = 0;
    public float placeOffset = .2f;

    public Transform placePoint, squisher;

    private bool squishing, opening;
    public float squishSpeed, openSpeed;

    public float waitAfterSquishing = 1f;

    public PowerBarController bar;


    void Start()
    {
        bar.SetupBar(requiredToFill, 0f);
    }

    // Update is called once per frame
    void Update()
    {
       /* if(squishing)
        {
            squisher.localScale = new Vector3(Mathf.MoveTowards(squisher.localScale.x, 0f, squishSpeed * Time.deltaTime), 1f, 1f);
        } else if(opening)
        {
           /* squisher.localScale = new Vector3(Mathf.MoveTowards(squisher.localScale.x, 1f, openSpeed * Time.deltaTime), 1f, 1f);
            if(squisher.localScale.x == 1f)
            {
                opening = false;
            }
        }*/
    }

    public bool IsHungry()
    {
        return anim.GetBool("isHungry");
    }

    public void SetIsHungry(bool isHungry) {
        anim.SetBool("isHungry", isHungry);
    }

    public bool CanAddCorpse()
    {
        return !isMashingFood;
    }

    public bool AddCorpse()
    {
        if (!CanAddCorpse())
            return false;

        corpsesInMasherCount++;
        if (corpsesInMasherCount >= requiredToFill)
        {
            isMashingFood = true;
            StartCoroutine(SpawnFoodCo());
        }


        bar.UpdateBar(corpsesInMasherCount);
        return true;
    }

    IEnumerator SpawnFoodCo()
    {

        //foreach (Corpse c in corpsesInMasher)
        //{
        //    Destroy(c.gameObject);
        //}
        //corpsesInMasher.Clear();
        camAnim.SetTrigger("shake");
        anim.SetTrigger("eat");
        squishing = true;
        opening = false;

        yield return new WaitForSeconds(waitAfterSquishing);


        
        squishing = false;
        opening = true;

        FoodStorage.instance.SpawnFood();
       
        corpsesInMasherCount = 0;
        bar.UpdateBar(0f);
        isMashingFood = false;
    }
}
