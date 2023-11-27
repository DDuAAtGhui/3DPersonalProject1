using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PracticeRoomTarget : MonoBehaviour, InterDamageable
{
    [SerializeField] public float health = 100f;
    [SerializeField] Image hpbar;
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
        hpbar.fillAmount = health / 100f;
        //표지판 보간해서 넘어감
        if (isDead)
            gameObject.transform.rotation = Quaternion.Euler(Mathf.Lerp(transform.rotation.eulerAngles.x, 90f, 17 * Time.deltaTime),
                transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        else
            gameObject.transform.rotation = Quaternion.Euler(Mathf.Lerp(transform.rotation.eulerAngles.x, 0f, 17 * Time.deltaTime),
    transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

    }
}
