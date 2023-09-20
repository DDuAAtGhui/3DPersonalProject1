using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, string AnimationName) : base(player, stateMachine, AnimationName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void FixedUpadate()
    {
        base.FixedUpadate();
    }

    public override void Upadate()
    {
        base.Upadate();
    }
    public override void Exit()
    {
        base.Exit();
    }

}
