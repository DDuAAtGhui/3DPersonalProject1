using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

//�Ŵ޸� ���¿��� �ٸ� Ledge ����
public class PlayerHangingState : PlayerParkourState
{
    protected RaycastHit LedgeToLedgeTopHit;
    protected bool ControllableLedgeAction; //�� ���¿��� Ű �Է��ϰ� �ٸ� �𼭸� �׼����� �Ѿ�� ����
    public PlayerHangingState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.isHanging = true;

        StateTimer = 0.4f;
    }

    public override void Update()
    {
        base.Update();

        //�� ũ�� Ŀ������ Ű �Է��Ҷ��� ���̰�����
        if (gameManager.Visible_MatchPosition && player._inputXZ != Vector2.zero)
            HangableNetworkSphereObject.SetActive(true);

        else if (gameManager.Visible_MatchPosition || player._inputXZ != Vector2.zero)
            HangableNetworkSphereObject.SetActive(false);

        if (isAnimEnd)
            stateMachine.ChangeState(player.hangingIdleWallState);



    }
    public override void Exit()
    {
        base.Exit();

        player.isHanging = false;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(LedgeToLedgeTopHit.point, 0.05f);
    }
}
