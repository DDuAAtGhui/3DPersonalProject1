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
    float speed; // �ִϸ��̼� �����
    float animationBlend; // �ִϸ��̼� �����
    float verticalVelocity;
    float terminalVelocity = 160f; //�����ӵ�
    float rotationVelocity;

    [Header("Jump And Gravity")]
    [SerializeField] float jumpHeight = 1.2f;
    [SerializeField] float Gravity = -9.81f;
    [SerializeField] float mass = 1f;
    [SerializeField] float jumpTimeout = 0.5f; //�ٽ� ���� �� �� ���������� �ɸ��� �ð�
    [SerializeField] float FallTimeout = 0.15f; // fall������Ʈ ���Ա��� �ɸ��½ð� (��ܿ� ����)

    float jumpTimeoutDelta;
    float fallTimeoutDelta;


    [Header("Collision Info")]
    [SerializeField] bool Grounded = false;
    [SerializeField] float GroundedOffset = -0.14f; //��ģ ǥ�� üũ�� ����
    [SerializeField] float GroundedCheckRadius = 0.28f; //�׶��� üũ ���� ������. ĳ���� ��Ʈ�ѷ��� �����ؾ���
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

    // GetAxis��Ÿ�� ��������� ���
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

        //�ν��Ͻ� ����
        //started �ݹ鿡 onMoveAction �޼ҵ� ���
        playerInput.CharacterControls.Move.started += onMoveAction;
        //���̽�ƽ�� ��ƽ���� ��Ʈ���ϴϱ� perform�ʼ���
        //�ٵ� Ű���嵵 �ᱹ 4���⸸ �Ұ� �ƴϸ� perform�ϴ°� ����
        //performed�� 0�� 1������ ���� �ݹ����
        playerInput.CharacterControls.Move.performed += onMoveAction;
        //Ű�� ���������� �����ϴ� �ݹ�
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

        //�÷��̾� horizon ���ν�Ƽ
        float currentHorizontalSpeed = new Vector3(CC.velocity.x, 0, CC.velocity.z).magnitude;

        //Ÿ�ٽ��ǵ忡�� ���ν�Ƽ���� ��� ���ġ
        float speedOffset = 0.1f;

        float inputMagnitude = ApplyAnalogMovement ? _inputXZ.magnitude : 1f;

        //Ⱦ �ӵ� ����ġ���� ������ �����ؼ� ����
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            //�Ҽ��� 3�ڸ� �ݿø�
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }

        else
            speed = targetSpeed;

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);

        if (animationBlend < 0.01f)
            animationBlend = 0f;

        //��ǲ�ý��ۿ��� ����ȭ ������ ���� ��� ���ص� ����
        //Vector3 inputDirection = new Vector3(_inputXZ.x, 0, _inputXZ.y).normalized;
        inputDirection = new Vector3(_inputXZ.x, 0, _inputXZ.y).normalized;
        inputDirection = orientation.forward * _inputXZ.y + orientation.right * _inputXZ.x;

        if (_inputXZ != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg
                + VCamera.transform.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation,
                ref rotationVelocity, RotatonSmoothTime);

            //ī�޶� ��ġ�� ����Ͽ� ȸ��
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
            //���� Ÿ�̸� �ʱ�ȭ
            fallTimeoutDelta = FallTimeout;

            anim.SetBool(animIDJump, false);
            //anim.SetBool(animIDFreeFall, false);

            //���� ���ν�Ƽ ������ �����ϴ°� ����
            if (verticalVelocity < 0.0f)
                verticalVelocity = -1.0f;

            if (_inputJump && jumpTimeoutDelta <= 0.0f)
            {
                //��Ʈ ���� * -2f * �߷� = ���ϴ� ���̱��� �����ϴ� ���ν�Ƽ��
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * Gravity);

                anim.SetBool(animIDJump, true);
            }

            //Ÿ�̸Ӱ� 0���� ũ�� Ÿ�̸� ���ҽ�Ŵ
            if (jumpTimeoutDelta >= 0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }

        else
        {
            //Ÿ�̸� �ʱ�ȭ
            jumpTimeoutDelta = jumpTimeout;

            if (fallTimeoutDelta >= 0.0f)
                fallTimeoutDelta -= Time.deltaTime;

            //else
            //    anim.SetBool(animIDFreeFall, true);

            //Ȥ�� �𸣴� ���� �پ������� �ʱ�ȭ
            _inputJump = false;
        }

        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += Gravity * mass * Time.deltaTime;
        }
    }
    void CameraControl()
    {
        //ī�޶� �÷��̾ �ٶ󺸴� ���Ͱ�(y���� �÷��̾�� �����ϰ� �� �ڵ�)
        LookDir = transform.position - new Vector3(VCamera.transform.position.x, transform.position.y, VCamera.transform.position.z);
        orientation.forward = LookDir.normalized;
    }

    #region ��ǲ�׼� - �׼ǵ�
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
    #region ĳ���� ��Ʈ�ѷ� �� �� �׻� ������ϴ°�
    //��ǲ�ý��� �� �� �׻� Ű�� ���� �������
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