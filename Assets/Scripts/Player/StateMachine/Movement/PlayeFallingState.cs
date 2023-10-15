using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Grounded, Jump ���¿��� Falling���� ������
public class PlayeFallingState : PlayerAirborneState
{
    public PlayeFallingState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(gameManager.animIDFreeFall, true);

        if (groundToFallState)
            StateTimer = player.CoyoteTime;

        //���ο��� �� �پ����� �� �ְ� GroundedGravity�� ũ�� �����س��� ������
        //�� ���� ������ ä �߰��� ���ӵ� �����ʰ� 0���� �ʱ�ȭ
        verticalVelocity = 0;

        //fallTimeoutDelta �ð� ���� �Ŀ� �ٽ� FallingState ���� ����
        player.StartCoroutine("nowBusy", fallTimeoutDelta);
    }
    public override void Update()
    {
        base.Update();


        fallingTimer += Time.deltaTime;

        if (player.isGrounded)
            stateMachine.ChangeState(player.landingState);


        else if (player._inputJump && StateTimer >= 0f)
            stateMachine.ChangeState(player.jumpState);
    }
    public override void Exit()
    {
        base.Exit();
        //�� ���� �ð� ��ȯ
        player.totalFallingTime = fallingTimer;
        fallingTimer = 0;
        anim.SetBool(gameManager.animIDFreeFall, false);

        groundToFallState = false;
        parkourToFallState = false;
    }
}
