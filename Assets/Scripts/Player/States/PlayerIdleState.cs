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
        player.SetVelocityToZero();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();
        if (X_Input != 0 || Y_Input != 0)
            stateMachine.ChangeState(player.runState);
    }
    public override void Exit()
    {
        base.Exit();
    }

}
