using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class Player : Entity
{
    [HideInInspector] public bool isBusy { get; private set; }
    #region components
    // [Header("Player")]
    [HideInInspector] public CharacterController CC;
    [HideInInspector] public PlayerInputSystem playerInput;
    [HideInInspector] public Animator anim;
    #endregion
    #region infos
    //���� Collision
    [HideInInspector] public bool PlayFallingAnimation = false;

    [Header("Parkour Info")]
    public List<ParkourAction> parkourActions;
    [HideInInspector] public ParkourAbleObstacleHitData hitData;
    [SerializeField] Vector3 forwardRayOffset = new Vector3(0, 0.25f, 0); //�� ó�� �÷��̾� ������ Ray �߻�
    [SerializeField] float forwardRayLength = 0.5f;
    [SerializeField] float heightRayLength = 100f;
    [SerializeField] float backSideRayLength = 100f;

    [HideInInspector][SerializeField] public float distanceToObstacle;
    [HideInInspector][SerializeField] public float heightToObstacle;
    [HideInInspector][SerializeField] public float thicknessOfObstacle;
    [HideInInspector] public bool inParkourAction;
    [HideInInspector] public int currentParkourActionIndex;
    [HideInInspector] int parkourActionIndex = 0;

    [HideInInspector] public LedgeData LedgeData { get; set; }


    [Header("Move Info")]
    [SerializeField] public float moveSpeed = 3f;
    [HideInInspector] public float temp_moveSpeed;
    [SerializeField] public float RotatonSmoothTime = 0.12f;
    [SerializeField] public float SpeedChangeRate = 10.0f;
    [SerializeField] public bool Can_MoveHorizontally = true;
    [HideInInspector] public bool Can_Rotate = true;
    [HideInInspector] public bool horizontalStop;

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
    [SerializeField] public float groundedGravity = -1f;
    [SerializeField] public float mass = 1f;
    [Tooltip("�ٽ� ���� �� �� ���������� �ɸ��� �ð�")]
    [SerializeField] public float jumpTimeout = 0.5f;
    [Tooltip("fall������Ʈ ���Ա��� �ɸ��½ð� (��ܿ� ����)")]
    [SerializeField] public float FallTimeout = 0.15f;
    [Tooltip("���� ���� ���� ���� �� ���� �Է� �� �� �ִ� �����ð�")][SerializeField] public float CoyoteTime = 0.1f;
    [Tooltip("���� ��� ��ȭ ���Ͻð� ����")][SerializeField] public float landingMotionTimerStandard = 1f;
    //  [Tooltip("���� ��� ��ȭ ���ν�Ƽ ����")][SerializeField] public float landingMotionVelocityStandard = -10f;

    [Header("Camera Info")]
    [SerializeField] public CinemachineFreeLook VCamera;
    [SerializeField] public CinemachineVirtualCamera AimingCamera;
    [SerializeField] public Transform orientation; //�÷��̾� ����
    [HideInInspector] public Vector3 LookDir;


    [Header("Input Info")]
    public Vector2 _inputXZ; // WASD
    [HideInInspector] public bool _inputWalk; // Left Shift
    [HideInInspector] public bool _inputCurosrVisible; // Left Alt
    [HideInInspector] public bool _inputJump; // Space
    public bool ApplyAnalogMovement = false;
    public bool isControlable = true;
    [HideInInspector] public Vector3 inputDirection;

    [Header("Aim Info")]
    [SerializeField] public Vector2 Look;
    [HideInInspector] public bool _InputAim; // RightClick
    [HideInInspector] public bool isAiming;
    #endregion

    #region ���µ�, ��ü����, ��ǲ�ý��� �ݹ�
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
    public PlayerCrouchToClimbUpState crouchToClimbUpState { get; private set; }
    public PlayerJumpOver_RollState jumpOver_RollState { get; private set; }
    public PlayerStandJumpingDownState standjumpingDownState { get; private set; }
    public PlayerHangingIdleWallState hangingIdleWallState { get; private set; }
    public PlayerIdleToHangWallState idleToHangWallState { get; private set; }
    public PlayerJumpFromHangingWallState jumpFromHangingWallState { get; private set; }
    public PlayerBracedHangHopUpState bracedHangHopUpState { get; private set; }
    public PlayerBracedHangHopDownState bracedHangHopDownState { get; private set; }
    public PlayerBracedHangHopRightState bracedHangHopRightState { get; private set; }
    public PlayerBracedHangHopLeftState bracedHangHopLeftState { get; private set; }
    public PlayerBracedHangShimmyRightState bracedHangShimmyRightState { get; private set; }
    public PlayerBracedHangShimmyLeftState bracedHangShimmyLeftState { get; private set; }
    public PlayerBracedHangToCrouchState bracedHangToCrouchState { get; private set; }
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
        crouchToClimbUpState = new PlayerCrouchToClimbUpState(this, stateMachine);
        jumpOver_RollState = new PlayerJumpOver_RollState(this, stateMachine);
        standjumpingDownState = new PlayerStandJumpingDownState(this, stateMachine);
        hangingIdleWallState = new PlayerHangingIdleWallState(this, stateMachine);
        idleToHangWallState = new PlayerIdleToHangWallState(this, stateMachine);
        jumpFromHangingWallState = new PlayerJumpFromHangingWallState(this, stateMachine);
        bracedHangHopUpState = new PlayerBracedHangHopUpState(this, stateMachine);
        bracedHangHopDownState = new PlayerBracedHangHopDownState(this, stateMachine);
        bracedHangHopRightState = new PlayerBracedHangHopRightState(this, stateMachine);
        bracedHangHopLeftState = new PlayerBracedHangHopLeftState(this, stateMachine);
        bracedHangShimmyRightState = new PlayerBracedHangShimmyRightState(this, stateMachine);
        bracedHangShimmyLeftState = new PlayerBracedHangShimmyLeftState(this, stateMachine);
        bracedHangToCrouchState = new PlayerBracedHangToCrouchState(this, stateMachine);
        #region ������Ʈ
        CC = GetComponentInChildren<CharacterController>();
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

        playerInput.CharacterControls.Look.started += onLookAction;
        playerInput.CharacterControls.Look.performed += onLookAction;
        playerInput.CharacterControls.Look.canceled += onLookAction;

        playerInput.CharacterControls.Aim.started += onAimAction;
        playerInput.CharacterControls.Aim.canceled += onAimAction;
        #endregion
    }
    #endregion
    public override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);

        temp_moveSpeed = moveSpeed;
    }

    public override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
        CameraControl();
        GroundCheck();

        if (!inParkourAction)
            ParkourAbleObstacleCheck();

        #region ����� �α�

        if (gameManager.Log_PlayerCurrentVelocity)
            Debug.Log("Velocity : " + CC.velocity);

        if (gameManager.Log_RayInfo_Obstacle_DistanceAndHeight)
            Debug.Log($"��ֹ� �Ÿ� : {distanceToObstacle}, ��ֹ��� ���� : {heightToObstacle} ");

        if (gameManager.Log_WhatisRayHitObstacle)
            if (hitData.forwardHit.transform != null)
                Debug.Log($"������ ��ֹ� �̸� : {hitData.forwardHit.transform.name}");

        if (gameManager.Log_Local_ForwardHitPoint && hitData.forwardHitFound)
            Debug.Log("Local Foward HitPoint : "
                + LocalFowardHitPoint);

        if (gameManager.Log_LedgeHeight)
            Debug.Log("�𼭸� ���� : " + LedgeData.height);

        if (gameManager.Log_PlayerTotalFallingTime && totalFallingTime != 0)
            Debug.Log("��ü ���� �ð� : " + totalFallingTime);
        #endregion

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

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (stateMachine != null && stateMachine.currentState != null)
        {
            stateMachine.currentState.OnDrawGizmos();
        }
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

    [HideInInspector] public bool isLanding = false;
    void GroundCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x,
            transform.position.y - GroundedOffset, transform.position.z);

        isGrounded = Physics.CheckSphere(spherePosition, GroundedCheckRadius, whatIsGround);

        PlayFallingAnimation = !Physics.Raycast(transform.position, Vector3.down, CanPlayFallingAnimationDistance, CanPlayFallingAimationLayer);

        if (!isLanding)
            anim.SetBool(gameManager.animIDGrounded, isGrounded);
    }

    [HideInInspector] Vector3 heightHitPointSnapShot; //���� �ֱ��� ��ֹ� ���� ��� (�ʱ�ȭX)
    [HideInInspector] Vector3 LocalFowardHitPoint;
    public ParkourAbleObstacleHitData ParkourAbleObstacleCheck()
    {
        hitData = new ParkourAbleObstacleHitData();

        hitData.forwardHitFound = Physics.Raycast(transform.position + forwardRayOffset, transform.forward, out hitData.forwardHit,
            forwardRayLength, Obstacle);

        Debug.DrawRay(transform.position + forwardRayOffset, transform.forward * forwardRayLength, hitData.forwardHitFound ? Color.green : Color.gray);

        if (hitData.forwardHitFound)
        {
            //������ �Ʒ��� �߻��ϴ� ���
            hitData.heightHitFound = Physics.Raycast(hitData.forwardHit.point + Vector3.up * heightRayLength, Vector3.down, out hitData.heighHit, heightRayLength, Obstacle);

            Debug.DrawRay(hitData.forwardHit.point + Vector3.up * heightRayLength, Vector3.down * heightRayLength, hitData.heightHitFound ? Color.green : Color.gray);

            LocalFowardHitPoint = hitData.forwardHit.transform.InverseTransformPoint(hitData.forwardHit.point);

            distanceToObstacle = hitData.forwardHit.distance;
            heightToObstacle = hitData.heighHit.point.y - transform.position.y; ////�÷��̾� - ��ֹ����� ��Ȯ�� ������
            heightHitPointSnapShot = hitData.heighHit.point;

            ////�β� �˻�
            //Debug.DrawRay(hitData.forwardHit.point, -hitData.forwardHit.normal * backSideRayLength, Color.red);


            if (gameManager.Visible_MatchPosition)
                gameManager.StandardTargetMatchingObject.SetActive(true);

            gameManager.StandardTargetMatchingObject.transform.position
                = heightHitPointSnapShot;

            gameManager.CustomTargetMatchingObject.transform.position
             = hangableData.HangableHit.point;


            //TargetMatchOffsetStandard�� Forward ������ �÷��̾ ���� �ϴ� ����� ��ġ��Ű��
            //�̷��� offset�� �� �÷��̾� ��ġ ���ص� ������ ����� ����
            gameManager.StandardTargetMatchingObject.transform.rotation =
                Quaternion.LookRotation(-hitData.forwardHit.normal);

        }

        else
        {
            distanceToObstacle = 0;
            heightToObstacle = 0;
            gameManager.StandardTargetMatchingObject.SetActive(false);
        }
        return hitData;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="TargetMathcingTF">Ÿ�� ��Ī ������ ���Ƿ� ��������(��� ���ҽ� Vector3.zero)</param>
    /// <param name="parkourStates"></param>
    public PlayerStates bestMatchStates;
    public void PerformParkourState(Vector3 TargetMathcingPosition = default(Vector3), params PlayerStates[] parkourStates)
    {
        parkourActionIndex = 0;

        var tempPercentage = float.MinValue;
        ParkourAction bestMatchAction = null;

        parkourActions.ForEach(action =>
        {
            if (gameManager.Log_ParkourPossibleState)
                Debug.Log($"Num {parkourActionIndex} : " + action.CheckIfPossible());


            // Debug.Log("actionList : " + action + " excutable : " + action.excutable);

            //�ε��� �ʰ� ����ó��
            //State�� ������ �����ְ�, ������ ������ ������ �ε����� ���� ���� �����ָ� ture �ɸ��� ������ ù��°�� �Ķ���Ϳ� ���� �׼��� �����⋚����
            //params�� ������ �ް� �迭 �ε����� ���� ��������
            if (parkourActionIndex < parkourActions.Count && action.CheckIfPossible())
            {
                bestMatchStates = LevenShteinStringSimilarity.FindMostSimilarState
                    (action.ToString(), parkourStates, out float matchingPercentage);

                if (matchingPercentage > tempPercentage)
                {
                    tempPercentage = matchingPercentage;
                    bestMatchAction = action;
                }
            }
            parkourActionIndex++;
        });

        if (bestMatchAction != null)
        {
            currentParkourActionIndex =
                parkourActions.IndexOf(bestMatchAction);

            Debug.Log("�׼� : " + currentParkourActionIndex);

            //Ÿ�ٸ�Ī Ȱ��ȭ �ϸ� ����
            if (bestMatchAction.EnableTargetMatching)
            {
                //Ÿ�ٸ�Ī ���� ������Ʈ�� �׻� ���� �������� �ٶ󺸰��ϰ�
                //�� ������ Forward�������� Ÿ�ٸ�Ī ������ ����
                gameManager.StandardTargetMatchingObject.transform.localPosition +=
                Quaternion.LookRotation(gameManager.StandardTargetMatchingObject.transform.forward) * bestMatchAction.MatchPositionOffset;

                gameManager.CustomTargetMatchingObject.transform.localPosition +=
                Quaternion.LookRotation(gameManager.CustomTargetMatchingObject.transform.forward) * bestMatchAction.MatchPositionOffset;

                StartCoroutine(PerformMatchTargetCor(bestMatchAction, TargetMathcingPosition));
            }


            stateMachine.ChangeState(bestMatchStates);
            #region �����
            if (gameManager.Log_TargetMatch)
            {
                Debug.Log($"==================\n" +
                 $"MatchTarget Info\n" +
                 $"action name : {bestMatchAction.ToString()}\n" +
                 $"anim.targetPosition : {anim.targetPosition} \n " +
                 $"action.MatchBodyPart : {bestMatchAction.MatchBodyPart}\n" +
                 $"action.MatchStartTime : {bestMatchAction.MatchStartTime}\n" +
                 $"action.MatchTargetTime : {bestMatchAction.MatchTargetTime}");
            }
            #endregion
        }
    }

    IEnumerator PerformMatchTargetCor(ParkourAction action, Vector3 TargetMathcingPosition = default(Vector3))
    {
        float timer = 0f;

        while (timer <= anim.GetNextAnimatorStateInfo(0).length)
        {
            timer += Time.deltaTime;


            //1������ ������ ������
            //MatchTarget���� �ݵ�� ����
            yield return null;

            MatchTarget(action, TargetMathcingPosition);
        }
    }

    public void MatchTarget(ParkourAction action, Vector3 TargetMatchingPosition = default(Vector3))
    {
        //Debug.Log("anim.isMatchingTarget : " + anim.isMatchingTarget);
        //Debug.Log("heightHitPointSnapShot : " + heightHitPointSnapShot);

        //�̹� Ÿ�� ��Ī���̸� �������� ���� X
        if (anim.isMatchingTarget)
        {
            //if (gameManager.Log_TargetMatch)
            //    Debug.Log("Ÿ�ٸ�Ī �̹� ������");

            return;
        }

        if (TargetMatchingPosition == Vector3.zero)
        {
            if (!action.UseCustomTargetMatchingPosition)
                anim.MatchTarget(gameManager.StandardTargetMatchingObject.transform.position, transform.rotation, action.MatchBodyPart,
                new MatchTargetWeightMask(action.MatchPositionWeight, action.MatchPositionRotateWeight),
                action.MatchStartTime, action.MatchTargetTime);

            else
                anim.MatchTarget(gameManager.CustomTargetMatchingObject.transform.position, transform.rotation, action.MatchBodyPart,
                new MatchTargetWeightMask(action.MatchPositionWeight, action.MatchPositionRotateWeight),
                action.MatchStartTime, action.MatchTargetTime);
        }

        else
        {
            anim.MatchTarget(TargetMatchingPosition + action.MatchPositionOffset, transform.rotation, action.MatchBodyPart,
            new MatchTargetWeightMask(action.MatchPositionWeight, action.MatchPositionRotateWeight),
            action.MatchStartTime, action.MatchTargetTime);

            Debug.Log("Ÿ�ٸ�Ī �Ͼ�� Ÿ�ٸ�Ī ������ : " + TargetMatchingPosition);
        }
    }

    public void ResetMatchTarget()
    {
        anim.MatchTarget(Vector3.zero, Quaternion.identity, AvatarTarget.Root,
            new MatchTargetWeightMask(Vector3.zero, 0f), 0.0f, 0.0f);
    }

    public IEnumerator nowBusy(float seconds)
    {
        isBusy = true;
        if (isBusy && gameManager.Log_isBusy)
            Debug.Log("�÷��̾ �ٻۻ���");

        yield return new WaitForSeconds(seconds);

        if (!isBusy && gameManager.Log_isBusy)
            Debug.Log("�÷��̾ �����ο� ����");
        isBusy = false;
    }
    public void SetControllable(bool isControlable)
    {
        //�ִϸ��̼��ڵ鷯���� on �� �� �ִ� �޼ҵ� ��������
        //�����Ҷ� �ݶ��̴� �ɷ��� �ȿö󰡴°� ����
        this.isControlable = isControlable;
        CC.enabled = isControlable;
    }
    public void isAnimEnd() => stateMachine.currentState.isAnimEnd = true;
    #endregion

    #region ��ǲ�׼� - �׼ǵ�
    void onMoveAction(InputAction.CallbackContext context)
        => _inputXZ = context.ReadValue<Vector2>();


    void onWalkAction(InputAction.CallbackContext context)
    {
        //���� �پ��������� �Է°���, LandingState Exit�ʿ� walk = false �����س���
        //walk ����ä�� �����Ѵ㿡 ���� ���� ���� walk�� �����Ǵ� �����־ �����صа�
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

    void onLookAction(InputAction.CallbackContext context)
    {
        Look = context.ReadValue<Vector2>();
    }

    void onAimAction(InputAction.CallbackContext context)
    {
        _InputAim = context.ReadValueAsButton();
    }
    #endregion

    #region ĳ���� ��Ʈ�ѷ� �� �� �׻� ������ϴ°�
    //��ǲ�ý��� �� �� �׻� Ű�� ���� �������
    private void OnEnable() => playerInput.CharacterControls.Enable();
    void OnDisable() => playerInput.CharacterControls.Disable();


    #endregion


    #region ������
    #endregion
}

#region �ӽú�����
//public void PerformParkourState(params PlayerStates[] parkourStates)
//{
//    parkourActionIndex = 0;

//    List<ParkourAction> temp_parkourActions = new List<ParkourAction>();

//    parkourActions.ForEach(action =>
//    {
//        if (gameManager.Log_ParkourPossibleState)
//            Debug.Log($"Num {parkourActionIndex} : " + action.CheckIfPossible());

//        // Debug.Log("actionList : " + action + " excutable : " + action.excutable);

//        //�ε��� �ʰ� ����ó��
//        //State�� ������ �����ְ�, ������ ������ ������ �ε����� ���� ���� �����ָ� ture �ɸ��� ������ ù��°�� �Ķ���Ϳ� ���� �׼��� �����⋚����
//        //params�� ������ �ް� �迭 �ε����� ���� ��������
//        if (parkourActionIndex < parkourStates.Length && action.CheckIfPossible())
//        {
//            currentParkourActionIndex = parkourActionIndex;

//            //Ÿ�ٸ�Ī Ȱ��ȭ �ϸ� ����
//            if (action.EnableTargetMatching)
//            {
//                //Ÿ�ٸ�Ī ���� ������Ʈ�� �׻� ���� �������� �ٶ󺸰��ϰ�
//                //�� ������ Forward�������� Ÿ�ٸ�Ī ������ ����
//                gameManager.TargetMatchOffsetStandard.transform.localPosition +=
//                Quaternion.LookRotation(gameManager.TargetMatchOffsetStandard.transform.forward) * action.MatchPositionOffset;

//                StartCoroutine(PerformMatchTargetCor(action));

//                #region �����
//                if (gameManager.Log_TargetMatch)
//                {
//                    Debug.Log($"==================\n" +
//                     $"MatchTarget Info\n" +
//                     $"anim.targetPosition : {anim.targetPosition} \n " +
//                     $"action.MatchPosition : {action.MatchPosition} \n " +
//                     $"action.MatchBodyPart : {action.MatchBodyPart}\n" +
//                     $"action.MatchStartTime : {action.MatchStartTime}\n" +
//                     $"action.MatchTargetTime : {action.MatchTargetTime}");
//                }
//                #endregion
//            }

//          //  Debug.Log("����� �׼� : " + action + " excutable : " + action.excutable);
//            stateMachine.ChangeState(parkourStates[parkourActionIndex]);
//        }
//        parkourActionIndex++;
//    });
//}
#endregion