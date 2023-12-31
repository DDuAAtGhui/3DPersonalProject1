using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBracedHangHopUpState : PlayerHangingState
{
    public PlayerBracedHangHopUpState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(gameManager.animIDParkour_BracedHangHopUp, true);
        ControllableLedgeAction = false;

        player.StartCoroutine("nowBusy", 0.7f);
    }
    public override void Update()
    {
        base.Update();
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool(gameManager.animIDParkour_BracedHangHopUp, false);
    }


}
