using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpFromHangingWallState : PlayerHangingState
{
    public PlayerJumpFromHangingWallState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(gameManager.animIDParkour_JumpFromHangingWall, true);
        parkourToFallState = true;

    }
    public override void Update()
    {
        base.Update();

        if (isAnimEnd)
            stateMachine.ChangeState(player.fallingState);

        if (player.isGrounded)
            stateMachine.ChangeState(player.landingState);
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool(gameManager.animIDParkour_JumpFromHangingWall, false);
    }

}
