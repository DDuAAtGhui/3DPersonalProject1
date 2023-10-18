using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHangingIdleWallState : PlayerParkourState
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

        if (player._inputJump)
            stateMachine.ChangeState(player.jumpFromHangingWallState);
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool(gameManager.animIDParkour_HangingIdle, false);
    }
}
