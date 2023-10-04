using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAirborneState
{
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        StateTimer = player.jumpTimeout;
    }

    public override void Update()
    {
        base.Update();

        anim.SetBool(player.animIDJump, true);
        // 루트 높이 * -2f * 중력 = 원하는 높이까지 도달하는 벨로시티값
        verticalVelocity = Mathf.Sqrt(player.jumpHeight * -2f * player.Gravity);

        if (!player.isGrounded && verticalVelocity <= 0f)
            stateMachine.ChangeState(player.fallingState);
    }
    public override void Exit()
    {
        base.Exit();
    }

}
