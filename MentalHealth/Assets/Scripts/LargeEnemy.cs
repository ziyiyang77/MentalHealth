using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeEnemy : MonoBehaviour
{
    //  attacking state
    public float m_attack = 10.0f;
    float waitForAttack = 2.0f;
    float attackTimer = 0.0f;
    public float attackSpeed = 10.0f;
    bool isRecharging = false;

    //  floating when findking target
    public float m_walkingSpeed = 2.0f;
    float floatingTime = 5.0f;
    public float floatingAmplitude = 0.5f; // How high and low the object floats in the y-axis
    public float floatingFrequency = 0.5f; // Speed of the floating effect
    
    private Vector2 direction; // Current movement direction (left or right)
    private float walkTime; // Randomly selected time to walk in the current direction
    private float walkTimer; // Timer to track how long the object has walked
    private Vector2 startPos; // Initial position for y floating effect

    //  check for target
    bool isAttackingPlatform = false;
    bool isLifting = false;
    public float liftTime = 4.0f; 
    public float liftHeight = 9.0f;
    public float liftSpeed = 5.0f;
    bool isRushing = false;
    Vector2 liftTargetPosition;

    public float m_attackReefDist = 3.0f;
    Reef m_target = null;

    //  info
    float m_HP;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        startPos = transform.position;
        direction = Vector2.left;
        walkTime = Random.Range(3.0f, 6.0f);

    }

    // Update is called once per frame
    void Update()
    {
        if (!isAttackingPlatform)
        {
            if (!CheckForPlatform())
            {
                //Debug.Log("floating");
                Floating();
            }
            else
            {
                Debug.Log("atacking");

                isLifting = true;
            }
        }
      

        if (m_target && isAttackingPlatform)
        {
            if (m_target.getHP() > 0.0f && !isRecharging)
            {
                if (isLifting)
                {
                    Debug.Log("Lifting");
                    liftTargetPosition = new Vector2(transform.position.x, transform.position.y + liftHeight);
                    LiftUp();
                }
                else if (isRushing)
                {
                    RushTowardsTarget();
                }
            }
            else
            {
                isAttackingPlatform = false;

            }

        }

        if (isLifting)
        {
            attackTimer += Time.deltaTime;
        }
       

        if (isRecharging)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > 2.0f)
            {
                isRecharging = false;
                attackTimer = 0.0f;
            }
        }

        
    }

    void Floating()
    {
        transform.Translate(direction * m_walkingSpeed * Time.deltaTime);

        Vector2 pos = transform.position;
        pos.y = startPos.y + Mathf.Sin(Time.time * floatingFrequency) * floatingAmplitude;
        transform.position = pos;

        walkTimer += Time.deltaTime;

        if (walkTimer >= walkTime)
        {
            ReverseDirection();
        }
    }

    void ReverseDirection()
    {
        direction = -direction;
        walkTime = Random.Range(3.0f, 6.0f);
        walkTimer = 0f; 
    }

    bool CheckForPlatform()
    {
        if (!isAttackingPlatform)
        {
            GameObject[] reefs = GameObject.FindGameObjectsWithTag("Reef");

            foreach (GameObject reef in reefs)
            {
                if (Vector2.Distance(transform.position, reef.transform.position) < m_attackReefDist)
                {
                    isAttackingPlatform = true;
                    m_target = reef.GetComponent<Reef>();
                    return true;
                }
            }
        }
        return false;

    }
    
    void LiftUp()
    {
        transform.position = Vector2.MoveTowards(transform.position, liftTargetPosition, liftSpeed * Time.deltaTime);
        if (attackTimer >= liftTime)
        {
            isLifting = false;
            attackTimer = 0.0f;
            StartCoroutine(WaitBeforeAttack());
        }
    }

    IEnumerator WaitBeforeAttack()
    {
        yield return new WaitForSeconds(1.0f);
        isRushing = true; // Start rushing
    }

    void RushTowardsTarget()
    {
        Vector2 direction = (m_target.transform.position - transform.position).normalized;
        rb.velocity = direction * attackSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == m_target.gameObject && isAttackingPlatform)
        {
            m_target.Damage(m_attack);

            isRecharging = true;

            isRushing = false;

            rb.velocity *= 0.5f; // Reduce speed after the collision
        }
    }


}
