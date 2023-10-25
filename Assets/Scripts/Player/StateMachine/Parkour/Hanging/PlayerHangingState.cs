using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

//�Ŵ޸� ���¿��� �ٸ� Ledge ����
public class PlayerHangingState : PlayerParkourState
{
    protected RaycastHit LedgeToLedgeFrontHit;
    protected RaycastHit LedgeToLedgeTopHit;
    protected bool ControllableLedgeAction; //�� ���¿��� Ű �Է��ϰ� �ٸ� �𼭸� �׼����� �Ѿ�� ����

    protected bool isShimmy;
    public PlayerHangingState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.isHanging = true;

        StateTimer = 0.2f;
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


        RaycastHit LedgeToLedgeTopHit = LedgeToLedgeCheck();

        this.LedgeToLedgeTopHit = LedgeToLedgeTopHit;


        if (player.climbPoint != null && gameManager.Log_HangingInfo)
        {
            Debug.Log($"{GetType().Name}'s climbPoint : " + player.climbPoint);
            Debug.Log($"{GetType().Name}'s LedgeToLedgeTopHit pos : " + LedgeToLedgeTopHit.point);
        }
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

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(LedgeToLedgeFrontHit.point, 0.05f);
    }

    protected RaycastHit LedgeToLedgeCheck()
    {
        Physics.Raycast(player.climbPoint.transform.position + player.climbPoint.transform.forward * gameManager.LedgeToLedgeFrontHitRayLength_forward,
        player.climbPoint.transform.forward * -gameManager.LedgeToLedgeFrontHitRayLength_forward,
        out RaycastHit LedgeToLedgeFrontHit, player.hangableLayer);

        this.LedgeToLedgeFrontHit = LedgeToLedgeFrontHit;

        Physics.Raycast(LedgeToLedgeFrontHit.point + Vector3.up * gameManager.LedgeToLedgeFrontHitRayLength_up,
        Vector3.down * gameManager.LedgeToLedgeFrontHitRayLength_up,
        out RaycastHit LedgeToLedgeTopHit, player.hangableLayer);

        return LedgeToLedgeTopHit;
    }
}
