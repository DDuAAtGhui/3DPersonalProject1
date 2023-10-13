using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchToClimbUpState : PlayerParkourState
{
    public PlayerCrouchToClimbUpState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(player.animIDParkour_CrouchToClimbUp, true);
    }
    public override void Update()
    {
        base.Update();
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool(player.animIDParkour_CrouchToClimbUp, false);
    }

}
