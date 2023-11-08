using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float speed = 10f;
    [Tooltip("�Ѿ� ���� �ð�")][SerializeField] float LifeSpan = 2f;

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
            //�ݶ��̴� ������ �����ǿ� �����ϰ�
            //�ݶ��̴� ������ �������� �Ѿ��� �ٶ󺸴� �������� ȸ��
            Instantiate(impactEffects[0],
                collision.GetContact(0).point, 
                Quaternion.LookRotation(transform.position - collision.GetContact(0).point).normalized);
        }

        Destroy(gameObject);
    }
}
