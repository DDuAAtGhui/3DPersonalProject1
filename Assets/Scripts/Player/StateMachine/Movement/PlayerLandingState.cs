using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandingState : PlayerStates
{
    bool small, roll, hard;
    public PlayerLandingState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //착지상태에서는 ground체크 안하려고 변수 생성
        //Locomotion이 기본전환상태라 이거 안하면 착지모션 안나옴
        anim.SetBool(gameManager.animIDLanding, true);

        if (player.totalFallingTime >= player.landingMotionTimerStandard && player._inputXZ != Vector2.zero)
        {
            player.can_MoveHorizontally = false;
            player.can_Rotate = false;
            anim.SetBool(gameManager.animIDLanding_Roll, true);
            roll = true;
        }

        else if (player.totalFallingTime >= player.landingMotionTimerStandard && player._inputXZ == Vector2.zero)
        {
            player.horizontalStop = true;
            player.can_MoveHorizontally = false;
            player.can_Rotate = false;
            anim.SetBool(gameManager.animIDLanding_Hard, true);
            hard = true;
        }


        else if (player.totalFallingTime < player.landingMotionTimerStandard)
        {
            anim.SetBool(gameManager.animIDLanding_Small, true);
            small = true;
        }

        //if (verticalVelocity <= player.landingMotionVelocityStandard && player._inputXZ != Vector2.zero)
        //{
        //    player.Can_MoveHorizontally = false;
        //    player.Can_Rotate = false;
        //    anim.SetBool(gameManager.animIDLanding_Roll, true);
        //    roll = true;
        //}

        //else if (verticalVelocity <= player.landingMotionVelocityStandard && player._inputXZ == Vector2.zero)
        //{
        //    player.horizontalStop = true;
        //    player.Can_MoveHorizontally = false;
        //    player.Can_Rotate = false;
        //    anim.SetBool(gameManager.animIDLanding_Hard, true);
        //    hard = true;
        //}


        //else if (verticalVelocity > player.landingMotionVelocityStandard)
        //{
        //    anim.SetBool(gameManager.animIDLanding_Small, true);
        //    small = true;
        //}

        Debug.Log("착지 낙하시간 판정 : " + player.totalFallingTime);
      //  Debug.Log("착지 벨로시티 판정 : " + CC.velocity.y);
    }
    public override void Update()
    {
        base.Update();

        if (player._inputXZ != Vector2.zero && isAnimEnd)
            stateMachine.ChangeState(player.idleState);

        else if (player._inputXZ != Vector2.zero && small)
            stateMachine.ChangeState(player.idleState);

        //제자리 점프시 Ground체크가 살짝 되기때문에 그때 유지된 Veloicty값에 영향받아 움직이는거 방지
        if (player._inputJump && !player.isBusy && player._inputXZ == Vector2.zero)
        {
            player.horizontalStop = true;
            stateMachine.ChangeState(player.jumpState);
        }

        //움직이다가 점프
        if (player._inputJump && !player.isBusy && player._inputXZ != Vector2.zero)
            stateMachine.ChangeState(player.jumpState);
    }

    public override void Exit()
    {
        player._inputWalk = false;
        player.horizontalStop = true;
        player.can_MoveHorizontally = true;
        player.can_Rotate = true;
        player.totalFallingTime = 0f;
        roll = false;
        hard = false;
        small = false;
        anim.SetBool(gameManager.animIDLanding_Roll, false);
        anim.SetBool(gameManager.animIDLanding_Hard, false);
        anim.SetBool(gameManager.animIDLanding_Small, false);
        anim.SetBool(gameManager.animIDLanding, false);
        base.Exit();
    }
}
