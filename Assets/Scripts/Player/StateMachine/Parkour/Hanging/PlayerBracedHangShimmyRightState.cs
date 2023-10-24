using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBracedHangShimmyRightState : PlayerBracedHangShimmyState
{
    public PlayerBracedHangShimmyRightState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(gameManager.animIDParkour_BracedHangShimmyRight, true);
    }
    public override void Update()
    {
        base.Update();
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool(gameManager.animIDParkour_BracedHangShimmyRight, false);

    }


}
