using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Infos
    [Header("Entity")]
    [Header("Collision Info")]
    [SerializeField] public bool isGrounded = false;
    [SerializeField] public float GroundedOffset = -0.14f; //거친 표면 체크에 유용
    [SerializeField] public float GroundedCheckRadius = 0.28f; //그라운드 체크 구의 반지름. 캐릭터 컨트롤러와 동일해야함
    [SerializeField] public LayerMask whatIsGround;
    [SerializeField] public float CanPlayFallingAnimationDistance = 0f;
    [SerializeField] public LayerMask CanPlayFallingAimationLayer;
    [HideInInspector] public float totalFallingTime = 0f;

    [SerializeField] public LayerMask Obstacle;
    [Header("Ledge Check Info")]
    [SerializeField] public float ledgeRayLength = 10f;
    [SerializeField] public float ledgeCheckOriginOffset = 0.5f;
    [SerializeField] public float ledgeCheckUpOffset = 1f;
    [HideInInspector] public bool isOnLedge = false;
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

    //모서리 감지
    public bool DetectingLedge(Vector3 moveDir, float heightStandard = 0.75f)
    {
        if (moveDir == Vector3.zero)
            return false;

        //플레이어 포지션 기준으로 플레이어 움직이는 방향으로 origin 포인트 설정
        var origin = transform.position + moveDir * ledgeCheckOriginOffset + Vector3.up * ledgeCheckUpOffset;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, ledgeRayLength, Obstacle))
        {
            Debug.DrawRay(origin, Vector3.down * ledgeRayLength, Color.green);

            float height = transform.position.y - hit.point.y;

            //heightStandard 보다 높이가 크면 이동시켜도 안떨어지게
            if (height >= heightStandard)
            {
                return true;
            }
        }
        return false;
    }
}
