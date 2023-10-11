using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    [HideInInspector] public bool isBusy { get; private set; }
    #region components
    [HideInInspector] public CharacterController CC;
    [HideInInspector] public PlayerInputSystem playerInput;
    [HideInInspector] public Animator anim;

    #endregion
    #region infos
    //개별 Collision
    [HideInInspector] public bool PlayFallingAnimation = false;

    [Header("Parkour Info")]
    public List<ParkourAction> parkourActions;
    [HideInInspector] public ParkourAbleObstacleHitData hitData;
    [SerializeField] Vector3 forwardRayOffset = new Vector3(0, 0.25f, 0); //맨 처음 플레이어 앞으로 Ray 발사
    [SerializeField] float forwardRayLength = 0.5f;
    [SerializeField] float heightRayLength = 100f;
    [SerializeField] float backSideRayLength = 100f;
    [SerializeField] public LayerMask ParkourAble;
    [HideInInspector][SerializeField] public float distanceToObstacle;
    [HideInInspector][SerializeField] public float heightToObstacle;
    [HideInInspector][SerializeField] public float thicknessOfObstacle;
    [HideInInspector][SerializeField] public int parkourActionIndex = 0;


    [Header("Move Info")]
    [SerializeField] public float moveSpeed = 3f;
    [HideInInspector] public float temp_moveSpeed;
    [SerializeField] public float RotatonSmoothTime = 0.12f;
    [SerializeField] public float SpeedChangeRate = 10.0f;
    [HideInInspector] public bool Can_MoveHorizontally = true;
    [HideInInspector] public bool Can_Rotate = true;
    [HideInInspector] public bool horizontalStop;

    //[HideInInspector] public float walkSpeed;

    //[HideInInspector] public float targetRotation = 0f;
    //[HideInInspector] public float speed; // 애니메이션 블렌드용
    //[HideInInspector] public float animationBlend; // 애니메이션 블렌드용
    //[HideInInspector] public float verticalVelocity;
    //[HideInInspector] public float terminalVelocity = 160f; //종말속도
    //[HideInInspector] public float rotationVelocity;

    [Header("Jump And Gravity")]
    [SerializeField] public float jumpHeight = 12f;
    [SerializeField] public float Gravity = -9.81f;
    [SerializeField] public float groundedGravity = -1f;
    [SerializeField] public float mass = 1f;
    [Tooltip("다시 점프 할 수 있을때까지 걸리는 시간")]
    [SerializeField] public float jumpTimeout = 0.5f;
    [Tooltip("fall스테이트 진입까지 걸리는시간 (계단에 유용)")]
    [SerializeField] public float FallTimeout = 0.15f;
    [Tooltip("공중 상태 진입 했을 때 점프 입력 할 수 있는 유예시간")][SerializeField] public float CoyoteTime = 0.1f;

    [Header("Camera Info")]
    [SerializeField] public CinemachineFreeLook VCamera;
    [SerializeField] public Transform orientation; //플레이어 원점
    [HideInInspector] public Vector3 LookDir;


    [Header("Input Info")]
    public Vector2 _inputXZ; // WASD
    public bool _inputWalk; // Left Shift
    public bool _inputCurosrVisible; // Left Alt
    public bool _inputJump; // Space
    public bool ApplyAnalogMovement = false;
    public bool isControlable = true;
    [HideInInspector] public Vector3 inputDirection;

    // GetAxis스타일 쓰고싶으면 사용
    [HideInInspector] public Vector2 GetAxisStyle_inputXZ;
    [HideInInspector] public Vector2 current_GetAxisStyle_inputXZ;
    [SerializeField] public float _inputXZtoGetAxisStyeSmoothTime = 0.05f;
    #endregion

    #region 애니메이터 파라미터 해쉬화
    [HideInInspector] public int animIDSpeed;
    [HideInInspector] public int animIDMotionSpeed;
    [HideInInspector] public int animIDJump;
    [HideInInspector] public int animIDFreeFall;
    [HideInInspector] public int animIDLanding_Roll;
    [HideInInspector] public int animIDLanding_Small;
    [HideInInspector] public int animIDLanding_Hard;
    [HideInInspector] public int animIDGrounded;
    [HideInInspector] public int animIDParkouring;
    [HideInInspector] public int animIDParkour_StepUp;
    [HideInInspector] public int animIDParkour_JumpUp;
    #endregion
    #region 상태들, 객체선언, 인풋시스템 콜백
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerWalkState walkState { get; private set; }
    public PlayerRunState runState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayeFallingState fallingState { get; private set; }
    public PlayerLandingState landingState { get; private set; }
    public PlayerParkourState parkourState { get; private set; }
    public PlayerStepUpState stepUpState { get; private set; }
    public PlayerJumpUpState jumpUpState { get; private set; }
    private void Awake()
    {

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine);
        walkState = new PlayerWalkState(this, stateMachine);
        runState = new PlayerRunState(this, stateMachine);
        jumpState = new PlayerJumpState(this, stateMachine);
        fallingState = new PlayeFallingState(this, stateMachine);
        landingState = new PlayerLandingState(this, stateMachine);
        parkourState = new PlayerParkourState(this, stateMachine);
        stepUpState = new PlayerStepUpState(this, stateMachine);
        jumpUpState = new PlayerJumpUpState(this, stateMachine);
        #region 컴포넌트
        CC = GetComponentInChildren<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        #endregion
        #region 인풋시스템 콜백관련
        playerInput = new PlayerInputSystem();

        //인스턴스 생성
        //started 콜백에 onMoveAction 메소드 등록
        playerInput.CharacterControls.Move.started += onMoveAction;
        //조이스틱은 스틱으로 컨트롤하니까 perform필수임
        //근데 키보드도 결국 4방향만 할거 아니면 perform하는게 좋음
        //performed은 0과 1사이의 값을 콜백받음
        playerInput.CharacterControls.Move.performed += onMoveAction;
        //키가 떼졌는지를 감지하는 콜백
        playerInput.CharacterControls.Move.canceled += onMoveAction;

        playerInput.CharacterControls.Walk.started += onWalkAction;
        playerInput.CharacterControls.Walk.canceled += onWalkAction;


        playerInput.CharacterControls.CurosrVisible.performed += onCurosrVisible;
        playerInput.CharacterControls.CurosrVisible.canceled += onCurosrVisible;

        playerInput.CharacterControls.Jump.started += onJumpAction;
        playerInput.CharacterControls.Jump.canceled += onJumpAction;

        #endregion
    }
    #endregion
    public override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
        animParameterToHash();

        temp_moveSpeed = moveSpeed;
    }

    public override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        CursorContorl();
        CameraControl();
        GroundCheck();
        ParkourAbleObstacleCheck();
        CalculateDigitalInputToAnalog();
        //DebugLog();

        if (Log_PlayerCurrentVelocity)
            Debug.Log("Velocity : " + CC.velocity);

        if (Log_RayInfo_Obstacle_DistanceAndHeight)
            Debug.Log($"장애물 거리 : {distanceToObstacle}, 장애물의 높이 : {heightToObstacle} ");

        if (Log_WhatisRayHitObstacle)
            if (hitData.forwardHit.transform != null)
                Debug.Log($"감지된 장애물 이름 : {hitData.forwardHit.transform.name}");

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        stateMachine.currentState.FixedUpdate();
    }
    public void LateUpdate()
    {
        stateMachine.currentState.LateUpdate();
    }

    #region 구체적 상태들에서 쓸 모노비헤이비어 기능 메소드들
    public void OnCollisionEnter(Collision collision)
    {
        stateMachine.currentState.OnCollisionEnter(collision);
    }

    public void OnCollisionStay(Collision collision)
    {
        stateMachine.currentState.OnCollisionStay(collision);
    }
    #endregion

    #region 메소드들
    void CameraControl()
    {
        //카메라가 플레이어를 바라보는 벡터값(y축은 플레이어와 동일하게 한 코드)
        LookDir = transform.position - new Vector3(VCamera.transform.position.x, transform.position.y, VCamera.transform.position.z);
        orientation.forward = LookDir.normalized;

        if (_inputXZ != Vector2.zero)
        {
            //움직일때 리센터링 시간 계산하는거 제거(이거 없으면 움직일땐 리센터링 안되도 WaitTime이 계산됨
            VCamera.m_RecenterToTargetHeading.CancelRecentering();
            VCamera.m_RecenterToTargetHeading.m_enabled = false;
        }

        else
            VCamera.m_RecenterToTargetHeading.m_enabled = true;

    }

    void GroundCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x,
            transform.position.y - GroundedOffset, transform.position.z);

        isGrounded = Physics.CheckSphere(spherePosition, GroundedCheckRadius, whatIsGround);

        PlayFallingAnimation = !Physics.Raycast(transform.position, Vector3.down, CanPlayFallingAnimationDistance, CanPlayFallingAimationLayer);

        anim.SetBool(animIDGrounded, isGrounded);
    }
    public ParkourAbleObstacleHitData ParkourAbleObstacleCheck()
    {
        hitData = new ParkourAbleObstacleHitData();

        hitData.forwardHitFound = Physics.Raycast(transform.position + forwardRayOffset, transform.forward, out hitData.forwardHit,
            forwardRayLength, ParkourAble);

        Debug.DrawRay(transform.position + forwardRayOffset, transform.forward * forwardRayLength, hitData.forwardHitFound ? Color.green : Color.gray);

        if (hitData.forwardHitFound)
        {
            //위에서 아래로 발사하는 방식
            hitData.heightHitFound = Physics.Raycast(hitData.forwardHit.point + Vector3.up * heightRayLength, Vector3.down, out hitData.heighHit, heightRayLength, ParkourAble);

            Debug.DrawRay(hitData.forwardHit.point + Vector3.up * heightRayLength, Vector3.down * heightRayLength, hitData.heightHitFound ? Color.green : Color.gray);

            distanceToObstacle = hitData.forwardHit.distance;
            heightToObstacle = hitData.heighHit.point.y - transform.position.y; ////플레이어 - 장애물간의 정확한 높이차

            ////두께 검사
            //Debug.DrawRay(hitData.forwardHit.point, -hitData.forwardHit.normal * backSideRayLength, Color.red);
        }

        else
        {
            distanceToObstacle = 0;
            heightToObstacle = 0;
        }

        return hitData;
    }

    void CursorContorl()
    {
        if (Cursor.lockState == CursorLockMode.Locked && _inputCurosrVisible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        else if (Cursor.lockState == CursorLockMode.None && !_inputCurosrVisible)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    void animParameterToHash()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("Falling");
        animIDLanding_Small = Animator.StringToHash("Landing_Small");
        animIDLanding_Roll = Animator.StringToHash("Landing_Roll");
        animIDLanding_Hard = Animator.StringToHash("Landing_Hard");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDParkouring = Animator.StringToHash("Parkouring"); //파쿠르 중일땐 기본 애니메이션에서 탈출하는 용도로 사용
        animIDParkour_StepUp = Animator.StringToHash("Parkour_StepUp");
        animIDParkour_JumpUp = Animator.StringToHash("Parkour_JumpUp");
    }

    //상태머신 활용형
    public void PerformParkourState(params PlayerStates[] parkourStates)
    {
        parkourActionIndex = 0;


        parkourActions.ForEach(action =>
        {
            if (Log_ParkourPossibleState)
                Debug.Log($"Num {parkourActionIndex} : " + action.CheckIfPossible());

            //인덱스 초과 예외처리
            //State를 여러개 갖고있고, 여러개 변수를 넣을때 인덱스로 따로 구분 안해주면 ture 걸릴때 무조건 첫번째로 파라미터에 넣은 액션이 나오기떄문에
            //params로 무한히 받고 배열 인덱스로 따로 관리해줌
            if (parkourActionIndex < parkourStates.Length && action.CheckIfPossible())
            {
                //타겟매칭 활성화 하면 실행
                if (action.EnableTargetMatching)
                {
                    MatchTarget(action);

                    Debug.Log($"==================\n" +
                        $"MatchTarget Info\n" +
                        $"anim.targetPosition : {anim.targetPosition} \n " +
                        $"action.MatchPosition : {action.MatchPosition} \n " +
                        $"action.MatchBodyPart : {action.MatchBodyPart}\n" +
                        $"action.MatchStartTime : {action.MatchStartTime}\n" +
                        $"action.MatchTargetTime : {action.MatchTargetTime}");
                }
                stateMachine.ChangeState(parkourStates[parkourActionIndex]);

            }
            parkourActionIndex++;
        });
    }
    public void MatchTarget(ParkourAction action)
    {
        //이미 타겟 매칭중이면 쓸데없는 실행 X
        if (anim.isMatchingTarget)
            return;

        //MatchTargetWeightMask : y축만 매칭하고싶으니까 0,1,0
        anim.MatchTarget(action.MatchPosition + Vector3.up * 5, transform.rotation, action.MatchBodyPart,
            new MatchTargetWeightMask(Vector3.up, 0), action.MatchStartTime, action.MatchTargetTime);




    }

    private void CalculateDigitalInputToAnalog()
    {
        GetAxisStyle_inputXZ =
        Vector2.SmoothDamp(GetAxisStyle_inputXZ, _inputXZ,
        ref current_GetAxisStyle_inputXZ, _inputXZtoGetAxisStyeSmoothTime);
    }
    public IEnumerator nowBusy(float seconds)
    {
        isBusy = true;
        if (isBusy && Log_isBusy)
            Debug.Log("플레이어가 바쁜상태");

        yield return new WaitForSeconds(seconds);

        if (!isBusy && Log_isBusy)
            Debug.Log("플레이어가 여유로운 상태");
        isBusy = false;
    }
    public void isHorizontalStop(bool horizontalStop) => this.horizontalStop = horizontalStop;
    public void SetControllable(bool isControlable)
    {
        //파쿠르할때 콜라이더 걸려서 안올라가는거 방지
        this.isControlable = isControlable;
        CC.enabled = isControlable;
    }
    public void isAnimEnd() => stateMachine.currentState.isAnimEnd = true;
    #endregion

    #region 인풋액션 - 액션들
    void onMoveAction(InputAction.CallbackContext context)
    {
        _inputXZ = context.ReadValue<Vector2>();
    }

    void onWalkAction(InputAction.CallbackContext context)
    {
        //땅에 붙어있을때만 입력가능, LandingState Exit쪽에 walk = false 설정해놨음
        //walk 누른채로 점프한담에 착지 전에 떼면 walk가 유지되는 문제있어서 설정해둔것
        if (isGrounded)
            _inputWalk = context.ReadValueAsButton();
    }
    void onJumpAction(InputAction.CallbackContext context)
    {
        _inputJump = context.ReadValueAsButton();
    }
    void onCurosrVisible(InputAction.CallbackContext context)
    {
        _inputCurosrVisible = context.ReadValueAsButton();
    }

    private void OnDrawGizmos()
    {
        #region 장애물 탐색 레이캐스트
        //Gizmos.DrawLine(transform.position + forwardRayOffset, transform.forward * forwardRayLength);
        #endregion
    }
    #endregion

    #region 캐릭터 컨트롤러 쓸 때 항상 해줘야하는거
    //인풋시스템 쓸 때 항상 키고 끄고 해줘야함
    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        playerInput.CharacterControls.Disable();

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
    [Tooltip("플레이어가 현재 바쁜지 표시")] public bool Log_isBusy = true;
    [Tooltip("ParkourAble에 등록된 레이어를 가진 플레이어의 장애물 탐지범위 안에 들어온 장애물과 플레이어간의 거리와 높이 표시")] public bool Log_RayInfo_Obstacle_DistanceAndHeight = true;
    [Tooltip("플레이어 앞에 있는 장애물의 두께 표시")] public bool Log_ObstacleThickness = true;
    [Tooltip("ParkourAble에 등록된 레이어를 가진 플레이어의 장애물 탐지범위 안에 들어온 장애물의 이름을 표시")] public bool Log_WhatisRayHitObstacle = true;
    [Tooltip("파쿠르 스크립터블 오브젝트 에셋 참 거짓 상태 정보")] public bool Log_ParkourPossibleState = true;
    [Tooltip("애니메이션 종료 혹은 애니메이션 중 컨트롤 회복 플래그")] public bool Log_isAnimEnd = true;
    //[Space(10)]
    //[Header("MEMBER DEBUG OPTION")]
    //public bool DEBUG_targetRotaion = false;
    //public bool DEBUG_VCamera_transform_eulerAngles_y = false;
    //public bool DEBUG_inputDirection = false;
    //public bool DEBUG_LookDir = false;
    //public bool DEBUG_rotationVelocity = false;
    //public bool DEBUG_GetAxisStyle_inputXZ = false;
    //public bool DEBUG_verticalVelocity = false;
    //void DebugLog()
    //{
    //    if (DEBUG_targetRotaion)
    //        Debug.Log("targetRotation : " + targetRotation);

    //    if (DEBUG_VCamera_transform_eulerAngles_y)
    //        Debug.Log("VCamera.transform.eulerAngles.y : " + VCamera.transform.eulerAngles.y);

    //    if (DEBUG_inputDirection)
    //        Debug.Log("inputDirection : " + inputDirection);

    //    if (DEBUG_LookDir)
    //        Debug.Log("LookDir : " + LookDir);

    //    if (DEBUG_rotationVelocity)
    //        Debug.Log("rotationVelocity : " + rotationVelocity);

    //    if (DEBUG_GetAxisStyle_inputXZ)
    //        Debug.Log("GetAxisStyle_inputXZ : " + GetAxisStyle_inputXZ);

    //    if (DEBUG_verticalVelocity)
    //        Debug.Log("verticalVelocity : " + verticalVelocity);
    //}

    #endregion

    //#region 연습용
    //bool inAction;
    //IEnumerator DoParkourAction(ParkourAction action)
    //{
    //    inAction = true;
    //    SetControllable(false);

    //    anim.CrossFade(action.AniName, 0.2f);
    //    yield return null;

    //    var animState = anim.GetNextAnimatorStateInfo(0);

    //    if (!animState.IsName(action.AniName))
    //        Debug.LogError("The parkour animation is wrong!");

    //    yield return new WaitForSeconds(animState.length);

    //    float timer = 0f;

    //    while (timer <= animState.length)
    //    {
    //        timer += Time.deltaTime;

    //        // 파쿠르동안 오브젝트 방향으로 회전하기
    //        if (action.RotateToObstacle)
    //            Quaternion.RotateTowards(transform.rotation, action.TargetRotation, 회전보정속도);

    //        if (action.EnableTargetMatching)
    //            MatchTarget(action);

    //        yield return null;
    //    }

    //    SetControllable(true);
    //    inAction = false;
    //}

    //#endregion
}
