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
        anim.SetBool(player.animIDParkour_StepUp, true);
        player.SetControllable(false);
    }
    public override void Update()
    {
        base.Update();
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool(player.animIDParkour_StepUp, false);
        player.SetControllable(true);
    }
}
