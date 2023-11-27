using System;
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
        LedgeToLedgeTopHit = LedgeToLedgeCheck();


        //�� ũ�� Ŀ������ Ű �Է��Ҷ��� ���̰�����
        if (gameManager.Visible_MatchPosition && player._inputXZ != Vector2.zero)
            HangableNetworkSphereObject.SetActive(true);

        else if (gameManager.Visible_MatchPosition || player._inputXZ != Vector2.zero)
            HangableNetworkSphereObject.SetActive(false);

        if (isAnimEnd)
            stateMachine.ChangeState(player.hangingIdleWallState);



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

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(LedgeToLedgeFrontHit.point, 0.05f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(LedgeToLedgeTopHit.point, 0.05f);

    }

    protected RaycastHit LedgeToLedgeCheck()
    {
        //bool frontHit = Physics.Raycast(player.climbPoint.transform.position +
        //    player.climbPoint.transform.forward * gameManager.LedgeToLedgeFrontHitRayLength_forward,
        //player.climbPoint.transform.forward * -gameManager.LedgeToLedgeFrontHitRayLength_forward,
        //out RaycastHit frontHitResult, player.hangableLayer);

        bool frontHit = Physics.Raycast(player.climbPoint.transform.position +
            player.climbPoint.transform.forward * gameManager.LedgeToLedgeFrontHitRayLength_forward,
            -player.climbPoint.transform.forward, out RaycastHit frontHitResult,
            gameManager.LedgeToLedgeFrontHitRayLength_forward, player.hangableLayer);

        LedgeToLedgeFrontHit = frontHitResult;

        Debug.DrawRay(player.climbPoint.transform.position + player.climbPoint.transform.forward * gameManager.LedgeToLedgeFrontHitRayLength_forward,
            -player.climbPoint.transform.forward * gameManager.LedgeToLedgeFrontHitRayLength_forward,
            frontHit ? Color.green : Color.red);

        ////�׽�Ʈ
        //Physics.Raycast(player.climbPoint.transform.position + player.climbPoint.transform.forward * 10f,
        //    -player.climbPoint.transform.forward, out RaycastHit test1, 10f, player.hangableLayer);
        //Debug.DrawRay(player.climbPoint.transform.position + player.climbPoint.transform.forward * 10f, player.climbPoint.transform.forward * -10f, Color.yellow);

        //bool isTest2 = Physics.Raycast(test1.point + test1.transform.up * 2f - test1.transform.forward * 0.15f, -test1.transform.up, out RaycastHit test2, 2f, player.hangableLayer);
        //Debug.DrawRay(test1.point + test1.transform.up * 2f - test1.transform.forward * 0.15f, -test1.transform.up * 2f, isTest2 ? Color.yellow : Color.blue);

        //Debug.Log("test2 : " + test2.collider.name + $" {test2.point}");

        //bool topHit = Physics.Raycast(LedgeToLedgeFrontHit.point + Vector3.up * gameManager.LedgeToLedgeFrontHitRayLength_up,
        //Vector3.down * gameManager.LedgeToLedgeFrontHitRayLength_up,
        //out RaycastHit topHitResult, player.hangableLayer);    

        bool topHit = Physics.Raycast(LedgeToLedgeFrontHit.point - LedgeToLedgeFrontHit.transform.forward * 0.15f
            + LedgeToLedgeFrontHit.transform.up * gameManager.LedgeToLedgeFrontHitRayLength_up
            , -LedgeToLedgeFrontHit.transform.up, out RaycastHit topHitResult, gameManager.LedgeToLedgeFrontHitRayLength_up, player.hangableLayer);

        //Debug.Log("Layer Mask Value: " + player.hangableLayer.value);

        try
        {
            Debug.Log("frontHitResult ������Ʈ �̸� : " + frontHitResult.collider.gameObject.name);
            Debug.Log("topHitResult ������Ʈ �̸� : " + topHitResult.collider.gameObject.name);

            Debug.DrawRay(topHitResult.point, topHitResult.transform.forward * 100f, Color.yellow);
            Debug.DrawRay(LedgeToLedgeFrontHit.point - LedgeToLedgeFrontHit.transform.forward * 0.15f 
                + LedgeToLedgeFrontHit.transform.up * gameManager.LedgeToLedgeFrontHitRayLength_up,
                -LedgeToLedgeFrontHit.transform.up * gameManager.LedgeToLedgeFrontHitRayLength_up, topHit ? Color.green : Color.red);
        }

        catch (Exception e)
        {
            Debug.Log(e);
        }
        return topHitResult;
    }
}
