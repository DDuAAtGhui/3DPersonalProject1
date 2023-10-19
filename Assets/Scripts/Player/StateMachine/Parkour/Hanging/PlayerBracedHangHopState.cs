using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBracedHangHopState : PlayerHangingState
{
    public PlayerBracedHangHopState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();


        if (neighbour != null)
        {
            if (neighbour.direction.y == 1)
                anim.SetBool(gameManager.animIDParkour_BracedHangHopUp, true);

            if (neighbour.direction.x == 1)
                anim.SetBool(gameManager.animIDParkour_BracedHangHopRight, true);
        }
    }
    public override void Update()
    {
        base.Update();
    }
    public override void Exit()
    {
        base.Exit();

        anim.SetBool(gameManager.animIDParkour_BracedHangHopUp, false);
        anim.SetBool(gameManager.animIDParkour_BracedHangHopRight, false);

    }


}
