using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBracedHangHopRightState : PlayerHangingState
{
    public PlayerBracedHangHopRightState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(gameManager.animIDParkour_BracedHangHopRight, true);
        ControllableLedgeAction = false;

        player.StartCoroutine("nowBusy", 0.7f);

        player.transform.rotation = Quaternion.LookRotation(-player.climbPoint.transform.forward);
    }

    public override void Update()
    {
        base.Update();
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool(gameManager.animIDParkour_BracedHangHopRight, false);

    }

}
