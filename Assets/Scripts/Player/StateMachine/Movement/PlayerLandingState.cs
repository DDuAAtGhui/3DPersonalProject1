using System.Collections;
using System.Collections.Generic;
using Unity.Android.Types;
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

        if (player.totalFallingTime >= 0.5f && player._inputXZ != Vector2.zero)
        {
            player.Can_MoveHorizontally = false;
            player.Can_Rotate = false;
            anim.SetBool(gameManager.animIDLanding_Roll, true);
            roll = true;
        }

        else if (player.totalFallingTime >= 0.5f && player._inputXZ == Vector2.zero)
        {
            player.isHorizontalStop(true);
            player.Can_MoveHorizontally = false;
            player.Can_Rotate = false;
            anim.SetBool(gameManager.animIDLanding_Hard, true);
            hard = true;
        }


        else if (player.totalFallingTime < 0.5f)
        {
            anim.SetBool(gameManager.animIDLanding_Small, true);
            small = true;
        }
    }
    public override void Update()
    {
        base.Update();

        if (player._inputXZ != Vector2.zero && isAnimEnd)
            stateMachine.ChangeState(player.idleState);

        else if (player._inputXZ != Vector2.zero && small)
            stateMachine.ChangeState(player.idleState);

        //���ڸ� ������ Groundüũ�� ��¦ �Ǳ⶧���� �׶� ������ Veloicty���� ����޾� �����̴°� ����
        if (player._inputJump && !player.isBusy && player._inputXZ == Vector2.zero)
        {
            player.isHorizontalStop(true) ;
            stateMachine.ChangeState(player.jumpState);
        }

        //�����̴ٰ� ����
        if (player._inputJump && !player.isBusy && player._inputXZ != Vector2.zero)
            stateMachine.ChangeState(player.jumpState);
    }

    public override void Exit()
    {
        player._inputWalk = false;
        player.isHorizontalStop(true);
        player.Can_MoveHorizontally = true;
        player.Can_Rotate = true;
        roll = false;
        hard = false;
        small = false;
        anim.SetBool(gameManager.animIDLanding_Roll, false);
        anim.SetBool(gameManager.animIDLanding_Hard, false);
        anim.SetBool(gameManager.animIDLanding_Small, false);
        base.Exit();
    }
}
