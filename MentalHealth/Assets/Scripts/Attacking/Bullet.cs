using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    GameObject target;
    // public GameObject explosion;
    public float rotationSpeed;

    Quaternion rotateToTarget;
    Vector3 direction;

    Rigidbody2D rb;

    void Start()
    {
        target = GameObject.Find("Enemy");
        rb = GetComponent<Rigidbody2D>();
        
    }

    void Update()
    {
        direction = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rotateToTarget = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotateToTarget, Time.deltaTime * rotationSpeed);        
        
        rb.velocity = new Vector2(direction.x * 2, direction.y * 2);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        // Instantiate(explosion, transform.position, Quaternion.identity);
        if (coll.CompareTag("Enemy"))
        {
            Destroy(this.gameObject);
        }
    }
}
