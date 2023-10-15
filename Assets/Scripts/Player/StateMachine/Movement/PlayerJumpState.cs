using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerJumpState : PlayerAirborneState
{
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(gameManager.animIDJump, true);

        //�ּ� ���� ���� �ð�
        StateTimer = 0.2f;

        //jumpTimeout �ð��� ���� �Ŀ� �ٽ� ���� ����
        player.StartCoroutine("nowBusy", player.jumpTimeout);

        // ��Ʈ ���� * -2f * �߷� = ���ϴ� ���̱��� �����ϴ� ���ν�Ƽ��
        verticalVelocity = Mathf.Sqrt(player.jumpHeight * -2f * player.Gravity);
    }

    public override void Update()
    {
        base.Update();

        if (StateTimer > 0)
            player.Can_MoveHorizontally = false;


        if (!player.isGrounded && verticalVelocity <= 0f && !player.isBusy)
            stateMachine.ChangeState(player.fallingState);

        else if (player.isGrounded && StateTimer <= 0f)
            stateMachine.ChangeState(player.idleState);
    }
    public override void Exit()
    {
        base.Exit();

        //����Ű ������ ������ �ݹ鿡�� ��� �� �о�鿩�� �ݺ����� �Ǵ°� ����
        player._inputJump = false;
        anim.SetBool(gameManager.animIDJump, false);
    }
}
