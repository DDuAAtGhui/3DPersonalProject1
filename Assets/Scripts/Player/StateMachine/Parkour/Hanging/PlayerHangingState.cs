using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�Ŵ޸� ���¿��� �ٸ� Ledge ����
public class PlayerHangingState : PlayerParkourState
{
    protected RaycastHit LedgeToLedgeTopHit;
    public PlayerHangingState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.isHanging = true;

        Debug.Log(player.climbPoint);
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
