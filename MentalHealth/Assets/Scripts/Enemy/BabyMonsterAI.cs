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
        Retreat
    }

    MotionState m_state;

    GameObject m_player;
    private Rigidbody2D m_rb;

    //  Idle State
    public float m_detectRange = 10.0f;
    bool m_isActivated = false;

    // shaking effect
    float m_shakingSpeed = 1.0f; //how fast it shakes
    float m_shakingAmount = 1.0f; //how much it shakes



    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_state = MotionState.Idle;
        m_rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        this.getState();


    }

    public void getState()
    {
        switch(m_state)
        {
            case MotionState.Idle:
                CheckTarget();
                break;
            
            case MotionState.Attack:
                Attack();
                break;
            
            case MotionState.Retreat:
                break;
        }
    }

    public void CheckTarget()
    {
        if (!m_isActivated)
        {
            if (Vector2.Distance(m_player.transform.position, transform.position) < m_detectRange)
            {
                m_isActivated = true;
                m_state = MotionState.Attack;
            }
        }
    }

    void Attack()
    {
        Vector2 pos = transform.position;
        pos.x = Mathf.Sin(Time.time * m_shakingSpeed) * m_shakingAmount;
        
    }
}
