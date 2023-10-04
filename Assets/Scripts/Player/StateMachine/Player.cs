using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    public bool isBusy { get; private set; }
    #region components
    [HideInInspector] public CharacterController CC;
    [HideInInspector] public PlayerInputSystem playerInput;
    [HideInInspector] public Animator anim;

    #endregion
    #region infos
    [Header("Move Info")]
    [SerializeField] public float moveSpeed = 3f;
    [SerializeField] public float RotatonSmoothTime = 0.12f;
    [SerializeField] public float SpeedChangeRate = 10.0f;

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
    [SerializeField] public float mass = 1f;
    [SerializeField] public float jumpTimeout = 0.5f; //다시 점프 할 수 있을때까지 걸리는 시간
    [SerializeField] public float FallTimeout = 0.15f; // fall스테이트 진입까지 걸리는시간 (계단에 유용)

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
    [HideInInspector] public Vector3 inputDirection;

    // GetAxis스타일 쓰고싶으면 사용
    [HideInInspector] public Vector2 GetAxisStyle_inputXZ;
    [HideInInspector] public Vector2 current_GetAxisStyle_inputXZ;
    [SerializeField] public float _inputXZtoGetAxisStyeSmoothTime = 0.05f;
    #endregion

    #region animaionIDsForHash
    [HideInInspector] public int animIDSpeed;
    [HideInInspector] public int animIDMotionSpeed;
    [HideInInspector] public int animIDJump;
    [HideInInspector] public int animIDGrounded;
    [HideInInspector] public int animIDFreeFall;
    #endregion
    #region 상태들, 객체선언, 인풋시스템 콜백
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerWalkState walkState { get; private set; }
    public PlayerRunState runState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayeFallingState fallingState { get; private set; }
    private void Awake()
    {

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine);
        walkState = new PlayerWalkState(this, stateMachine);
        runState = new PlayerRunState(this, stateMachine);
        jumpState = new PlayerJumpState(this, stateMachine);
        fallingState = new PlayeFallingState(this, stateMachine);
        #region 컴포넌트
        CC = GetComponent<CharacterController>();
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
        animationToHash();
    }

    public override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();

        CursorContorl();
        CameraControl();
        GroundCheck();
        CalculateDigitalInputToAnalog();
        //DebugLog();
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

        anim.SetBool(animIDGrounded, isGrounded);
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
    void animationToHash()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
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
        if (isBusy)
            Debug.Log("플레이어가 바쁜상태");

        yield return new WaitForSeconds(seconds);

        if (isBusy)
            Debug.Log("플레이어가 여유로운 상태");
        isBusy = false;
    }
    #endregion

    #region 인풋액션 - 액션들
    void onMoveAction(InputAction.CallbackContext context)
    {
        _inputXZ = context.ReadValue<Vector2>();
    }

    void onWalkAction(InputAction.CallbackContext context)
    {
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
    [Tooltip("플레이어가 현재 바쁜지 표시")] public bool Log_isBusy = true;

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
}
