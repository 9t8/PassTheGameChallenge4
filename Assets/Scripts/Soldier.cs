using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;

public class Soldier : MonoBehaviour, IAttackable, IHealable
{

    public bool dontStartAsLarva = false;

    Transform outdoorStartPos;
    Transform corpseRoom;
    Transform foodRoom;
    Transform healRoom;

    AIDestinationSetter ai;
    AIPath path;

    public bool isOutdoors;

    private float minX, maxX, minY, maxY;
    public float outdoorY;

    public float speed;
    Vector3 randomPosition;

    public float visionRadius;
    public float checkVisionTime;
    float nextCheckVision;
    public LayerMask visionLayer;

    Transform enemyTarget;

    public enum State { Indoor, Patrol, Chase, Attack, Carry, Larva, Healing, Follow }
    public State state;

    public float damage;
    public float timeBetweenAttacks;
    float nextAttackTime;

    float startSpeed;
    public float larvaScale = 0.3f;

    public Transform carryPoint;

    Corpse corpse;

    public float health = 10;
    private float startHealth;
    public float healSpeed = 2;
    public float healRange = 0.3f;
    public float foodSearchRadiusLarva = 2f;

    public PowerBarController healthBar;

    public GameObject deathEffect, attackEffect;

    public GameObject effect;
    private Animator camAnim;
    public Transform bodyArt;

    void Start()
    {
        startSpeed = speed;
        path = GetComponent<AIPath>();
        ai = GetComponent<AIDestinationSetter>();
        outdoorStartPos = GameObject.FindGameObjectWithTag("Outdoor").transform;
        corpseRoom = GameObject.FindGameObjectWithTag("CorpseRoom").transform;
        foodRoom = GameObject.FindGameObjectWithTag("FoodRoom").transform;
        healRoom = GameObject.FindGameObjectWithTag("HealRoom").transform;

        startHealth = health;
        healthBar.SetupBar(health, health);
        healthBar.gameObject.SetActive(false);

        if (!dontStartAsLarva)
        {
            state = State.Larva;
            ai.target = foodRoom;
            bodyArt.localScale = larvaScale * Vector3.one;
        }
        

        minX = OutdoorArea.instance.minPatrolPoint.position.x;
        minY = OutdoorArea.instance.minPatrolPoint.position.y;
        maxX = OutdoorArea.instance.maxPatrolPoint.position.x;
        maxY = OutdoorArea.instance.maxPatrolPoint.position.y;

        Instantiate(effect, transform.position, Quaternion.identity);
        camAnim = Camera.main.GetComponent<Animator>();
        camAnim.SetTrigger("shake");
        
    }

    void Update()
    {
        if (state == State.Indoor && isOutdoors == false)
        {
            ai.target = outdoorStartPos;
        }

        if (state == State.Larva || state == State.Follow)
        {
            if (FindObjectOfType<Player>())
            {
                ai.target = FindObjectOfType<Player>().transform;
            }
            Collider2D[] potentialFood = Physics2D.OverlapCircleAll(transform.position, foodSearchRadiusLarva);
            foreach (Collider2D food in potentialFood)
            {
                if (food.CompareTag("Food"))
                {
                    ai.target = food.transform;
                }
            }

            // GameObject food = GameObject.FindGameObjectWithTag("Food");
            // if (food == null)
            //     ai.target = foodRoom;
            // else
            //     ai.target = food.transform;

        }

        if (state == State.Healing)
        {
            ai.target = healRoom;
            path.canMove = true;
            isOutdoors = false;

            if (Vector3.Distance(transform.position, healRoom.transform.position) < healRange)
            {
                if (health >= startHealth)
                {
                    health = startHealth;
                    healthBar.gameObject.SetActive(false);
                    state = State.Indoor;
                }
                healthBar.UpdateBar(health);
            }
        }

        if (transform.position.y > outdoorY && isOutdoors == false && state != State.Carry && state != State.Healing && state != State.Larva)
        {
            isOutdoors = true;
            ai.target = null;
            path.canMove = false;
            state = State.Patrol;
            speed = startSpeed;
        }

        if (state == State.Carry)
        {
            ai.target = corpseRoom;
            path.canMove = true;
            isOutdoors = false;
        }

        if (state == State.Patrol)
        {
            Patrol();
            if (Time.time > nextCheckVision)
            {
                nextCheckVision = Time.time + checkVisionTime;
                CheckVision();
            }
        }

        if (state == State.Chase && enemyTarget != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget.position, speed * Time.deltaTime);
            Vector3 displacement = transform.position - enemyTarget.position;
            float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
        }

        if (state == State.Attack && enemyTarget != null)
        {
            if (Time.time > nextAttackTime)
            {
                nextAttackTime = Time.time + timeBetweenAttacks;
                Attack();

                if (attackEffect != null)
                {
                    Instantiate(attackEffect, enemyTarget.position + new Vector3(Random.Range(-.2f, .2f), Random.Range(-.2f, .2f), 0f), Quaternion.identity);
                }
            }
        }

        if (state == State.Chase || state == State.Attack)
        {
            if (enemyTarget == null)
            {
                state = State.Patrol;
                speed = startSpeed;
            }
        }

        if (state == State.Carry)
        {
            if (corpse == null)
            {
                state = State.Indoor;
            }
            else if (Vector2.Distance(transform.position, corpseRoom.position) < 0.3f)
            {
                corpse.transform.parent = null;
                state = State.Indoor;
                corpseRoom.GetComponent<CorpseRoomPoint>().MoveToRandomPoint();
                

                corpse.PlaceForPickup(true);

                corpse = null;
            }
        }
    }

    public bool isHurt()
    {
        return !Mathf.Approximately(startHealth, health);
    }

    public void Carry(Corpse carry)
    {
        state = State.Carry;
        corpse = carry;
        carry.transform.SetParent(carryPoint);
        //carry.transform.localScale = carry.transform.localScale / 2;
    }

    void Attack()
    {
        Enemy e = enemyTarget.GetComponent<Enemy>();
        transform.position = Vector2.MoveTowards(transform.position, enemyTarget.position, speed * Time.deltaTime);
        e.TakeDamage(damage, this);
    }

    void CheckVision()
    {
        if (state == State.Patrol)
        {
            Collider2D col = Physics2D.OverlapCircle(transform.position, visionRadius, visionLayer);
            if (col != null)
            {
                state = State.Chase;
                enemyTarget = col.transform;
            }
        }
    }

    public void GoHeal()
    {
        if (state == State.Carry && corpse != null)
        {
            corpse.transform.parent = null;
            corpse.PlaceForPickup(false);
            corpse = null;
        }
        state = State.Healing;
        speed = startSpeed;
    }

    void Patrol()
    {
        if (Vector2.Distance(transform.position, randomPosition) < 0.2f)
        {
            randomPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
        } else
        {
            transform.position = Vector2.MoveTowards(transform.position, randomPosition, speed * Time.deltaTime);
            Vector3 displacement = transform.position - randomPosition;
            float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + 90 , Vector3.forward);
        }
    }

    public void Feed()
    {
        bodyArt.localScale = Vector3.one;
        healthBar.gameObject.SetActive(true);
        state = State.Indoor;
    }

    void IAttackable.TakeDamage(float damage)
    {
        health -= damage;

        if(health <= 0)
        {
            health = 0;

            if(deathEffect != null)
            {
                Instantiate(deathEffect, transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }
        healthBar.gameObject.SetActive(true);
        healthBar.UpdateBar(health);
    }

    void IHealable.Heal() {
        health += healSpeed * Time.deltaTime;
    }

    public void AttackTarget(Transform target) {
        enemyTarget = target;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy" && state != State.Carry && state != State.Healing && state != State.Larva)
        {
            if(other.GetComponent<Enemy>() == null) { return; }
            enemyTarget = other.transform;
            speed = 0f;
            other.GetComponent<Enemy>().speed = 0;
            state = State.Attack;
        }

        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<Enemy>().ResetSpeed();
        }
    }

}
