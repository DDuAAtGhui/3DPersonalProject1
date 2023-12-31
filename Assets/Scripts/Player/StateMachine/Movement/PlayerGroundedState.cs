using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerGroundedState : PlayerStates
{
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        base.Update();
        // anim.applyRootMotion = false;

        //속도가 떨어지거나 입력없으면 Idle
        if (speed <= 0.1f || player._inputXZ == Vector2.zero)
            stateMachine.ChangeState(player.idleState);

        if (speed > 0.1f && speed <= 7f)
            stateMachine.ChangeState(player.walkState);

        if (speed > 7f)
            stateMachine.ChangeState(player.runState);

        // PlayerFallingAnimation : 애매한 높이에서 떨어질 때 애니메이션 이상하게 나오는거 수정
        if (!player.isGrounded && verticalVelocity <= 0f && !player.isBusy && player.PlayFallingAnimation)
        {
            groundToFallState = true;
            stateMachine.ChangeState(player.fallingState);
        }

        //제자리 점프시 Ground체크가 살짝 되기때문에 그때 유지된 Veloicty값에 영향받아 움직이는거 방지
        if (player._inputJump && !player.isBusy && player._inputXZ == Vector2.zero)
        {
            player.horizontalStop = true;
            stateMachine.ChangeState(player.jumpState);
        }

        //움직이다가 점프시 속도 유지
        if (player._inputJump && !player.isBusy && player._inputXZ != Vector2.zero)
            stateMachine.ChangeState(player.jumpState);

        ////각도 제한등의 파쿠르 액션 조건은 ParkourAction에 있으니 기본적인 bool값만 체크해주기가 가능
        ///모서리에서 움직이면 내려가는 애니메이션 사용할거면 사용
        if (shouldJumpDown)
            player.PerformParkourState(Vector3.zero, player.standjumpingDownState);


    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }
    public override void Exit()
    {
        base.Exit();
        player.horizontalStop = false;
    }
    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }

    public override void OnCollisionStay(Collision collision)
    {
        base.OnCollisionStay(collision);
    }
    #region 메소드들
    #endregion
}
