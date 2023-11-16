using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirborneState : PlayerStates
{
    public PlayerAirborneState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        player.can_MoveHorizontally = false;
        player.can_Rotate = false;
    }
    public override void Update()
    {
        base.Update();

    }
    public override void Exit()
    {
        base.Exit();
        player.can_MoveHorizontally = true;
        player.can_Rotate = true;
    }
}
