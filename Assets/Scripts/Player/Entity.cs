using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Infos
    [Header("Collision Info")]
    [SerializeField] public bool isGrounded = false;
    [SerializeField] public float GroundedOffset = -0.14f; //거친 표면 체크에 유용
    [SerializeField] public float GroundedCheckRadius = 0.28f; //그라운드 체크 구의 반지름. 캐릭터 컨트롤러와 동일해야함
    [SerializeField] public LayerMask whatIsGround;
    [SerializeField] public float CanPlayFallingAnimationDistance = 0f;
    [SerializeField] public LayerMask CanPlayFallingAimationLayer;
    [HideInInspector] public float totalFallingTime = 0f;

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedCheckRadius);

        Gizmos.color = new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.3f);
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - CanPlayFallingAnimationDistance, transform.position.z));
    }
}
