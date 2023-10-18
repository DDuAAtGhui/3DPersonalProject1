using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Infos
    [Header("Entity")]
    public GameManager gameManager;
    [Header("Collision Info")]
    [SerializeField] public bool isGrounded = false;
    [SerializeField] public float GroundedOffset = -0.14f; //��ģ ǥ�� üũ�� ����
    [SerializeField] public float GroundedCheckRadius = 0.28f; //�׶��� üũ ���� ������. ĳ���� ��Ʈ�ѷ��� �����ؾ���
    [SerializeField] public LayerMask whatIsGround;
    [SerializeField] public float CanPlayFallingAnimationDistance = 0f;
    [SerializeField] public LayerMask CanPlayFallingAimationLayer;
    [HideInInspector] public float totalFallingTime = 0f;

    [SerializeField] public LayerMask Obstacle;
    [Header("Ledge Check Info")]
    [SerializeField] public float ledgeRayLength = 10f;
    [SerializeField] public float ledgeCheckOriginOffset = 0.5f;
    [SerializeField] public float ledgeCheckPlayerLRFootOffset = 0.34f;
    [SerializeField] public float ledgeCheckUpOffset = 1f;
    // �� ��ġ ������ �𼭸��� �ƴѰɷ� ����
    [SerializeField] public float ledgeCheckHeightStandard_Top = 1.5f;
    [SerializeField] public float ledgeCheckHeightStandard_Bottom = 0.3f;
    [HideInInspector] public bool isOnLedge = false;

    [Header("Climb Check Info")]
    [SerializeField] public float hangableLedgeRayLength = 3f;
    [SerializeField] public int hangableRayEA = 15;
    [SerializeField] public Vector3 hangableRayInterval = new Vector3(0, 0.18f, 0);
    [SerializeField] public Vector3 hangableRayOffset = new Vector3(0, 1.8f, 0);
    [Tooltip("������Ʈ �Ʒ� �𼭸����� �� ��ġ��ŭ�� y������ ������������ �Ʒ��� ����ĳ��Ʈ �߻�")][SerializeField] float hangableLedgeThickness = 0.4f;
    [SerializeField] public LayerMask hangableLayer;
    [HideInInspector] public bool isHangable = false;
    [HideInInspector] public HangableData hangableData { get; set; }

    #endregion
    public virtual void Start()
    {
    }

    public virtual void Update()
    {
        try
        {
            isHangable = DetectingHangableLedge(transform.forward
    , out HangableData hangableLedgeHit, hangableLedgeThickness);

            hangableData = hangableLedgeHit;

            if (gameManager.Visible_MatchPosition)
                gameManager.CustomTargetMatchingPosition.SetActive(true);

            else
                gameManager.CustomTargetMatchingPosition.SetActive(false);
        }

        catch (Exception e)
        {
            Debug.Log(e);
        }
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

    //�𼭸� ����
    public bool DetectingLedge(Vector3 moveDir, out LedgeData ledgeData, float heightStandard_Top = 0.75f, float heightStandard_Bottom = 0.5f)
    {
        ledgeData = new LedgeData();

        if (moveDir == Vector3.zero)
            return false;

        //�÷��̾� ������ �������� �÷��̾� �����̴� �������� origin ����Ʈ ����
        var origin = transform.position + moveDir * ledgeCheckOriginOffset + Vector3.up * ledgeCheckUpOffset;

        if (PhysicsUtil.ThreeRayCasts(origin, Vector3.down, ledgeCheckPlayerLRFootOffset,
            transform, out List<RaycastHit> hits, ledgeRayLength, Obstacle, gameManager.Visible_LedgeRay))
        {

            //����Ʈ���� height => ���� ����(Where ���)�� �����ϴ� ��ҵ� ��ȯ
            //��, ���� ���̰� heightStandard ū ��ҵ��� ��ȯ��.
            //LINQ �Լ����� �⺻������ IEnumerable Ÿ������ ���� ��ȯ��.
            //List�� ����Ұ��̴� .ToList()�� ��ȯ������
            var validHits = hits.Where(y => transform.position.y - y.point.y > heightStandard_Bottom
            && transform.position.y - y.point.y < heightStandard_Top).ToList();

            if (validHits.Count > 0)
            {
                //ù��°�� Ledge������ ����ĳ��Ʈ �ٴڿ� point ��ǥ ����
                var surfaceRayOrigin = validHits[0].point;
                //y����ǥ �÷��̾�� ���� �Ʒ��ΰ��� ����
                surfaceRayOrigin.y = transform.position.y - 0.2f;

                if (gameManager.Visible_LedgeRay)
                    Debug.DrawLine(surfaceRayOrigin, transform.position, Color.cyan);

                //surfaceRayOrigin�� transform.position�� �ٶ󺸴� ����
                if (Physics.Raycast(surfaceRayOrigin, transform.position - surfaceRayOrigin, out RaycastHit surfaceHit, 1.8f, Obstacle))
                {
                    float height = transform.position.y - validHits[0].point.y;

                    //�÷��̾��� �� �����, �÷��̾ ���ִ� ��ֹ��� normal ���� 
                    ledgeData.angle = Vector3.Angle(transform.forward, surfaceHit.normal);
                    ledgeData.height = height;
                    ledgeData.LedgeHit = surfaceHit;

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

    /// <summary>
    /// �÷��̾��� �տ� �Ŵ޸� �� �ִ� ������Ʈ �ִ��� üũ.
    /// 
    /// </summary>
    /// <param name="dir">�÷��̾��� �� ����</param>
    /// <param name="hangableLedgeHit">Ŭ���̹� �ϴ� ������Ʈ�� ���� �𼭸�</param>
    /// <returns></returns>
    [SerializeField] Vector3 HangablePositionSnapshot;
    public bool DetectingHangableLedge(Vector3 dir, out HangableData hangableLedgeHit, float hangableLedgeThickness = 0.4f)
    {
        hangableLedgeHit = new HangableData();

        if (dir == Vector3.zero)
            return false;

        var origin = transform.position + hangableRayOffset;

        var interval = hangableRayInterval;

        for (int i = 0; i < hangableRayEA; i++)
        {
            Debug.DrawRay(origin + interval * i, dir * hangableLedgeRayLength);

            hangableLedgeHit.hitFound = Physics.Raycast(origin + interval * i, dir, out RaycastHit hit, hangableLedgeRayLength, hangableLayer);

            if (hangableLedgeHit.hitFound)
            {
                var upper_origin = hit.point + Vector3.up * hangableLedgeThickness;

                Physics.Raycast(upper_origin, Vector3.down, out RaycastHit topHit, hangableLedgeThickness * 2f, hangableLayer);
                Debug.DrawRay(upper_origin, Vector3.down * hangableLedgeThickness * 2f, Color.red);

                hangableLedgeHit.HangableHit = topHit;
                hangableLedgeHit.height = topHit.transform.position.y - transform.position.y;

                //for (int j = 0; j < 5; j++)
                //{
                //    Physics.Raycast(upper_origin + 0.01f * dir * j, Vector3.down, out RaycastHit topHit, hangableLedgeThickness * 2f, hangableLayer);
                //    Debug.DrawRay(upper_origin + 0.01f * dir * j, Vector3.down * hangableLedgeThickness * 2f, Color.red);

                //    hangableLedgeHit.height = topHit.transform.position.y - transform.position.y;
                //    hangableLedgeHit.HangableHit = topHit;
                //}

                return true;
            }

        }
        return false;
    }
}
