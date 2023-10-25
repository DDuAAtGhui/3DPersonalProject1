using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBracedHangToCrouchState : PlayerHangingState
{
    public PlayerBracedHangToCrouchState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(gameManager.animIDParkour_BracedHangToCrouch, true);
    }
    public override void Update()
    {
        base.Update();

        if (isAnimEnd)
            stateMachine.ChangeState(player.idleState);
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool(gameManager.animIDParkour_BracedHangToCrouch, false);
    }
}
