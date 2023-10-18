using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleToHangWallState : PlayerParkourState
{
    public PlayerIdleToHangWallState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(gameManager.animIDParkour_IdleToHang, true);
    }
    public override void Update()
    {
        base.Update();

        if (isAnimEnd)
            stateMachine.ChangeState(player.hangingIdleWallState);
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool(gameManager.animIDParkour_IdleToHang, false);

    }

}
