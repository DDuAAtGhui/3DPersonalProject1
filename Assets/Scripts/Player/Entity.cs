using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Infos
    [Header("Collision Info")]
    [SerializeField] public bool isGrounded = false;
    [SerializeField] public float GroundedOffset = -0.14f; //��ģ ǥ�� üũ�� ����
    [SerializeField] public float GroundedCheckRadius = 0.28f; //�׶��� üũ ���� ������. ĳ���� ��Ʈ�ѷ��� �����ؾ���
    [SerializeField] public LayerMask whatIsGround;
    #endregion
    public virtual void Start()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {

    }
}
