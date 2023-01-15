using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool isFinalBoss = false;
    public bool isKnight = false;

    private float minX, maxX, minY, maxY;
    Vector3 randomPosition;
    public float speed;
    public float turnSpeed = 360.0f;
    public float health;
    public Corpse corpse;

    float startSpeed;

    public PowerBarController healthBar;

    public float attackRange;
    public LayerMask attackables;
    public float distanceToAttack;

    private GameObject target;

    public float damage, timeBetweenAttacks, attackDamage = 2;
    private float attackCounter;

    public GameObject attackEffect, deathEffect;

    private float slowDownTimer;
    Collider2D[] newTarget;
    //public bool isFat;

    void Start()
    {
        startSpeed = speed;

        if (healthBar) 
        {
            healthBar.SetupBar(health, health);

            if (!isFinalBoss)
            {
                healthBar.gameObject.SetActive(false);
            }
        }


        minX = OutdoorArea.instance.minPatrolPoint.position.x;
        minY = OutdoorArea.instance.minPatrolPoint.position.y;
        maxX = OutdoorArea.instance.maxPatrolPoint.position.x;
        maxY = OutdoorArea.instance.maxPatrolPoint.position.y;
    }

    void Update()
    {

        if(slowDownTimer > 0)
        { 
            slowDownTimer -= Time.deltaTime;
            speed = 0.75f;
        } 
        else
        {
            ResetSpeed();
        }

        if (target == null)
        {
            Patrol();

            //check for soldiers nearby
            newTarget = Physics2D.OverlapCircleAll(transform.position, attackRange, attackables);
            foreach (Collider2D potentialTarget in newTarget)
            {
                //reset attackCounter
                attackCounter = 0;

                if(potentialTarget != null)
                {
                    target = potentialTarget.gameObject;
                }
                if (target.GetComponent<Player>())
                {
                    break;
                }
            }

            if(target == null)
            {
                soldiersAndPlayer[] attackables = FindObjectsOfType<soldiersAndPlayer>();
                foreach (soldiersAndPlayer entity in attackables)
                {
                    if (Vector2.Distance(entity.transform.position, transform.position) < attackRange)
                    {
                        attackCounter = 0;
                        target = entity.gameObject;
                        if (target.GetComponent<Player>())
                        {
                            break;
                        }
                    }
                }
            }
        } 
        else
        {

            //face target
            FacePoint(target.transform.position);

            //have a target so chase them if still outside
            if (target.transform.position.y > CameraController.instance.yCameraChangePoint)
            {
                if(Vector3.Distance(transform.position, target.transform.position) > distanceToAttack)
                {
                    //move towards target
                    transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

                    attackCounter -= Time.deltaTime;
                }
                else
                {
                    //attack target
                    attackCounter -= Time.deltaTime;
                    if(attackCounter <= 0)
                    {

                        attackCounter = timeBetweenAttacks;

                        target.GetComponent<IAttackable>().TakeDamage(attackDamage);

                        if (target.GetComponent<Player>() != null) 
                        {
                            target.GetComponent<Player>().SetCurrentAttacker(this.gameObject);
                        }

                        if (attackEffect != null)
                        {
                            Instantiate(attackEffect, target.transform.position + new Vector3(Random.Range(-.2f, .2f), Random.Range(-.2f, .2f), 0f), Quaternion.identity);
                        }

                        if (target.GetComponent<Soldier>())
                        {
                            if(target.GetComponent<Soldier>().health <= 0)
                            {
                                target = null;
                            }
                        }
                    }
                }
            } 
            else
            {
                //target is indoors so stop chasing them
                target = null;
            }
        }

        if(isKnight)
        {
            Collider2D potentialLarva = Physics2D.OverlapCircle(transform.position, 0.35f, attackables);
            if (potentialLarva) 
            { 
                if (potentialLarva.GetComponent<Soldier>()) { if(potentialLarva.GetComponent<Soldier>().state == Soldier.State.Larva) { EatLarva(potentialLarva); target = null; } }
                if (potentialLarva.GetComponent<Player>()) { potentialLarva.GetComponent<IAttackable>().TakeDamage(attackDamage); }
            }
        }
    }

    public void ResetSpeed()
    {
        speed = startSpeed;
    }

    void Patrol()
    {
        if (Vector2.Distance(transform.position, randomPosition) < 0.2f)
        {
            randomPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, randomPosition, speed * Time.deltaTime);
            FacePoint(randomPosition);
            //Vector3 displacement = transform.position - randomPosition;
            //float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        }
    }

    void FacePoint(Vector3 pos)
    {
        if (isFinalBoss) 
        {
            return;
        }
        Vector3 displacement = transform.position - pos;
        float goalAngle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg + 90;
        Quaternion targetRotation = Quaternion.AngleAxis(goalAngle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    public void TakeDamage(float damage, Soldier soldier)
    {
        health -= damage;
        if (health <= 0)
        {
            Corpse g = Instantiate(corpse, transform.position, Quaternion.identity);
            soldier.Carry(g);
            Destroy(gameObject);

            health = 0;

            if(deathEffect != null)
            {
                Instantiate(deathEffect, transform.position, transform.rotation);
            }
        }

        target = soldier.gameObject;
        healthBar.UpdateBar(health);
        healthBar.gameObject.SetActive(true);
    }

    public void Death()
    {
        
        Corpse g = Instantiate(corpse, transform.position, Quaternion.identity);

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
        }
        health = 0;

        if (isFinalBoss) 
        {
            Debug.Log("WIN");
            GM.instance.CallGameWon();
        }

        Destroy(gameObject);
    }

	private void OnTriggerEnter2D(Collider2D other)
	{
        if (other.CompareTag("Player") && (isFinalBoss || isKnight)) 
        {
            target = other.gameObject;
        }

        if (other.CompareTag("Player") && speed > 0 && (!isFinalBoss && !isKnight))
        {
            if (other.GetComponent<Soldier>())
            {
                if(other.GetComponent<Soldier>().state == Soldier.State.Larva)
                {
                    EatLarva(other);
                    return;
                }
            }
            other.GetComponent<IAttackable>().TakeDamage(attackDamage);
            Corpse g = Instantiate(corpse, transform.position, Quaternion.identity);
            Death();
        }
        
    }

    void EatLarva(Collider2D other)
    {
        Instantiate(attackEffect, other.transform.position, Quaternion.identity);
        Destroy(other.gameObject);
    }

	private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
