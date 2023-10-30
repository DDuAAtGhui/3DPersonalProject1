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
    //개별 Collision
    [HideInInspector] public bool PlayFallingAnimation = false;

    [Header("Parkour Info")]
    public List<ParkourAction> parkourActions;
    [HideInInspector] public ParkourAbleObstacleHitData hitData;
    [SerializeField] Vector3 forwardRayOffset = new Vector3(0, 0.25f, 0); //맨 처음 플레이어 앞으로 Ray 발사
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
    [Tooltip("착지 모션 변화 낙하시간 기준")][SerializeField] public float landingMotionTimerStandard = 1f;
    //  [Tooltip("착지 모션 변화 벨로시티 기준")][SerializeField] public float landingMotionVelocityStandard = -10f;

    [Header("Camera Info")]
    [SerializeField] public CinemachineFreeLook VCamera;
    [SerializeField] public CinemachineVirtualCamera AimingCamera;
    [SerializeField] public Transform orientation; //플레이어 원점
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

        #region 디버그 로그

        if (gameManager.Log_PlayerCurrentVelocity)
            Debug.Log("Velocity : " + CC.velocity);

        if (gameManager.Log_RayInfo_Obstacle_DistanceAndHeight)
            Debug.Log($"장애물 거리 : {distanceToObstacle}, 장애물의 높이 : {heightToObstacle} ");

        if (gameManager.Log_WhatisRayHitObstacle)
            if (hitData.forwardHit.transform != null)
                Debug.Log($"감지된 장애물 이름 : {hitData.forwardHit.transform.name}");

        if (gameManager.Log_Local_ForwardHitPoint && hitData.forwardHitFound)
            Debug.Log("Local Foward HitPoint : "
                + LocalFowardHitPoint);

        if (gameManager.Log_LedgeHeight)
            Debug.Log("모서리 높이 : " + LedgeData.height);

        if (gameManager.Log_PlayerTotalFallingTime && totalFallingTime != 0)
            Debug.Log("전체 낙하 시간 : " + totalFallingTime);
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

    #region 구체적 상태들에서 쓸 모노비헤이비어 기능 메소드들
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

    [HideInInspector] Vector3 heightHitPointSnapShot; //가장 최근의 장애물 높이 기억 (초기화X)
    [HideInInspector] Vector3 LocalFowardHitPoint;
    public ParkourAbleObstacleHitData ParkourAbleObstacleCheck()
    {
        hitData = new ParkourAbleObstacleHitData();

        hitData.forwardHitFound = Physics.Raycast(transform.position + forwardRayOffset, transform.forward, out hitData.forwardHit,
            forwardRayLength, Obstacle);

        Debug.DrawRay(transform.position + forwardRayOffset, transform.forward * forwardRayLength, hitData.forwardHitFound ? Color.green : Color.gray);

        if (hitData.forwardHitFound)
        {
            //위에서 아래로 발사하는 방식
            hitData.heightHitFound = Physics.Raycast(hitData.forwardHit.point + Vector3.up * heightRayLength, Vector3.down, out hitData.heighHit, heightRayLength, Obstacle);

            Debug.DrawRay(hitData.forwardHit.point + Vector3.up * heightRayLength, Vector3.down * heightRayLength, hitData.heightHitFound ? Color.green : Color.gray);

            LocalFowardHitPoint = hitData.forwardHit.transform.InverseTransformPoint(hitData.forwardHit.point);

            distanceToObstacle = hitData.forwardHit.distance;
            heightToObstacle = hitData.heighHit.point.y - transform.position.y; ////플레이어 - 장애물간의 정확한 높이차
            heightHitPointSnapShot = hitData.heighHit.point;

            ////두께 검사
            //Debug.DrawRay(hitData.forwardHit.point, -hitData.forwardHit.normal * backSideRayLength, Color.red);


            if (gameManager.Visible_MatchPosition)
                gameManager.StandardTargetMatchingObject.SetActive(true);

            gameManager.StandardTargetMatchingObject.transform.position
                = heightHitPointSnapShot;

            gameManager.CustomTargetMatchingObject.transform.position
             = hangableData.HangableHit.point;


            //TargetMatchOffsetStandard의 Forward 방향을 플레이어가 파쿠르 하는 방향과 일치시키기
            //이래야 offset할 때 플레이어 위치 변해도 일정한 결과값 도출
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
    /// <param name="TargetMathcingTF">타겟 매칭 포지션 임의로 지정가능(사용 안할시 Vector3.zero)</param>
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

            //인덱스 초과 예외처리
            //State를 여러개 갖고있고, 여러개 변수를 넣을때 인덱스로 따로 구분 안해주면 ture 걸릴때 무조건 첫번째로 파라미터에 넣은 액션이 나오기떄문에
            //params로 무한히 받고 배열 인덱스로 따로 관리해줌
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

            Debug.Log("액션 : " + currentParkourActionIndex);

            //타겟매칭 활성화 하면 실행
            if (bestMatchAction.EnableTargetMatching)
            {
                //타겟매칭 기준 오브젝트를 항상 파쿠르 방향으로 바라보게하고
                //그 방향의 Forward방향으로 타겟매칭 오프셋 적용
                gameManager.StandardTargetMatchingObject.transform.localPosition +=
                Quaternion.LookRotation(gameManager.StandardTargetMatchingObject.transform.forward) * bestMatchAction.MatchPositionOffset;

                gameManager.CustomTargetMatchingObject.transform.localPosition +=
                Quaternion.LookRotation(gameManager.CustomTargetMatchingObject.transform.forward) * bestMatchAction.MatchPositionOffset;

                StartCoroutine(PerformMatchTargetCor(bestMatchAction, TargetMathcingPosition));
            }


            stateMachine.ChangeState(bestMatchStates);
            #region 디버그
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


            //1프레임 단위로 끊어줌
            //MatchTarget보다 반드시 위에
            yield return null;

            MatchTarget(action, TargetMathcingPosition);
        }
    }

    public void MatchTarget(ParkourAction action, Vector3 TargetMatchingPosition = default(Vector3))
    {
        //Debug.Log("anim.isMatchingTarget : " + anim.isMatchingTarget);
        //Debug.Log("heightHitPointSnapShot : " + heightHitPointSnapShot);

        //이미 타겟 매칭중이면 쓸데없는 실행 X
        if (anim.isMatchingTarget)
        {
            //if (gameManager.Log_TargetMatch)
            //    Debug.Log("타겟매칭 이미 적용중");

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

            Debug.Log("타겟매칭 일어날때 타겟매칭 포지션 : " + TargetMatchingPosition);
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
            Debug.Log("플레이어가 바쁜상태");

        yield return new WaitForSeconds(seconds);

        if (!isBusy && gameManager.Log_isBusy)
            Debug.Log("플레이어가 여유로운 상태");
        isBusy = false;
    }
    public void SetControllable(bool isControlable)
    {
        //애니메이션핸들러에도 on 할 수 있는 메소드 만들어놨음
        //파쿠르할때 콜라이더 걸려서 안올라가는거 방지
        this.isControlable = isControlable;
        CC.enabled = isControlable;
    }
    public void isAnimEnd() => stateMachine.currentState.isAnimEnd = true;
    #endregion

    #region 인풋액션 - 액션들
    void onMoveAction(InputAction.CallbackContext context)
        => _inputXZ = context.ReadValue<Vector2>();


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

    void onLookAction(InputAction.CallbackContext context)
    {
        Look = context.ReadValue<Vector2>();
    }

    void onAimAction(InputAction.CallbackContext context)
    {
        _InputAim = context.ReadValueAsButton();
    }
    #endregion

    #region 캐릭터 컨트롤러 쓸 때 항상 해줘야하는거
    //인풋시스템 쓸 때 항상 키고 끄고 해줘야함
    private void OnEnable() => playerInput.CharacterControls.Enable();
    void OnDisable() => playerInput.CharacterControls.Disable();


    #endregion


    #region 연습용
    #endregion
}

#region 임시보관용
//public void PerformParkourState(params PlayerStates[] parkourStates)
//{
//    parkourActionIndex = 0;

//    List<ParkourAction> temp_parkourActions = new List<ParkourAction>();

//    parkourActions.ForEach(action =>
//    {
//        if (gameManager.Log_ParkourPossibleState)
//            Debug.Log($"Num {parkourActionIndex} : " + action.CheckIfPossible());

//        // Debug.Log("actionList : " + action + " excutable : " + action.excutable);

//        //인덱스 초과 예외처리
//        //State를 여러개 갖고있고, 여러개 변수를 넣을때 인덱스로 따로 구분 안해주면 ture 걸릴때 무조건 첫번째로 파라미터에 넣은 액션이 나오기떄문에
//        //params로 무한히 받고 배열 인덱스로 따로 관리해줌
//        if (parkourActionIndex < parkourStates.Length && action.CheckIfPossible())
//        {
//            currentParkourActionIndex = parkourActionIndex;

//            //타겟매칭 활성화 하면 실행
//            if (action.EnableTargetMatching)
//            {
//                //타겟매칭 기준 오브젝트를 항상 파쿠르 방향으로 바라보게하고
//                //그 방향의 Forward방향으로 타겟매칭 오프셋 적용
//                gameManager.TargetMatchOffsetStandard.transform.localPosition +=
//                Quaternion.LookRotation(gameManager.TargetMatchOffsetStandard.transform.forward) * action.MatchPositionOffset;

//                StartCoroutine(PerformMatchTargetCor(action));

//                #region 디버그
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

//          //  Debug.Log("실행된 액션 : " + action + " excutable : " + action.excutable);
//            stateMachine.ChangeState(parkourStates[parkourActionIndex]);
//        }
//        parkourActionIndex++;
//    });
//}
#endregion