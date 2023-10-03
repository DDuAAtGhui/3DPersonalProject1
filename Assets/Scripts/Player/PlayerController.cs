using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region components
    CharacterController CC;
    PlayerInputSystem playerInput;
    Animator anim;

    #endregion
    #region infos
    [Header("Move Info")]
    [SerializeField] float moveSpeed = 3f;
    float walkSpeed;
    [SerializeField] float RotatonSmoothTime = 0.12f;
    [SerializeField] float SpeedChangeRate = 10.0f;

    float targetRotation = 0f;
    float speed; // 애니메이션 블렌드용
    float animationBlend; // 애니메이션 블렌드용
    float verticalVelocity;
    float terminalVelocity = 160f; //종말속도
    float rotationVelocity;

    [Header("Jump And Gravity")]
    [SerializeField] float jumpHeight = 1.2f;
    [SerializeField] float Gravity = -9.81f;
    [SerializeField] float mass = 1f;
    [SerializeField] float jumpTimeout = 0.5f; //다시 점프 할 수 있을때까지 걸리는 시간
    [SerializeField] float FallTimeout = 0.15f; // fall스테이트 진입까지 걸리는시간 (계단에 유용)

    float jumpTimeoutDelta;
    float fallTimeoutDelta;


    [Header("Collision Info")]
    [SerializeField] bool Grounded = false;
    [SerializeField] float GroundedOffset = -0.14f; //거친 표면 체크에 유용
    [SerializeField] float GroundedCheckRadius = 0.28f; //그라운드 체크 구의 반지름. 캐릭터 컨트롤러와 동일해야함
    [SerializeField] LayerMask whatIsGround;


    [Header("Camera Info")]
    [SerializeField] CinemachineFreeLook VCamera;
    Vector3 LookDir;
    [SerializeField] Transform orientation;

    [Header("Input Info")]
    public Vector2 _inputXZ; // WASD
    public bool _inputWalk; // Left Shift
    public bool _inputCurosrVisible; // Left Alt
    public bool _inputJump; // Space
    public bool ApplyAnalogMovement = false;
    Vector3 inputDirection;

    // GetAxis스타일 쓰고싶으면 사용
    Vector2 GetAxisStyle_inputXZ;
    Vector2 current_GetAxisStyle_inputXZ;
    [SerializeField]
    float _inputXZtoGetAxisStyeSmoothTime = 0.05f;
    #endregion

    #region animaionIDsForHash
    int animIDSpeed;
    int animIDMotionSpeed;
    int animIDJump;
    int animIDGrounded;
    int animIDFreeFall;
    #endregion
    private void Awake()
    {
        CC = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        //FreeLookAxis = Vcam_FreeLook.transform.GetChild(0);

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
    }

    private void Start()
    {
        animationToHash();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        CursorContorl();
        CameraControl();
        CalculateDigitalInputToAnalog();
        GroundCheck();

        Move();
        JumpAndGravity();

        DebugLog();
    }

    private void CalculateDigitalInputToAnalog()
    {
        GetAxisStyle_inputXZ =
        Vector2.SmoothDamp(GetAxisStyle_inputXZ, _inputXZ,
        ref current_GetAxisStyle_inputXZ, _inputXZtoGetAxisStyeSmoothTime);
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
    void GroundCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x,
            transform.position.y - GroundedOffset, transform.position.z);

        Grounded = Physics.CheckSphere(spherePosition, GroundedCheckRadius, whatIsGround);

        anim.SetBool(animIDGrounded, Grounded);
    }
    void Move()
    {
        if (walkSpeed != moveSpeed / 2.0f)
            walkSpeed = moveSpeed / 2.0f;

        float targetSpeed = _inputWalk ? walkSpeed : moveSpeed;

        if (_inputXZ == Vector2.zero)
            targetSpeed = 0f;

        //플레이어 horizon 벨로시티
        float currentHorizontalSpeed = new Vector3(CC.velocity.x, 0, CC.velocity.z).magnitude;

        //타겟스피드에서 벨로시티값이 벗어남 허용치
        float speedOffset = 0.1f;

        float inputMagnitude = ApplyAnalogMovement ? _inputXZ.magnitude : 1f;

        //횡 속도 기준치에서 벗어날경우 보간해서 보정
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            //소숫점 3자리 반올림
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }

        else
            speed = targetSpeed;

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);

        if (animationBlend < 0.01f)
            animationBlend = 0f;

        //인풋시스템에서 정규화 했으면 굳이 사용 안해도 무방
        //Vector3 inputDirection = new Vector3(_inputXZ.x, 0, _inputXZ.y).normalized;
        inputDirection = new Vector3(_inputXZ.x, 0, _inputXZ.y).normalized;
        inputDirection = orientation.forward * _inputXZ.y + orientation.right * _inputXZ.x;

        if (_inputXZ != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg
                + VCamera.transform.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation,
                ref rotationVelocity, RotatonSmoothTime);

            //카메라 위치에 비례하여 회전
            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        Vector3 targetDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;


        CC.Move(targetDirection.normalized * (speed * Time.deltaTime)
            + new Vector3(0, verticalVelocity, 0) * Time.deltaTime);

        anim.SetFloat(animIDSpeed, animationBlend);
        anim.SetFloat(animIDMotionSpeed, inputMagnitude);
    }
    void JumpAndGravity()
    {
        if (Grounded)
        {
            //낙하 타이머 초기화
            fallTimeoutDelta = FallTimeout;

            anim.SetBool(animIDJump, false);
            //anim.SetBool(animIDFreeFall, false);

            //수직 벨로시티 무한히 증가하는거 방지
            if (verticalVelocity < 0.0f)
                verticalVelocity = -1.0f;

            if (_inputJump && jumpTimeoutDelta <= 0.0f)
            {
                //루트 높이 * -2f * 중력 = 원하는 높이까지 도달하는 벨로시티값
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Gravity);

                anim.SetBool(animIDJump, true);
            }

            //타이머가 0보다 크면 타이머 감소시킴
            if (jumpTimeoutDelta >= 0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }

        else
        {
            //타이머 초기화
            jumpTimeoutDelta = jumpTimeout;

            if (fallTimeoutDelta >= 0.0f)
                fallTimeoutDelta -= Time.deltaTime;

            //else
            //    anim.SetBool(animIDFreeFall, true);

            //혹시 모르니 땅에 붙어있으면 초기화
            _inputJump = false;
        }

        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += Gravity * mass * Time.deltaTime;
        }
    }
    void CameraControl()
    {
        //카메라가 플레이어를 바라보는 벡터값(y축은 플레이어와 동일하게 한 코드)
        LookDir = transform.position - new Vector3(VCamera.transform.position.x, transform.position.y, VCamera.transform.position.z);
        orientation.forward = LookDir.normalized;
    }

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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(new Vector3(transform.position.x,
            transform.position.y - GroundedOffset, transform.position.z), GroundedCheckRadius);
    }
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
    #region DEBUG
    [Space(10)]
    [Header("Debug Info")]
    public bool DEBUG_targetRotaion = false;
    public bool DEBUG_VCamera_transform_eulerAngles_y = false;
    public bool DEBUG_inputDirection = false;
    public bool DEBUG_LookDir = false;
    public bool DEBUG_rotationVelocity = false;
    public bool DEBUG_GetAxisStyle_inputXZ = false;
    public bool DEBUG_verticalVelocity = false;
    void DebugLog()
    {
        if (DEBUG_targetRotaion)
            Debug.Log("targetRotation : " + targetRotation);

        if (DEBUG_VCamera_transform_eulerAngles_y)
            Debug.Log("VCamera.transform.eulerAngles.y : " + VCamera.transform.eulerAngles.y);

        if (DEBUG_inputDirection)
            Debug.Log("inputDirection : " + inputDirection);

        if (DEBUG_LookDir)
            Debug.Log("LookDir : " + LookDir);

        if (DEBUG_rotationVelocity)
            Debug.Log("rotationVelocity : " + rotationVelocity);

        if (DEBUG_GetAxisStyle_inputXZ)
            Debug.Log("GetAxisStyle_inputXZ : " + GetAxisStyle_inputXZ);

        if (DEBUG_verticalVelocity)
            Debug.Log("verticalVelocity : " + verticalVelocity);
    }
    #endregion
}