
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerStates
{
    protected PlayerStateMachine stateMachine;
    protected Player player;
    protected TPSController tpsController;
    public float StateTimer;

    #region 멤버 연결용 변수선언
    protected CharacterController CC;
    protected Animator anim;
    protected GameManager gameManager;
    #endregion

    #region 플레이어쪽 은닉화된 변수들
    [Header("Move Info")]
    protected float walkSpeed;
    protected float targetRotation = 0f;
    protected float speed; // 애니메이션 블렌드용
    protected float animationBlend; // 애니메이션 블렌드용
    protected float verticalVelocity;
    protected float terminalVelocity = 160f; //종말속도
    protected float rotationVelocity;
    protected Vector3 targetDirection;
    [Header("Jump And Gravity")]
    protected float jumpTimeoutDelta;
    protected float fallTimeoutDelta;
    #endregion

    #region 관리용 변수
    public bool isAnimEnd;
    protected bool groundToFallState; //코요테 타임용 변수
    protected bool parkourToFallState; //낙하시간 녹화 필요한 파쿠르용 변수
    protected float fallingTimer = 0f;
    protected float moveDirAngle_toLedge;
    #endregion
    public PlayerStates(Player player, PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.player = player;
    }

    public virtual void Enter()
    {
        #region 플레이어쪽 멤버 연결
        CC = player.CC;
        anim = player.anim;
        gameManager = player.gameManager;
        tpsController = player.tpsController;
        #endregion

        if (gameManager.Log_StateEnter)
        {
            Debug.Log("Enter : " + this.GetType().Name);
            Debug.Log("상태 진입시 parkourToFallState : " + parkourToFallState);
        }





        player.Can_MoveHorizontally = true;
        isAnimEnd = false;
    }

    public virtual void Update()
    {
        if (gameManager.Log_StateUpdate)
            Debug.Log("Update : " + this.GetType().Name);

        if (gameManager.Log_isAnimEnd)
            Debug.Log("isAnimEnd : " + isAnimEnd);

        if (gameManager.Log_PlayerVerticalVelocity)
            Debug.Log("VerticalVelocity : " + verticalVelocity);

        moveDirAngle_toLedge = Vector3.Angle(targetDirection, player.LedgeData.LedgeHit.normal);
        if (gameManager.Log_moveDirAngle_toLedget)
            Debug.Log("모서리의 normal기준으로 플레이어의 이동방향 각도 : " + moveDirAngle_toLedge);

        StateTimer -= Time.deltaTime;

        player.isOnLedge = player.DetectingLedge(player.transform.forward, out LedgeData ledgeData,
            player.ledgeCheckHeightStandard_Top, player.ledgeCheckHeightStandard_Bottom);
        player.LedgeData = ledgeData;





        Move();
        LedgeMovement();
        ApplyGravity();
        whenLostControl();


    }

    public virtual void FixedUpdate()
    {
        if (gameManager.Log_StateFixedUpdate)
            Debug.Log("FixedUpdate : " + this.GetType().Name);
    }

    public virtual void LateUpdate()
    {
        if (gameManager.Log_StateLateUpdate)
            Debug.Log("LateUpdate : " + this.GetType().Name);

    }

    public virtual void Exit()
    {
        if (gameManager.Log_StateExit)
            Debug.Log("Exit : " + this.GetType().Name);

        StateTimer = 0;
        player.Can_MoveHorizontally = true;
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (gameManager.Log_StateOnCollisionEnter)
            Debug.Log("Collision Enter Class : " + this.GetType().Name);
    }

    public virtual void OnCollisionStay(Collision collision)
    {
        if (gameManager.Log_StateOnCollisionStay)
            Debug.Log("Collision Stay Class : " + this.GetType().Name);
    }

    public virtual void OnDrawGizmos()
    {

    }


    #region 메소드들
    float targetSpeed;
    protected void Move()
    {
        if (gameManager.Log_PlayerSpeed)
            Debug.Log("Current Speed :" + speed);

        if (walkSpeed != player.moveSpeed / 2.0f)
            walkSpeed = player.moveSpeed / 2.0f;

        if (!player.isControlable)
            return;

        if (player._inputXZ == Vector2.zero || player.horizontalStop)
            player.moveSpeed = 0;

        else
            player.moveSpeed = player.temp_moveSpeed;

        switch (player.isAiming)
        {
            case true:
                targetSpeed = walkSpeed;
                break;
            case false:
                targetSpeed = player._inputWalk ? walkSpeed : player.moveSpeed;
                break;
        }

        //조준중일때도 느리게 움직이게

        //플레이어 horizon 벨로시티
        float currentHorizontalVelocity = new Vector3(CC.velocity.x, 0, CC.velocity.z).magnitude;

        //타겟스피드에서 벨로시티값이 벗어남 허용치
        float speedOffset = 0.1f;

        float inputMagnitude = player.ApplyAnalogMovement ? player._inputXZ.magnitude : 1f;

        //횡 속도 기준치에서 벗어날경우 보간해서 보정
        if (currentHorizontalVelocity < targetSpeed - speedOffset ||
            currentHorizontalVelocity > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalVelocity, targetSpeed * inputMagnitude,
                Time.deltaTime * player.SpeedChangeRate);

            //소숫점 3자리 반올림
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }

        else
            speed = targetSpeed;

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * player.SpeedChangeRate);

        if (animationBlend < 0.01f)
            animationBlend = 0f;

        //인풋시스템에서 정규화 했으면 굳이 사용 안해도 무방
        //Vector3 inputDirection = new Vector3(_inputXZ.x, 0, _inputXZ.y).normalized;
        player.inputDirection = new Vector3(player._inputXZ.x, 0, player._inputXZ.y).normalized;
        player.inputDirection = player.orientation.forward * player._inputXZ.y + player.orientation.right * player._inputXZ.x;

        if (player._inputXZ != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(player.inputDirection.x, player.inputDirection.z) * Mathf.Rad2Deg
            + player.VCamera.transform.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetRotation,
                ref rotationVelocity, player.RotatonSmoothTime);

            //카메라 위치에 비례하여 회전, 공중에 뜬 상태면 회전 불가
            //우클릭해서 조준중일때도 회전 비활성화
            if (player.Can_Rotate && !player.isAiming)
                player.transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        targetDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;

        if (player.isGrounded && player.Can_MoveHorizontally && !LedgeMovement())
        {
            //  Debug.Log("타겟 디렉션 적용중");
            if (!player.isAiming)
                CC.Move(targetDirection.normalized * (speed * Time.deltaTime)
                    + new Vector3(0, verticalVelocity, 0) * Time.deltaTime);


            //조준중일땐 TPSController가 잡아준 cameraRoot값 써서 이동
            else
            {
                Vector3 moveDirection = player.transform.forward * player._inputXZ.y +
                           tpsController.cameraRoot.transform.right * player._inputXZ.x;

                CC.Move(moveDirection * (speed * Time.deltaTime)
                    + new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
            }
        }

        //공중에 뜬 상태면 조작 불가
        else if (!player.isGrounded || !player.Can_MoveHorizontally)
        {
            //  Debug.Log("벨로시티 적용중");
            CC.Move(new Vector3(CC.velocity.x * 0.85f, 0, CC.velocity.z * 0.85f).normalized * (speed * Time.deltaTime)
                 + new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
        }


        anim.SetFloat(gameManager.animIDSpeed, animationBlend);
        anim.SetFloat(gameManager.animIDMotionSpeed, inputMagnitude);
    }

    //모서리에서의 움직임감지
    protected bool shouldJumpDown = false;
    float ledgeTimer = 0f; //모서리 탈출방향으로 입력하는 시간
    protected bool LedgeMovement(float shouldJumpDownHeightLimit = 2f)
    {
        float angle = player.LedgeData.angle;
        float height = player.LedgeData.height;
        float moveDirAngle_toLedge = this.moveDirAngle_toLedge;
        // 플레이어가 입력받아서 움직여야 하는 방향
        if (player.isOnLedge && player._inputXZ != Vector2.zero &&
            moveDirAngle_toLedge < 90 && height <= shouldJumpDownHeightLimit)
        {
            ledgeTimer += Time.deltaTime;
            if (ledgeTimer >= 0.3f)
            {
                ledgeTimer = 0f;
                shouldJumpDown = true;
            }
        }

        else if (player.isOnLedge || moveDirAngle_toLedge >= 90)
        {
            ledgeTimer = 0f;
            shouldJumpDown = false;
        }

        //플레이어의 forward 방향
        if (player.isOnLedge && angle < 90)
        {
            CC.Move(Vector3.zero);
            return true;
        }
        shouldJumpDown = false;
        return false;
    }

    //protected void Jump()
    //{
    //    if (player.isGrounded)
    //    {
    //        anim.SetBool(gameManager.animIDJump, false);

    //        if (player._inputJump && jumpTimeoutDelta <= 0.0f)
    //        {
    //            // 루트 높이 * -2f * 중력 = 원하는 높이까지 도달하는 벨로시티값
    //            verticalVelocity = Mathf.Sqrt(player.jumpHeight * -2f * player.Gravity);
    //            anim.SetBool(gameManager.animIDJump, true);
    //        }

    //        // 타이머가 0보다 크면 타이머 감소시킴
    //        if (jumpTimeoutDelta >= 0f)
    //        {
    //            jumpTimeoutDelta -= Time.deltaTime;
    //        }
    //    }
    //    else
    //    {
    //        // 혹시 모르니 땅에 붙어있으면 초기화
    //        player._inputJump = false;
    //    }
    //}
    protected void ApplyGravity()
    {
        if (!player.isControlable)
            return;


        if (player.isGrounded)
        {
            // 낙하 타이머 초기화
            fallTimeoutDelta = player.FallTimeout;

            // 수직 벨로시티 무한히 감소하는 것 방지
            // 파쿠르 중일때 velocity값 크게 유지되는거 방지(내려오는 파쿠르일때 급강하 방지)
            // 점프중일때 장애물 체크해서 급강하 하는거 방지
            if (verticalVelocity < 0.0f && GetType().Name != "PlayerJumpState")
            {
                if (!player.inParkourAction)
                    verticalVelocity = player.groundedGravity;
            }

        }


        else
        {
            // 타이머 초기화
            jumpTimeoutDelta = player.jumpTimeout;

            if (fallTimeoutDelta >= 0.0f)
                fallTimeoutDelta -= Time.deltaTime;
        }


        if (verticalVelocity < terminalVelocity)
            verticalVelocity += player.Gravity * player.mass * Time.deltaTime;


        if (CC.velocity.y < -0.5f)
            player.mass = 3.5f * 1.5f;

        else
            player.mass = 3.5f;

    }
    protected void whenLostControl()
    {
        if (!player.isControlable)
        {
            //변수 초기해줘야 하는거 있으면 해주기
        }
    }
    #endregion
}
