using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float FireRate = 2;
    public float Speed = 10f;
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
