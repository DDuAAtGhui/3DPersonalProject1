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
        player.Can_MoveHorizontally = false;
        player.Can_Rotate = false;
    }
    public override void Update()
    {
        base.Update();

    }
    public override void Exit()
    {
        base.Exit();
        player.Can_MoveHorizontally = true;
        player.Can_Rotate = true;
    }
}
