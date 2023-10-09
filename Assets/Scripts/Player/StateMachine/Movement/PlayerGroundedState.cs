using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerStates
{
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        base.Update();


        if (speed <= 0.1f && player._inputXZ == Vector2.zero)
            stateMachine.ChangeState(player.idleState);

        if (speed > 0.1f && speed <= 7f)
            stateMachine.ChangeState(player.walkState);

        if (speed > 7f)
            stateMachine.ChangeState(player.runState);

        // PlayerFallingAnimation : �ָ��� ���̿��� ������ �� �ִϸ��̼� �̻��ϰ� �����°� ����
        if (!player.isGrounded && verticalVelocity <= 0f && !player.isBusy && player.PlayFallingAnimation)
            stateMachine.ChangeState(player.fallingState);

        //���ڸ� ������ Groundüũ�� ��¦ �Ǳ⶧���� �׶� ������ Veloicty���� ����޾� �����̴°� ����
        if (player._inputJump && !player.isBusy && player._inputXZ == Vector2.zero)
        {
            player.Stop();
            stateMachine.ChangeState(player.jumpState);
        }

        //�����̴ٰ� ����
        if (player._inputJump && !player.isBusy && player._inputXZ != Vector2.zero)
            stateMachine.ChangeState(player.jumpState);
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }
    public override void Exit()
    {
        base.Exit();
        player.horizontalStop = false;
    }
    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }

    public override void OnCollisionStay(Collision collision)
    {
        base.OnCollisionStay(collision);
    }
    #region �޼ҵ��
    #endregion
}
