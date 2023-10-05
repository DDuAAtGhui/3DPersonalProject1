using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirborneState : PlayerStates
{
    protected float fallingTimer = 0f;
    public PlayerAirborneState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
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
