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
        anim.SetBool(player.animIDJump, true);

        StateTimer = player.jumpTimeout;

        // 루트 높이 * -2f * 중력 = 원하는 높이까지 도달하는 벨로시티값
        verticalVelocity = Mathf.Sqrt(player.jumpHeight * -2f * player.Gravity);
    }

    public override void Update()
    {
        base.Update();


        if (!player.isGrounded && verticalVelocity <= 0f && StateTimer <= 0f)
            stateMachine.ChangeState(player.fallingState);
    }
    public override void Exit()
    {
        anim.SetBool(player.animIDJump, false);
        base.Exit();
    }

}
