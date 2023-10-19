using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHangingIdleWallState : PlayerHangingState
{
    public PlayerHangingIdleWallState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(gameManager.animIDParkour_HangingIdle, true);
    }
    public override void Update()
    {
        base.Update();

        if (player._inputJump && player._inputXZ == Vector2.zero)
            stateMachine.ChangeState(player.jumpFromHangingWallState);

        if (player._inputJump && player._inputXZ != Vector2.zero)
            stateMachine.ChangeState(player.bracedHangHopState);
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool(gameManager.animIDParkour_HangingIdle, false);
    }
}
