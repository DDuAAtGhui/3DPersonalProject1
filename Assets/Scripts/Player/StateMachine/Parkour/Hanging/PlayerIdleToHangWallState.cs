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

        //뭔가 개 쓰레기같은 버그로 이거 없으면 모서리로 점프할때 장애물 바라보는거 적용안됨
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
