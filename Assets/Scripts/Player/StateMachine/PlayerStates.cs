
using UnityEngine;

public class PlayerStates
{
    protected PlayerStateMachine stateMachine;
    protected Player player;
    public float StateTimer;

    #region 멤버 연결용 변수선언
    protected CharacterController CC;
    protected Animator anim;

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

    [Header("Jump And Gravity")]
    protected float jumpTimeoutDelta;
    protected float fallTimeoutDelta;
    #endregion
    public PlayerStates(Player player, PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.player = player;
    }

    public virtual void Enter()
    {
        if (player.Log_StateEnter)
            Debug.Log("Enter : " + this.GetType().Name);

        #region 플레이어쪽 멤버 연결
        CC = player.CC;
        anim = player.anim;
        #endregion

    }

    public virtual void Update()
    {
        if (player.Log_StateUpdate)
            Debug.Log("Update : " + this.GetType().Name);

        StateTimer -= Time.deltaTime;

        ApplyGravity();
    }

    public virtual void FixedUpdate()
    {
        if (player.Log_StateFixedUpdate)
            Debug.Log("FixedUpdate : " + this.GetType().Name);
    }

    public virtual void LateUpdate()
    {
        if (player.Log_StateLateUpdate)
            Debug.Log("LateUpdate : " + this.GetType().Name);
    }

    public virtual void Exit()
    {
        if (player.Log_StateExit)
            Debug.Log("Exit : " + this.GetType().Name);
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (player.Log_StateOnCollisionEnter)
            Debug.Log("Collision Enter Class : " + this.GetType().Name);
    }

    public virtual void OnCollisionStay(Collision collision)
    {
        if (player.Log_StateOnCollisionStay)
            Debug.Log("Collision Stay Class : " + this.GetType().Name);
    }

    #region 메소드들
    protected void Move()
    {
        if (player.Log_PlayerSpeed)
            Debug.Log("Current Speed :" + speed);

        if (walkSpeed != player.moveSpeed / 2.0f)
            walkSpeed = player.moveSpeed / 2.0f;

        float targetSpeed = player._inputWalk ? walkSpeed : player.moveSpeed;

        if (player._inputXZ == Vector2.zero)
            targetSpeed = 0f;

        //플레이어 horizon 벨로시티
        float currentHorizontalSpeed = new Vector3(CC.velocity.x, 0, CC.velocity.z).magnitude;

        //타겟스피드에서 벨로시티값이 벗어남 허용치
        float speedOffset = 0.1f;

        float inputMagnitude = player.ApplyAnalogMovement ? player._inputXZ.magnitude : 1f;

        //횡 속도 기준치에서 벗어날경우 보간해서 보정
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
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

            //카메라 위치에 비례하여 회전
            player.transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        Vector3 targetDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;


        CC.Move(targetDirection.normalized * (speed * Time.deltaTime)
            + new Vector3(0, verticalVelocity, 0) * Time.deltaTime);

        anim.SetFloat(player.animIDSpeed, animationBlend);
        anim.SetFloat(player.animIDMotionSpeed, inputMagnitude);
    }
    protected void Jump()
    {
        if (player.isGrounded)
        {
            anim.SetBool(player.animIDJump, false);

            if (player._inputJump && jumpTimeoutDelta <= 0.0f)
            {
                // 루트 높이 * -2f * 중력 = 원하는 높이까지 도달하는 벨로시티값
                verticalVelocity = Mathf.Sqrt(player.jumpHeight * -2f * player.Gravity);
                anim.SetBool(player.animIDJump, true);
            }

            // 타이머가 0보다 크면 타이머 감소시킴
            if (jumpTimeoutDelta >= 0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // 혹시 모르니 땅에 붙어있으면 초기화
            player._inputJump = false;
        }
    }
    protected void ApplyGravity()
    {
        if (player.isGrounded)
        {
            // 낙하 타이머 초기화
            fallTimeoutDelta = player.FallTimeout;

            // 수직 벨로시티 무한히 감소하는 것 방지
            if (verticalVelocity < 0.0f)
                verticalVelocity = -1.0f;
        }
        else
        {
            // 타이머 초기화
            jumpTimeoutDelta = player.jumpTimeout;

            if (fallTimeoutDelta >= 0.0f)
                fallTimeoutDelta -= Time.deltaTime;
        }

        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += player.Gravity * player.mass * Time.deltaTime;
        }
    }

    #endregion
}
