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
        CC.Move(Vector3.zero);
    }
    public override void Update()
    {
        base.Update();

        if (player._inputJump && player.hitData.forwardHitFound)
            player.PerformParkourState(player.stepUpState, player.jumpUpState, player.crouchToClimbUpState,
                player.jumpOver_RollState);

        if (player._inputJump && player.isOnLedge)
            player.PerformParkourState(player.standjumpingDownState);
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
