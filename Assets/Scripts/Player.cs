using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IAttackable, IHealable
{
    public Food foodToSpawn;
    public Transform foodSpitPoint;
    public bool invulnerable = false;
    public float speed;
    public float homeSpeed = 4;

    public float corpsePickupRange;
    public float soldierCommandRange = 2;
    public LayerMask whatIsCorpse;
    public LayerMask whatIsSoldier;

    private const int maxCorpseCarryCount = 5;
    private int costOfNewfood = 200;
    private int curCorpseCarryCount = 0;
    private Transform[] carryPoints = new Transform[5];

    private bool canPlaceInMasher;

    private Animator anim;

    private PlayerTrail trail;

    public float maxHealth = 60;
    public float startingHealth = 40;
    public float health;
    public Slider hpSlider;
    public GameObject healthDisplayCanvas;
    private float showHealthCanvas;

    public GameObject masherEffect;
    private Animator camAnim;
    public GameObject king;
    public GameObject currentAttacker;
    public int outdoorY = -2;
    public int healSpeed = 4;


    UnityEngine.KeyCode healCommandKey = KeyCode.H;
    UnityEngine.KeyCode feedCommandKey = KeyCode.F;
    UnityEngine.KeyCode attackTheKingKey = KeyCode.R;

    Collider2D[] results = new Collider2D[100];

    private void Start()
    {
        health = startingHealth;
        anim = GetComponent<Animator>();
        trail = GetComponent<PlayerTrail>();
        camAnim = Camera.main.GetComponent<Animator>();
        // carryPoints = transform.Find("CarryPoints").GetComponentsInChildren<Transform>();

        Transform carryPointBase = transform.Find("CarryPoints");
        int i = 0;
        foreach (Transform child in carryPointBase.transform)
        {
            carryPoints[i] = child;
            i++;
        }
    }

    void IAttackable.TakeDamage (float damage) 
    {
        if (invulnerable) { return; }
        health -= damage;

        if (health > 0) 
        {
            StartCoroutine(DamageEffectCo());
        }
    }

    void IHealable.Heal() 
    {
        if(health >= maxHealth) { health = maxHealth; return; }
        health += healSpeed * Time.deltaTime;
    }

    void Update()
    {
        if(health <= 0){ 
        
            GM.instance.CallGameOver();
            camAnim.SetTrigger("shake");
            Destroy(gameObject);
        }
        // showHealthCanvas -= Time.deltaTime;

        if (GM.instance.HasGameStarted()) {
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
            if(transform.position.y < CameraController.instance.yCameraChangePoint)
            {
                transform.position += input.normalized * homeSpeed * Time.deltaTime;

            }
            else
            {
                transform.position += input.normalized * speed * Time.deltaTime;
            }

            //Debug.Log(input);
            if (input != Vector3.zero)
            {
                trail.enabled = true;
                anim.SetBool("isRunning", true);
            }
            else
            {
                trail.enabled = false;
                anim.SetBool("isRunning", false);
            }

            if (input != Vector3.zero)
            {
                float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            }
        }


        //Collider2D nearbySoldierColl = Physics2D.OverlapCircleNonAlloc(transform.position, soldierCommandRange, whatIsSoldier);
        var hits = Physics2D.OverlapCircleNonAlloc(transform.position, soldierCommandRange, results, whatIsSoldier);
        bool hurtSoldierNearby = false;
        bool larvaNearby = false;
        bool isOutdoors = transform.position.y > outdoorY;

        // attempt to generate new food
        if (Input.GetKeyDown(feedCommandKey))
        {
            if (GM.instance.CanSpendExtraHunger(costOfNewfood)) {
                GM.instance.SpendExtraHunger(costOfNewfood);
                Food newFood = Instantiate(foodToSpawn, foodSpitPoint.position, foodSpitPoint.rotation);
            } else { 
                UIController.instance.shakeHungerBar = true;
            }
            //nearbySoldier.Feed();
        }

        for (int i = 0; i < hits; i++)
        {
            var nearbySoldierColl = results[i];
            if (nearbySoldierColl != null)
            {
                Soldier nearbySoldier = nearbySoldierColl.GetComponent<Soldier>();
                hurtSoldierNearby = (nearbySoldier.isHurt() && nearbySoldier.state != Soldier.State.Healing);

                if (hurtSoldierNearby && Input.GetKeyDown(healCommandKey))
                {
                    nearbySoldier.GoHeal();
                }

                
                // attack the King
                if (Input.GetKeyDown(attackTheKingKey) && king != null) {
                    nearbySoldier.AttackTarget(king.gameObject.transform);
                }

                // protect the Queen
                if (Input.GetKeyDown(attackTheKingKey) && currentAttacker != null) {
                    nearbySoldier.AttackTarget(currentAttacker.gameObject.transform);
                }

                if (nearbySoldier.state == Soldier.State.Larva)
                {
                    larvaNearby = true;
                    // if (Input.GetKeyDown(feedCommandKey))
                    // {
                    //     GM.instance.SpendExtraHunger(300);
                    //     Food newFood = Instantiate(foodToSpawn, transform.position + transform.forward * 5f, transform.rotation);
                    //     //nearbySoldier.Feed();
                    // }
                }
            }
            
            UIController.instance.feedLarvaMessage.SetActive(larvaNearby && !isOutdoors);
            UIController.instance.commandSoldierToHealMessage.SetActive(hurtSoldierNearby);
        }
        

        //check for corpses to pick up
        if (curCorpseCarryCount < maxCorpseCarryCount)
        {
            Collider2D foundCorpse = Physics2D.OverlapCircle(transform.position, corpsePickupRange, whatIsCorpse);

            if (foundCorpse != null)
            {

                PickupOneCorpse(foundCorpse.transform);
                foundCorpse.GetComponent<Corpse>().Pickup();
                // Destroy(foundCorpse.gameObject);
            }
            else
            {
                UIController.instance.pickupCorpseMessage.SetActive(false);
            }
        }

        if (curCorpseCarryCount > 0 && canPlaceInMasher)
        {
            if (Masher.instance.AddCorpse())
            {
                camAnim.SetTrigger("shake");
                Instantiate(masherEffect, Masher.instance.effectSpawnPoint.position, Quaternion.identity);
                DeleteOneCorpse();
            }
        }

        if (canPlaceInMasher)
        {
            bool b = (curCorpseCarryCount != 0 && Masher.instance.CanAddCorpse());
            UIController.instance.placeInMasherMessage.SetActive(b);
        }
    }



    private void PickupOneCorpse(Transform startTransform)
    {
        carryPoints[curCorpseCarryCount].transform.position = startTransform.position;
        carryPoints[curCorpseCarryCount].transform.rotation = startTransform.rotation;
        curCorpseCarryCount += 1;

        if (!Masher.instance.IsHungry()) {
            Masher.instance.SetIsHungry(true);
        }

        if (curCorpseCarryCount > maxCorpseCarryCount)
            curCorpseCarryCount = maxCorpseCarryCount;
        UpdateCorpseVisuals();
        UIController.instance.UpdateCorpseUICount(curCorpseCarryCount);
        UIController.instance.SetCorpseUIActive(true);
    }

    private void DeleteOneCorpse()
    {
        curCorpseCarryCount -= 1;
        if (curCorpseCarryCount <= 0) {
            curCorpseCarryCount = 0;
            Masher.instance.SetIsHungry(false);
            UIController.instance.SetCorpseUIActive(false);
        }

        UpdateCorpseVisuals();
    }

    public void SetCurrentAttacker(GameObject attacker) {
        currentAttacker = attacker;
    }

    private void UpdateCorpseVisuals()
    {
        for (int i = 0; i < maxCorpseCarryCount; i++)
        {
            var a = i < curCorpseCarryCount;
            carryPoints[i].gameObject.SetActive(a);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Masher")
        {
            if (curCorpseCarryCount > 0)
            {
                canPlaceInMasher = true;
                UIController.instance.placeInMasherMessage.SetActive(true);
            }
        }

        if (other.CompareTag("Enemy"))
        {
            camAnim.SetTrigger("shake");

            Enemy e = other.GetComponent<Enemy>();

            
            if (invulnerable) 
            {
                //other.GetComponent<Enemy>().Death();
                return;
            }

            if (e.isFinalBoss || e.isKnight) {
                return;
            } else {
                healthDisplayCanvas.SetActive(true);
                showHealthCanvas = 4f;
                GM.instance.RestoreHunger(e.damage);
                other.GetComponent<Enemy>().Death();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Masher")
        {
            canPlaceInMasher = false;
            UIController.instance.placeInMasherMessage.SetActive(false);
        }
    }

    IEnumerator DamageEffectCo()
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        invulnerable = true;
        foreach (SpriteRenderer renderer in spriteRenderers) {
            renderer.enabled = false;
            }
        yield return new WaitForSeconds(.1f);
        foreach (SpriteRenderer renderer in spriteRenderers) {
            renderer.enabled = true;
            }
       yield return new WaitForSeconds(.1f);
        foreach (SpriteRenderer renderer in spriteRenderers) {
            renderer.enabled = false;
            }
        yield return new WaitForSeconds(.1f);
        foreach (SpriteRenderer renderer in spriteRenderers) {
            renderer.enabled = true;
            }
        yield return new WaitForSeconds(.1f);
        foreach (SpriteRenderer renderer in spriteRenderers) {
            renderer.enabled = false;
            }
        yield return new WaitForSeconds(.1f);
        foreach (SpriteRenderer renderer in spriteRenderers) {
            renderer.enabled = true;
            }
        invulnerable = false;

        yield return null;
    } 
}