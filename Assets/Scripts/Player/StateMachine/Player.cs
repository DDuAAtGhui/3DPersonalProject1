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
    //[HideInInspector] public float speed; // �ִϸ��̼� �����
    //[HideInInspector] public float animationBlend; // �ִϸ��̼� �����
    //[HideInInspector] public float verticalVelocity;
    //[HideInInspector] public float terminalVelocity = 160f; //�����ӵ�
    //[HideInInspector] public float rotationVelocity;

    [Header("Jump And Gravity")]
    [SerializeField] public float jumpHeight = 12f;
    [SerializeField] public float Gravity = -9.81f;
    [SerializeField] public float mass = 1f;
    [SerializeField] public float jumpTimeout = 0.5f; //�ٽ� ���� �� �� ���������� �ɸ��� �ð�
    [SerializeField] public float FallTimeout = 0.15f; // fall������Ʈ ���Ա��� �ɸ��½ð� (��ܿ� ����)

    [Header("Camera Info")]
    [SerializeField] public CinemachineFreeLook VCamera;
    [SerializeField] public Transform orientation; //�÷��̾� ����
    [HideInInspector] public Vector3 LookDir;


    [Header("Input Info")]
    public Vector2 _inputXZ; // WASD
    public bool _inputWalk; // Left Shift
    public bool _inputCurosrVisible; // Left Alt
    public bool _inputJump; // Space
    public bool ApplyAnalogMovement = false;
    [HideInInspector] public Vector3 inputDirection;

    // GetAxis��Ÿ�� ��������� ���
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
    #region ���µ�, ��ü����, ��ǲ�ý��� �ݹ�
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
        #region ������Ʈ
        CC = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        #endregion
        #region ��ǲ�ý��� �ݹ����
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

    #region ��ü�� ���µ鿡�� �� �������̺�� ��� �޼ҵ��
    public void OnCollisionEnter(Collision collision)
    {
        stateMachine.currentState.OnCollisionEnter(collision);
    }

    public void OnCollisionStay(Collision collision)
    {
        stateMachine.currentState.OnCollisionStay(collision);
    }

    #endregion
    #region �޼ҵ��
    void CameraControl()
    {
        //ī�޶� �÷��̾ �ٶ󺸴� ���Ͱ�(y���� �÷��̾�� �����ϰ� �� �ڵ�)
        LookDir = transform.position - new Vector3(VCamera.transform.position.x, transform.position.y, VCamera.transform.position.z);
        orientation.forward = LookDir.normalized;

        if (_inputXZ != Vector2.zero)
        {
            //�����϶� �����͸� �ð� ����ϴ°� ����(�̰� ������ �����϶� �����͸� �ȵǵ� WaitTime�� ����
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
            Debug.Log("�÷��̾ �ٻۻ���");

        yield return new WaitForSeconds(seconds);

        if (isBusy)
            Debug.Log("�÷��̾ �����ο� ����");
        isBusy = false;
    }
    #endregion

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
    [Tooltip("�÷��̾ ���� �ٻ��� ǥ��")] public bool Log_isBusy = true;

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
