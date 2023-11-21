using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float hp = 100f;
    public float chaseSpeed = 10f;
    public float damage = 10f;
    GameManager gameManager;
    Animator anim;
    public FieldOfView fov;
    public NavMeshAgent navMeshAgent;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        gameManager = GameManager.instance;
        fov = GetComponentInChildren<FieldOfView>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
    }
    private void Update()
    {
        if (hp <= 0)
        {
            anim.SetBool(gameManager.animIDisDead, true);
            anim.SetBool(gameManager.animIDisMove, false);
            anim.SetBool(gameManager.animIDisIdle, false);
        }
    }
    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            var playerBullet = collision.gameObject;

            var damage =
                playerBullet.GetComponent<RememberingParent>().parent.
                GetComponent<GunTweak>().gunData.damage;

            hp -= damage;
        }
    }
}
