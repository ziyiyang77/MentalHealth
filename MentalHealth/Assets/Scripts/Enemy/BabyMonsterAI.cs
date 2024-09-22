using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyMonsterAI : MonoBehaviour
{
    //  motion state
    public enum MotionState
    {
        Idle,
        Attack,
        Retreat,
        Chase
    }

    MotionState m_state;

    Transform m_player;
    private Rigidbody2D m_rb;

    //  Idle State
    bool m_isActivated = false;

    //  Attack state
    public float detectionRange = 10f; // Range to detect the player
    public float attackRange = 5f;     // Range to attack the player
    public float retreatDistance = 0.5f; // Distance to retreat before attacking
    public float speed = 40f;            // Speed of the fish
    public float angleVariation = 45f;  // Variation in angles while attacking
    public float retreatTime = 0.5f;
    private float retreatTimer;

    public float chasingSpeed = 50.0f;


    // shaking effect
    float m_shakingSpeed = 1.0f; //how fast it shakes
    float m_shakingAmount = 1.0f; //how much it shakes



    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").transform;
        m_state = MotionState.Idle;
        m_rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, m_player.position);

        this.getState(distanceToPlayer);

         RotateToFacePlayer();

    }

    public void getState(float distanceToPlayer)
    {
        Debug.Log("Distance to Player: " + distanceToPlayer + " | Current State: " + m_state);

        switch (m_state)
        {
            case MotionState.Idle:

                if (distanceToPlayer <= detectionRange)
                {
                    m_state = MotionState.Attack;
                }

                break;
            
            case MotionState.Attack:
                if (distanceToPlayer <= attackRange)
                {
                    AttackPlayer();
                }
                else if (distanceToPlayer > detectionRange)
                {
                    m_state = MotionState.Idle;
                }
                break;
            
            case MotionState.Retreat:
                RetreatFromPlayer();

                break;

            case MotionState.Chase:
                Debug.Log("Chasing mode.");
                if (distanceToPlayer >= attackRange + 0.1f  && distanceToPlayer <= detectionRange)
                {
                    ChasingPlayer();
                }
                else if (distanceToPlayer <= attackRange)
                {
                    m_state = MotionState.Attack;
                }
                else if (distanceToPlayer > detectionRange + 0.1f)
                {
                    m_state = MotionState.Idle;
                }

                break;
        }

    }

    private void RotateToFacePlayer()
    {
        Vector2 directionToPlayer = m_player.position - transform.position;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void AttackPlayer()
    {
        Vector2 directionToPlayer = (m_player.position - transform.position).normalized;
        Vector2 newPosition = (Vector2)transform.position + directionToPlayer * speed * Time.deltaTime;

        m_rb.MovePosition(newPosition);

        // If the enemy reaches close to the player, switch to retreat state
        if (Vector2.Distance(transform.position, m_player.position) <= retreatDistance)
        {
            m_state = MotionState.Retreat;
            retreatTimer = retreatTime;
        }
        else if (Vector2.Distance(transform.position, m_player.position) > attackRange &&
                Vector2.Distance(transform.position, m_player.position) <= detectionRange)
        {
            m_state = MotionState.Chase;
        }
        else if (Vector2.Distance(transform.position, m_player.position) > detectionRange)
        {
            m_state = MotionState.Idle;
        }
    }

    private void RetreatFromPlayer()
    {
        // Retreat a little bit
        Vector2 directionToPlayer = (transform.position - m_player.position).normalized;
        Vector2 retreatPosition = (Vector2)transform.position + directionToPlayer * retreatDistance;

        m_rb.MovePosition(Vector2.MoveTowards(transform.position, retreatPosition, speed * Time.deltaTime));

        // Countdown the retreat timer
        retreatTimer -= Time.deltaTime;

        // Once retreating is done, switch back to attacking if the player is still in range
        if (retreatTimer <= 0)
        {
            if (Vector2.Distance(transform.position, m_player.position) <= attackRange)
            {
                m_state = MotionState.Attack;  
            }
            else if (Vector2.Distance(transform.position, m_player.position) > attackRange + 0.1f &&
                Vector2.Distance(transform.position, m_player.position) <= detectionRange)
            {
                m_state = MotionState.Chase;
            }
            else
            {
                m_state = MotionState.Idle;
            }
             
        }
    }

    private void ChasingPlayer()
    {
        Debug.Log("Chasing Player.");
        Vector2 directionToPlayer = (m_player.position - transform.position).normalized;
        Vector2 newPosition = (Vector2)transform.position + directionToPlayer * chasingSpeed * Time.deltaTime;

        m_rb.MovePosition(newPosition);


    }

}
