using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float speed = 10f;
    [Tooltip("�Ѿ� ���� �ð�")][SerializeField] float LifeSpan = 2f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.velocity = transform.forward * speed;

        Destroy(gameObject, LifeSpan);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
