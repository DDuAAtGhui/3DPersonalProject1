using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //idle�϶� ���ν�Ƽ �ʱ�ȭ
        if (!parkourToFallState)
            CC.Move(Vector3.zero);

    }
    public override void Update()
    {
        base.Update();

        //�Ŵ޸��°� �켱�ǰ�
        if (player._inputJump && !player.isHangable && player.hitData.forwardHitFound)
            player.PerformParkourState(Vector3.zero,player.stepUpState, player.jumpUpState, player.crouchToClimbUpState,
            player.jumpOver_RollState);

        if (player._inputJump && !player.isHangable && player.isOnLedge && !player.hitData.forwardHitFound)
            player.PerformParkourState(Vector3.zero, player.standjumpingDownState);

        if (player._inputJump && player.isHangable)
            player.PerformParkourState(Vector3.zero, player.idleToHangWallState);
    }
    public override void Exit()
    {
        base.Exit();

    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }

}
