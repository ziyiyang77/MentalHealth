using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bullet;

    void Start()
    {
        
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector2 pos = transform.position;
            Instantiate(bullet, pos, Quaternion.Euler(0,0,90));
        }
    }
}
