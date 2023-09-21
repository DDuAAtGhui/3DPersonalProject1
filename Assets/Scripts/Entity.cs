using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region components
    public Animator anim { get; private set; }
    public Rigidbody rb { get; private set; }
    #endregion
    #region infos
    [Header("Move Info")]
    [SerializeField] public float MoveSpeed = 10f;
    #endregion
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {

    }

    public void SetVelocityToZero() => rb.velocity = Vector3.zero;
    public void SetVelocityXZ(float X, float Z)
    {
        Vector3 normalizedVector = rb.velocity = new Vector3(X, rb.velocity.y, Z).normalized;
        rb.velocity = normalizedVector * MoveSpeed;
    }
}
