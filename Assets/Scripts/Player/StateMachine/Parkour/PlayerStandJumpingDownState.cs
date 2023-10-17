using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//기본으로 약한 착지모션 존재
public class PlayerStandJumpingDownState : PlayerParkourState
{
    float t = 0f;
    public PlayerStandJumpingDownState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(gameManager.animIDParkour_StandJumpingDown, true);
        parkourToFallState = true;

        verticalVelocity = 0f;
    }
    public override void Update()
    {
        base.Update();

        player.Can_MoveHorizontally = false;

        t = Mathf.MoveTowards(0f, 1f, Time.deltaTime * 40f);

        if (player.LedgeData.height > player.ledgeCheckHeightStandard_Bottom)
            anim.SetLayerWeight(anim.GetLayerIndex("Upper Layer"),
            Mathf.MoveTowards(0, 1, t));



        if (isAnimEnd)
        {

            if (player.isGrounded)
                stateMachine.ChangeState(player.idleState);

            else
                stateMachine.ChangeState(player.fallingState);
        }
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetLayerWeight(anim.GetLayerIndex("Upper Layer"), 0);
        anim.SetBool(gameManager.animIDParkour_StandJumpingDown, false);
    }

}
