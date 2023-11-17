using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombie : MonoBehaviour
{
    public float hp = 100f;
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (hp <= 0)
            anim.SetBool("isDead", true);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("PlayerBullet"))
        {
            var playerBullet = collision.gameObject;

            var damage = 
                playerBullet.GetComponent<RememberingParent>().parent.
                GetComponent<GunTweak>().gunData.damage;

            hp -= damage;
        }
    }
}
