using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStepUpState : PlayerStates
{
    public PlayerStepUpState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(player.animIDParkour_StepUp, true);
    }
    public override void Update()
    {
        base.Update();
    }
    public override void Exit()
    {
        anim.SetBool(player.animIDParkour_StepUp, false);
        base.Exit();
    }
}
