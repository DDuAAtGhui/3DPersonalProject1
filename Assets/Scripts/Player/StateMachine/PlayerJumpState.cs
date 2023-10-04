using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAirborneState
{
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        StateTimer = player.jumpTimeout;
    }

    public override void Update()
    {
        base.Update();

        anim.SetBool(player.animIDJump, true);
        // ��Ʈ ���� * -2f * �߷� = ���ϴ� ���̱��� �����ϴ� ���ν�Ƽ��
        verticalVelocity = Mathf.Sqrt(player.jumpHeight * -2f * player.Gravity);

        if (!player.isGrounded && verticalVelocity <= 0f)
            stateMachine.ChangeState(player.fallingState);
    }
    public override void Exit()
    {
        base.Exit();
    }

}
