using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStepUpState : PlayerParkourState
{
    public PlayerStepUpState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(gameManager.animIDParkour_StepUp, true);
    }
    public override void Update()
    {
        base.Update();
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool(gameManager.animIDParkour_StepUp, false);
    }
}
