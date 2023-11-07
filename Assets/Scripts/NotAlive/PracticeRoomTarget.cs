using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeRoomTarget : MonoBehaviour, InterDamageable
{
    [SerializeField] public float health = 100f;
    [HideInInspector] public float initialHealth;
    public bool isDead;
    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
            isDead = true;
    }
    private void Awake()
    {
        initialHealth = health;
    }
    private void Update()
    {
        //ǥ���� �����ؼ� �Ѿ
        if (isDead)
            gameObject.transform.rotation = Quaternion.Euler(Mathf.Lerp(transform.rotation.eulerAngles.x, 90f, 17 * Time.deltaTime),
                transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        else
            gameObject.transform.rotation = Quaternion.Euler(Mathf.Lerp(transform.rotation.eulerAngles.x, 0f, 17 * Time.deltaTime),
    transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

    }
}
