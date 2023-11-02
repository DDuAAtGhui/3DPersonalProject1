using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    #region ���� ���
    public Player player;


    [Header("Raycast Option")]
    public float LedgeToLedgeFrontHitRayLength_forward = 2f;
    public float LedgeToLedgeFrontHitRayLength_up = 0.5f;

    // GetAxis��Ÿ�� ��������� ���
    [Header("Optional - GetAxisStyle")]
    public Vector2 GetAxisStyle_inputXZ;
    [HideInInspector] public Vector2 current_GetAxisStyle_inputXZ;
    [SerializeField] public float _inputXZtoGetAxisStyeSmoothTime = 0.05f;
    #endregion
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);

        else
        {
            instance = this;
            DontDestroyOnLoad(instance.gameObject);
        }
    }

    private void Start()
    {
        LoadResources();
    }

    //heightHit ��ġ�� �����ǰ� ���� �� ������ �� ��ŭ ���� �������� �̵��� ������(������ ����)
    //���� ���۵����� �����־�� �ϴϱ� ���� ���� �� active�� false�� ������
    [HideInInspector] public GameObject StandardTargetMatchingObject;
    [HideInInspector] public GameObject CustomTargetMatchingObject;
    [HideInInspector] public GameObject HangableNetworkSphereObject;

    //Ÿ�ٸ�Ī ������ ������Ʈ ��ȯ������ �ٲٱ����� ���� ������Ʈ ������
    [HideInInspector] public GameObject StoredObjectForSwitching;
    private void LoadResources()
    {
        StandardTargetMatchingObject = Resources.Load("StandardTargetMatchingPosition") as GameObject;
        StandardTargetMatchingObject = Instantiate(StandardTargetMatchingObject, transform.position, Quaternion.identity);
        StandardTargetMatchingObject.SetActive(false);

        CustomTargetMatchingObject = Resources.Load("ClimbableLedgeTargetMatchingPosition") as GameObject;
        CustomTargetMatchingObject = Instantiate(CustomTargetMatchingObject, transform.position, Quaternion.identity);
        CustomTargetMatchingObject.SetActive(false);

        HangableNetworkSphereObject = Resources.Load("HangableObject") as GameObject;
        HangableNetworkSphereObject = Instantiate(HangableNetworkSphereObject, transform.position, Quaternion.identity);
        HangableNetworkSphereObject.SetActive(false);

        StoredObjectForSwitching = CustomTargetMatchingObject;
    }

    private void Update()
    {
        CursorContorl();
        animParameterToHash();
        CalculateDigitalInputToAnalog();

    }

    void CursorContorl()
    {
        if (Cursor.lockState == CursorLockMode.Locked && player._inputCurosrVisible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        else if (Cursor.lockState == CursorLockMode.None && !player._inputCurosrVisible)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private void CalculateDigitalInputToAnalog()
    {
        GetAxisStyle_inputXZ =
        Vector2.SmoothDamp(GetAxisStyle_inputXZ, player._inputXZ,
        ref current_GetAxisStyle_inputXZ, _inputXZtoGetAxisStyeSmoothTime);
    }

    #region �ִϸ����� �Ķ���� �ؽ�ȭ
    [HideInInspector] public int animIDisAiming;
    [HideInInspector] public int animIDSpeed;
    [HideInInspector] public int animIDMotionSpeed;
    [HideInInspector] public int animIDJump;
    [HideInInspector] public int animIDFreeFall;
    [HideInInspector] public int animIDLanding_Roll;
    [HideInInspector] public int animIDLanding_Small;
    [HideInInspector] public int animIDLanding_Hard;
    [HideInInspector] public int animIDGrounded;
    [HideInInspector] public int animIDLanding;
    [HideInInspector] public int animIDParkouring;
    [HideInInspector] public int animIDParkour_StepUp;
    [HideInInspector] public int animIDParkour_JumpUp;
    [HideInInspector] public int animIDParkour_CrouchToClimbUp;
    [HideInInspector] public int animIDParkour_JumpOver_Roll;
    [HideInInspector] public int animIDParkour_StandJumpingDown;
    [HideInInspector] public int animIDParkour_HangingIdle;
    [HideInInspector] public int animIDParkour_IdleToHang;
    [HideInInspector] public int animIDParkour_JumpFromHangingWall;
    [HideInInspector] public int animIDParkour_BracedHangHopUp;
    [HideInInspector] public int animIDParkour_BracedHangHopDown;
    [HideInInspector] public int animIDParkour_BracedHangHopRight;
    [HideInInspector] public int animIDParkour_BracedHangHopLeft;
    [HideInInspector] public int animIDParkour_BracedHangShimmyRight;
    [HideInInspector] public int animIDParkour_BracedHangShimmyLeft;
    [HideInInspector] public int animIDParkour_BracedHangToCrouch;

    void animParameterToHash()
    {
        animIDisAiming = Animator.StringToHash("isAiming");
        animIDSpeed = Animator.StringToHash("Speed");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("Falling");
        animIDLanding_Small = Animator.StringToHash("Landing_Small");
        animIDLanding_Roll = Animator.StringToHash("Landing_Roll");
        animIDLanding_Hard = Animator.StringToHash("Landing_Hard");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDLanding = Animator.StringToHash("isLanding");
        animIDParkouring = Animator.StringToHash("Parkouring"); //���� ���϶� �⺻ �ִϸ��̼ǿ��� Ż���ϴ� �뵵�� ���
        animIDParkour_StepUp = Animator.StringToHash("Parkour_StepUp");
        animIDParkour_JumpUp = Animator.StringToHash("Parkour_JumpUp");
        animIDParkour_CrouchToClimbUp = Animator.StringToHash("Parkour_CrouchToClimbUp");
        animIDParkour_JumpOver_Roll = Animator.StringToHash("Parkour_JumpOver_Roll");
        animIDParkour_StandJumpingDown = Animator.StringToHash("Parkour_StandJumpingDown");
        animIDParkour_HangingIdle = Animator.StringToHash("Parkour_HangingIdle");
        animIDParkour_IdleToHang = Animator.StringToHash("Parkour_IdleToHang");
        animIDParkour_JumpFromHangingWall = Animator.StringToHash("Parkour_JumpFromHangingWall");
        animIDParkour_BracedHangHopUp = Animator.StringToHash("Parkour_BracedHangHopUp");
        animIDParkour_BracedHangHopDown = Animator.StringToHash("Parkour_BracedHangHopDown");
        animIDParkour_BracedHangHopRight = Animator.StringToHash("Parkour_BracedHangHopRight");
        animIDParkour_BracedHangHopLeft = Animator.StringToHash("Parkour_BracedHangHopLeft");
        animIDParkour_BracedHangShimmyRight = Animator.StringToHash("Parkour_BracedHangShimmyRight");
        animIDParkour_BracedHangShimmyLeft = Animator.StringToHash("Parkour_BracedHangShimmyLeft");
        animIDParkour_BracedHangToCrouch = Animator.StringToHash("Parkour_BracedHangToCrouch");
    }
    #endregion
    #region DEBUG_OPTION
    [Space(15)]
    [Header("STATE DEBUG OPTION")]
    [Tooltip("���� ���� ����� Ŭ���� �̸� ǥ��")] public bool Log_StateEnter = true;
    [Tooltip("���� Update ����� Ŭ���� �̸� ǥ��")] public bool Log_StateUpdate = true;
    [Tooltip("���� FixedUpdate ����� Ŭ���� �̸� ǥ��")] public bool Log_StateFixedUpdate = true;
    [Tooltip("���� LateUpdate ����� Ŭ���� �̸� ǥ��")] public bool Log_StateLateUpdate = true;
    [Tooltip("���� Ż�� ����� Ŭ���� �̸� ǥ��")] public bool Log_StateExit = true;
    [Tooltip("�÷��̾� ���˽��� Ŭ���� �̸� ǥ��")] public bool Log_StateOnCollisionEnter = true;
    [Tooltip("�÷��̾� ������ Ŭ���� �̸� ǥ��")] public bool Log_StateOnCollisionStay = true;
    [Tooltip("�÷��̾� ���� ���ǵ� ǥ��")] public bool Log_PlayerSpeed = true;
    [Tooltip("�÷��̾��� ���ν�Ƽ")] public bool Log_PlayerCurrentVelocity = true;
    [Tooltip("�÷��̾��� y�� �ӵ� ������")] public bool Log_PlayerVerticalVelocity = true;
    [Tooltip("�÷��̾��� ��ü ���� �ð�")] public bool Log_PlayerTotalFallingTime = true;
    [Tooltip("�÷��̾ ���� �ٻ��� ǥ��")] public bool Log_isBusy = true;
    [Tooltip("ParkourAble�� ��ϵ� ���̾ ���� �÷��̾��� ��ֹ� Ž������ �ȿ� ���� ��ֹ��� �÷��̾�� �Ÿ��� ���� ǥ��")] public bool Log_RayInfo_Obstacle_DistanceAndHeight = true;
    [Tooltip("�÷��̾ ���ִ� �𼭸� ���� ǥ��")] public bool Log_LedgeHeight = true;
    [Tooltip("�𼭸��� normal�������� �÷��̾��� �̵����� ���� ǥ��")] public bool Log_moveDirAngle_toLedget = true;
    [Tooltip("forwardHit ����ĳ��Ʈ�� ������ ������Ʈ�� ���� ��ǥ�� ���� forwardHit.point ��ǥ��")] public bool Log_Local_ForwardHitPoint = true;
    [Tooltip("���� Ÿ�ٸ�Ī ���� ǥ��")] public bool Log_TargetMatch = true;
    [Tooltip("�÷��̾� �տ� �ִ� ��ֹ��� �β� ǥ��")] public bool Log_ObstacleThickness = true;
    [Tooltip("ParkourAble�� ��ϵ� ���̾ ���� �÷��̾��� ��ֹ� Ž������ �ȿ� ���� ��ֹ��� �̸��� ǥ��")] public bool Log_WhatisRayHitObstacle = true;
    [Tooltip("���� ��ũ���ͺ� ������Ʈ ���� �� ���� ���� ����")] public bool Log_ParkourPossibleState = true;
    [Tooltip("���� �׼� ���� ���� ǥ�ÿ� ����� �׼� ǥ��")] public bool Log_ParkourActionSuccessInfo = true;
    [Tooltip("�Ŵ޷��ִ� ������ ȯ������")] public bool Log_HangingInfo = true;
    [Tooltip("�ִϸ��̼� ���� Ȥ�� �ִϸ��̼� �� ��Ʈ�� ȸ�� �÷���")] public bool Log_isAnimEnd = true;
    [Tooltip("���� ��ġ ������ ���� ����ȭ. ������ ������ ���� �� �� ������ ��ŭ �̵���")] public bool Visible_MatchPosition = true;
    [Tooltip("�𼭸� üũ ����ĳ��Ʈ ����ȭ")] public bool Visible_LedgeRay = true;

    #endregion

}
