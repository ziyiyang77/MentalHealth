using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reef : MonoBehaviour
{
    float m_HP = 30.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_HP <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void Damage(float damage)
    {
        m_HP -= damage;
    }

    public float getHP()
    {
        return m_HP;
    }
}
