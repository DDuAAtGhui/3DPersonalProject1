using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerGroundState
{
    public PlayerWalkState(Player player, PlayerStateMachine stateMachine, string AnimationName) : base(player, stateMachine, AnimationName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
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
