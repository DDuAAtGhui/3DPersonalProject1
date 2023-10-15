using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStandJumpingDownState : PlayerParkourState
{
    public PlayerStandJumpingDownState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(gameManager.animIDParkour_StandJumpingDown, true);
        parkourToFallState = true;

    }
    public override void Update()
    {
        base.Update();

        player.Can_MoveHorizontally = false;

        if (isAnimEnd)
            stateMachine.ChangeState(player.landingState);



    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool(gameManager.animIDParkour_StandJumpingDown, false);
    }

}
