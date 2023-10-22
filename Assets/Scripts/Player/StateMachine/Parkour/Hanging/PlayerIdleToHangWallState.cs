using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleToHangWallState : PlayerParkourState
{
    public PlayerIdleToHangWallState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(gameManager.animIDParkour_IdleToHang, true);

        player.climbPoint = player.hangableData.HangableHit.transform.GetComponent<ClimbPoint>();

        climbPointAtEnter = player.climbPoint;

        neighbour = player.climbPoint.GetNeighbour(Vector2.zero);
    }
    public override void Update()
    {
        base.Update();

        //���� �� �����ⰰ�� ���׷� �̰� ������ �𼭸��� �����Ҷ� ��ֹ� �ٶ󺸴°� ����ȵ�
        player.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (isAnimEnd)
            stateMachine.ChangeState(player.hangingIdleWallState);
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool(gameManager.animIDParkour_IdleToHang, false);

    }

}
