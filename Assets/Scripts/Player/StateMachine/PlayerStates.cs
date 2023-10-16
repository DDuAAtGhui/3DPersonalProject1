
using UnityEngine;

public class PlayerStates
{
    protected PlayerStateMachine stateMachine;
    protected Player player;
    public float StateTimer;

    #region ��� ����� ��������
    protected CharacterController CC;
    protected Animator anim;
    protected GameManager gameManager;
    #endregion

    #region �÷��̾��� ����ȭ�� ������
    [Header("Move Info")]
    protected float walkSpeed;
    protected float targetRotation = 0f;
    protected float speed; // �ִϸ��̼� �����
    protected float animationBlend; // �ִϸ��̼� �����
    protected float verticalVelocity;
    protected float terminalVelocity = 160f; //�����ӵ�
    protected float rotationVelocity;
    protected Vector3 targetDirection;
    [Header("Jump And Gravity")]
    protected float jumpTimeoutDelta;
    protected float fallTimeoutDelta;
    #endregion

    #region ������ ����
    public bool isAnimEnd;
    protected bool groundToFallState; //�ڿ��� Ÿ�ӿ� ����
    protected bool parkourToFallState; //���Ͻð� ��ȭ �ʿ��� ������ ����
    protected float fallingTimer = 0f;
    #endregion
    public PlayerStates(Player player, PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.player = player;
    }

    public virtual void Enter()
    {
        #region �÷��̾��� ��� ����
        CC = player.CC;
        anim = player.anim;
        gameManager = player.gameManager;
        #endregion

        if (gameManager.Log_StateEnter)
            Debug.Log("Enter : " + this.GetType().Name);




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

        StateTimer -= Time.deltaTime;

        player.isOnLedge = player.DetectingLedge(player.transform.forward, out LedgeData ledgeData, 1.5f);
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
    #region �޼ҵ��
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

        targetSpeed = player._inputWalk ? walkSpeed : player.moveSpeed;

        //�÷��̾� horizon ���ν�Ƽ
        float currentHorizontalVelocity = new Vector3(CC.velocity.x, 0, CC.velocity.z).magnitude;

        //Ÿ�ٽ��ǵ忡�� ���ν�Ƽ���� ��� ���ġ
        float speedOffset = 0.1f;

        float inputMagnitude = player.ApplyAnalogMovement ? player._inputXZ.magnitude : 1f;

        //Ⱦ �ӵ� ����ġ���� ������ �����ؼ� ����
        if (currentHorizontalVelocity < targetSpeed - speedOffset ||
            currentHorizontalVelocity > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalVelocity, targetSpeed * inputMagnitude,
                Time.deltaTime * player.SpeedChangeRate);

            //�Ҽ��� 3�ڸ� �ݿø�
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }

        else
            speed = targetSpeed;

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * player.SpeedChangeRate);

        if (animationBlend < 0.01f)
            animationBlend = 0f;

        //��ǲ�ý��ۿ��� ����ȭ ������ ���� ��� ���ص� ����
        //Vector3 inputDirection = new Vector3(_inputXZ.x, 0, _inputXZ.y).normalized;
        player.inputDirection = new Vector3(player._inputXZ.x, 0, player._inputXZ.y).normalized;
        player.inputDirection = player.orientation.forward * player._inputXZ.y + player.orientation.right * player._inputXZ.x;

        if (player._inputXZ != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(player.inputDirection.x, player.inputDirection.z) * Mathf.Rad2Deg
                + player.VCamera.transform.eulerAngles.y;

            float rotation = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetRotation,
                ref rotationVelocity, player.RotatonSmoothTime);

            //ī�޶� ��ġ�� ����Ͽ� ȸ��, ���߿� �� ���¸� ȸ�� �Ұ�
            if (player.Can_Rotate)
                player.transform.rotation = Quaternion.Euler(0, rotation, 0);
        }


        targetDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;

        if (player.isGrounded && player.Can_MoveHorizontally && !LedgeMovement())
        {
            //  Debug.Log("Ÿ�� �𷺼� ������");
            CC.Move(targetDirection.normalized * (speed * Time.deltaTime)
                + new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
        }

        //���߿� �� ���¸� ���� �Ұ�
        else if (!player.isGrounded || !player.Can_MoveHorizontally)
        {
            //  Debug.Log("���ν�Ƽ ������");
            CC.Move(new Vector3(CC.velocity.x * 0.85f, 0, CC.velocity.z * 0.85f).normalized * (speed * Time.deltaTime)
                 + new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
        }


        anim.SetFloat(gameManager.animIDSpeed, animationBlend);
        anim.SetFloat(gameManager.animIDMotionSpeed, inputMagnitude);
    }

    //�𼭸������� ������
    protected bool LedgeMovement()
    {
        float angle = player.LedgeData.angle;

        if (player.isOnLedge && angle < 90)
        {
            CC.Move(Vector3.zero);
            return true;
        }
        return false;
    }
    //protected void Jump()
    //{
    //    if (player.isGrounded)
    //    {
    //        anim.SetBool(gameManager.animIDJump, false);

    //        if (player._inputJump && jumpTimeoutDelta <= 0.0f)
    //        {
    //            // ��Ʈ ���� * -2f * �߷� = ���ϴ� ���̱��� �����ϴ� ���ν�Ƽ��
    //            verticalVelocity = Mathf.Sqrt(player.jumpHeight * -2f * player.Gravity);
    //            anim.SetBool(gameManager.animIDJump, true);
    //        }

    //        // Ÿ�̸Ӱ� 0���� ũ�� Ÿ�̸� ���ҽ�Ŵ
    //        if (jumpTimeoutDelta >= 0f)
    //        {
    //            jumpTimeoutDelta -= Time.deltaTime;
    //        }
    //    }
    //    else
    //    {
    //        // Ȥ�� �𸣴� ���� �پ������� �ʱ�ȭ
    //        player._inputJump = false;
    //    }
    //}
    protected void ApplyGravity()
    {
        if (!player.isControlable)
            return;

        if (player.isGrounded)
        {
            // ���� Ÿ�̸� �ʱ�ȭ
            fallTimeoutDelta = player.FallTimeout;

            // ���� ���ν�Ƽ ������ �����ϴ� �� ����
            // ���� ���϶� velocity�� ũ�� �����Ǵ°� ����(�������� �����϶� �ް��� ����)
            // �������϶� ��ֹ� üũ�ؼ� �ް��� �ϴ°� ����
            if (verticalVelocity < 0.0f && !player.inParkourAction && GetType().Name == "PlayerJumpState")
                verticalVelocity = player.groundedGravity;
        }
        else
        {
            // Ÿ�̸� �ʱ�ȭ
            jumpTimeoutDelta = player.jumpTimeout;

            if (fallTimeoutDelta >= 0.0f)
                fallTimeoutDelta -= Time.deltaTime;
        }


        if (verticalVelocity < terminalVelocity)
            verticalVelocity += player.Gravity * player.mass * Time.deltaTime;

    }
    protected void whenLostControl()
    {
        if (!player.isControlable)
        {
            //���� �ʱ������ �ϴ°� ������ ���ֱ�
        }
    }
    #endregion
}
