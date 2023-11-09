using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//기본으로 약한 착지모션 존재
public class PlayerStandJumpingDownState : PlayerParkourState
{
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

        if (player.LedgeData.height > player.ledgeCheckHeightStandard_Bottom)
            anim.SetLayerWeight(anim.GetLayerIndex("Upper Layer"),
            Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("Upper Layer")), 1, 15 * Time.deltaTime));



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
        anim.SetBool(gameManager.animIDParkour_StandJumpingDown, false);
    }

}
