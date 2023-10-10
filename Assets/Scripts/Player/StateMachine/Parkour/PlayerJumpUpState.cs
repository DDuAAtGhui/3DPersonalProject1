using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpUpState : PlayerParkourState
{
    public PlayerJumpUpState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(player.animIDParkour_JumpUp, true);
        player.SetControllable(false);
    }
    public override void Update()
    {
        base.Update();

    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool(player.animIDParkour_JumpUp, false);
        player.SetControllable(true);
    }
}
