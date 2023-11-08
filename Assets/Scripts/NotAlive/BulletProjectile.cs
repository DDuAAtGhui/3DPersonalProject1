using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float speed = 10f;
    [Tooltip("총알 생존 시간")][SerializeField] float LifeSpan = 2f;

    [SerializeField] List<GameObject> impactEffects = new List<GameObject>();

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
        var damageAble = collision.gameObject.GetComponent<InterDamageable>();

        damageAble?.TakeDamage(GetComponent<RememberingParent>().parent.GetComponent<GunTweak>().gunData.damage);


        if (impactEffects != null)
        {
            //콜라이더 접촉한 포지션에 생성하고
            //콜라이더 접촉한 포지션이 총알을 바라보는 방향으로 회전
            Instantiate(impactEffects[0],
                collision.GetContact(0).point, 
                Quaternion.LookRotation(transform.position - collision.GetContact(0).point).normalized);
        }

        Destroy(gameObject);
    }
}
