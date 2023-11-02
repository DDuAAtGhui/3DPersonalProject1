using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    #region 관리 멤버
    public Player player;


    [Header("Raycast Option")]
    public float LedgeToLedgeFrontHitRayLength_forward = 2f;
    public float LedgeToLedgeFrontHitRayLength_up = 0.5f;

    // GetAxis스타일 쓰고싶으면 사용
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

    //heightHit 위치로 생성되고 파쿠르 시 오프셋 값 만큼 로컬 기준으로 이동할 프리팹(오프셋 지점)
    //파쿠르 동작동안은 남아있어야 하니까 파쿠르 끝날 때 active를 false로 돌릴것
    [HideInInspector] public GameObject StandardTargetMatchingObject;
    [HideInInspector] public GameObject CustomTargetMatchingObject;
    [HideInInspector] public GameObject HangableNetworkSphereObject;

    //타겟매칭 설정할 오브젝트 교환용으로 바꾸기전에 예전 오브젝트 보관용
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

    #region 애니메이터 파라미터 해쉬화
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
        animIDParkouring = Animator.StringToHash("Parkouring"); //파쿠르 중일땐 기본 애니메이션에서 탈출하는 용도로 사용
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
    [Tooltip("상태 진입 실행된 클래스 이름 표시")] public bool Log_StateEnter = true;
    [Tooltip("상태 Update 실행된 클래스 이름 표시")] public bool Log_StateUpdate = true;
    [Tooltip("상태 FixedUpdate 실행된 클래스 이름 표시")] public bool Log_StateFixedUpdate = true;
    [Tooltip("상태 LateUpdate 실행된 클래스 이름 표시")] public bool Log_StateLateUpdate = true;
    [Tooltip("상태 탈출 실행된 클래스 이름 표시")] public bool Log_StateExit = true;
    [Tooltip("플레이어 접촉시작 클래스 이름 표시")] public bool Log_StateOnCollisionEnter = true;
    [Tooltip("플레이어 접촉중 클래스 이름 표시")] public bool Log_StateOnCollisionStay = true;
    [Tooltip("플레이어 현재 스피드 표시")] public bool Log_PlayerSpeed = true;
    [Tooltip("플레이어의 벨로시티")] public bool Log_PlayerCurrentVelocity = true;
    [Tooltip("플레이어의 y축 속도 보존값")] public bool Log_PlayerVerticalVelocity = true;
    [Tooltip("플레이어의 전체 낙하 시간")] public bool Log_PlayerTotalFallingTime = true;
    [Tooltip("플레이어가 현재 바쁜지 표시")] public bool Log_isBusy = true;
    [Tooltip("ParkourAble에 등록된 레이어를 가진 플레이어의 장애물 탐지범위 안에 들어온 장애물과 플레이어간의 거리와 높이 표시")] public bool Log_RayInfo_Obstacle_DistanceAndHeight = true;
    [Tooltip("플레이어가 서있는 모서리 높이 표시")] public bool Log_LedgeHeight = true;
    [Tooltip("모서리의 normal기준으로 플레이어의 이동방향 각도 표시")] public bool Log_moveDirAngle_toLedget = true;
    [Tooltip("forwardHit 레이캐스트에 감지된 오브젝트의 로컬 좌표계 기준 forwardHit.point 좌표계")] public bool Log_Local_ForwardHitPoint = true;
    [Tooltip("파쿠르 타겟매칭 정보 표시")] public bool Log_TargetMatch = true;
    [Tooltip("플레이어 앞에 있는 장애물의 두께 표시")] public bool Log_ObstacleThickness = true;
    [Tooltip("ParkourAble에 등록된 레이어를 가진 플레이어의 장애물 탐지범위 안에 들어온 장애물의 이름을 표시")] public bool Log_WhatisRayHitObstacle = true;
    [Tooltip("파쿠르 스크립터블 오브젝트 에셋 참 거짓 상태 정보")] public bool Log_ParkourPossibleState = true;
    [Tooltip("파쿠르 액션 실패 원인 표시와 실행된 액션 표시")] public bool Log_ParkourActionSuccessInfo = true;
    [Tooltip("매달려있는 파쿠르의 환경정보")] public bool Log_HangingInfo = true;
    [Tooltip("애니메이션 종료 혹은 애니메이션 중 컨트롤 회복 플래그")] public bool Log_isAnimEnd = true;
    [Tooltip("파쿠르 매치 포지션 지점 가시화. 오프셋 설정시 파쿠르 할 때 오프셋 만큼 이동함")] public bool Visible_MatchPosition = true;
    [Tooltip("모서리 체크 레이캐스트 가시화")] public bool Visible_LedgeRay = true;

    #endregion

}
