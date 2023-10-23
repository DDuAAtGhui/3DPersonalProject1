using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBracedHangHopDownState : PlayerHangingState
{
    public PlayerBracedHangHopDownState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(gameManager.animIDParkour_BracedHangHopDown, true);
        ControllableLedgeAction = false;

        player.StartCoroutine("nowBusy", 0.7f);

    }
    public override void Update()
    {
        base.Update();

        if (player.isGrounded) { }
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool(gameManager.animIDParkour_BracedHangHopDown, false);
    }
}
