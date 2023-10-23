using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBracedHangShimmyState : PlayerHangingState
{
    public PlayerBracedHangShimmyState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        ControllableLedgeAction = true;
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
