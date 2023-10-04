using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayeFallingState : PlayerAirborneState
{
    public PlayeFallingState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        StateTimer = player.FallTimeout;
    }
    public override void Update()
    {
        base.Update();
    }
    public override void Exit()
    {
        base.Exit();
    }
}
