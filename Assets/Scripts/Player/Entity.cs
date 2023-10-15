using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Infos
    [Header("Entity")]
    public GameManager gameManager;
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
    public bool DetectingLedge(Vector3 moveDir, out LedgeData ledgeData, float heightStandard = 0.75f)
    {
        ledgeData = new LedgeData();

        if (moveDir == Vector3.zero)
            return false;

        //플레이어 포지션 기준으로 플레이어 움직이는 방향으로 origin 포인트 설정
        var origin = transform.position + moveDir * ledgeCheckOriginOffset + Vector3.up * ledgeCheckUpOffset;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, ledgeRayLength, Obstacle))
        {

            var surfaceRayOrigin = transform.position + transform.forward * 1.8f + Vector3.down * 0.1f;

            if (gameManager.Visible_LedgeRay)
            {
                Debug.DrawRay(origin, Vector3.down * ledgeRayLength, Color.green);

                Debug.DrawRay(transform.position, transform.forward * 1.8f, Color.red);
                Debug.DrawRay(transform.position + transform.forward * 1.8f, Vector3.down * 0.1f, Color.red);
                Debug.DrawRay(surfaceRayOrigin, -transform.forward * 1.8f, Color.red);
            }

            if (Physics.Raycast(surfaceRayOrigin, -transform.forward, out RaycastHit surfaceHit, 1.8f, Obstacle))
            {
                float height = transform.position.y - hit.point.y;

                //heightStandard 보다 높은곳이면 모서리 파쿠르 나올수있는 판정
                if (height >= heightStandard)
                {
                    //플레이어의 앞 방향과, 플레이어가 서있는 장애물의 normal 각도 
                    ledgeData.angle = Vector3.Angle(transform.forward, surfaceHit.normal);
                    ledgeData.height = height;
                    ledgeData.surfaceHit = surfaceHit;

                    if (gameManager.Visible_LedgeRay)
                    {
                        Debug.DrawRay(surfaceHit.point, surfaceHit.normal * 5f, Color.blue);
                        Debug.Log("player - ledge angle : " + ledgeData.angle);
                    }

                    return true;
                }
            }
        }
        return false;
    }
}
