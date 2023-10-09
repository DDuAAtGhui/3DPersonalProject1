using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParkourState : PlayerStates
{
    public PlayerParkourState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.applyRootMotion = true;
        anim.SetBool(player.animIDParkouring, true);
    }
    public override void Update()
    {
        base.Update();

        if (isAnimEnd)
            stateMachine.ChangeState(player.idleState);
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool(player.animIDParkouring, false);
    }


}
