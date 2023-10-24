using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;


//���� ���� ���� : player.climbPoint�� ����ĳ��Ʈ ��ġ�� ����ִµ� �����߿� �������� ������ �ȵǱ⶧��
public class PlayerHangingIdleWallState : PlayerHangingState
{
    public PlayerHangingIdleWallState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(gameManager.animIDParkour_HangingIdle, true);
        ControllableLedgeAction = true;
    }
    public override void Update()
    {
        base.Update();

        //����ĳ��Ʈ ������Ʈ �̸� �����
        //for (int i = 0; i < bodyPartHits.Count(); i++)
        //{
        //    Debug.Log("bodyPartHits : " + bodyPartHits[i].transform.gameObject.name);

        //}

        #region �𼭸��׼�
        //Ÿ�ٸ�Ī �̷����� ��ü������ ������Ʈ
        if (bodyPartHits.Count() > 0)
            player.climbPoint = bodyPartHits[0].transform.GetComponent<ClimbPoint>();

        //�Է� ������� ����
        neighbour = player.climbPoint?.GetNeighbour(new Vector2(Mathf.Round(player._inputXZ.x), Mathf.Round(player._inputXZ.y)));

        if (neighbour != null)
        {
            if (neighbour.connectionType == ConnectionType.Jump)
            {
                player.climbPoint = neighbour.climbpoint;


                ////�� �������� ����ĳ��Ʈ
                //RaycastHit LedgeToLedgeTopHit = LedgeToLedgeCheck();

                //this.LedgeToLedgeTopHit = LedgeToLedgeTopHit;

                //��Ÿ�ϸ� ���峪�� �����־ ���Ƶ�
                if (player._inputJump && StateTimer <= 0f)
                {
                    if (neighbour.direction.y == 1)
                        player.PerformParkourState(this.LedgeToLedgeTopHit.point, player.bracedHangHopUpState);

                    if (neighbour.direction.y == -1)
                        player.PerformParkourState(this.LedgeToLedgeTopHit.point, player.bracedHangHopDownState);

                    if (neighbour.direction.x == 1)
                        player.PerformParkourState(this.LedgeToLedgeTopHit.point, player.bracedHangHopRightState);

                    if (neighbour.direction.x == -1)
                        player.PerformParkourState(this.LedgeToLedgeTopHit.point, player.bracedHangHopLeftState);
                }
            }

            else if (neighbour.connectionType == ConnectionType.Move)
            {
                player.climbPoint = neighbour.climbpoint;

                ////�� �������� ����ĳ��Ʈ
                //RaycastHit LedgeToLedgeTopHit = LedgeToLedgeCheck();

                //this.LedgeToLedgeTopHit = LedgeToLedgeTopHit;

                
                if (neighbour.direction.x == 1 && bodypartXpositiveHitFound)
                    player.PerformParkourState(this.LedgeToLedgeTopHit.point, player.bracedHangShimmyRightState);

                if (neighbour.direction.x == -1 && bodypartXnegativeHitFound)
                    player.PerformParkourState(this.LedgeToLedgeTopHit.point, player.bracedHangShimmyLeftState);
            }
        }
        #endregion


        //������ �پ ���������� ������ ƨ�ܳ�����
        if (player._inputJump && player._inputXZ == Vector2.zero)
            stateMachine.ChangeState(player.jumpFromHangingWallState);
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool(gameManager.animIDParkour_HangingIdle, false);
    }

    //protected RaycastHit LedgeToLedgeCheck()
    //{
    //    Physics.Raycast(player.climbPoint.transform.position + player.climbPoint.transform.forward * gameManager.LedgeToLedgeFrontHitRayLength_forward,
    //    player.climbPoint.transform.forward * -gameManager.LedgeToLedgeFrontHitRayLength_forward,
    //    out RaycastHit LedgeToLedgeFrontHit, player.hangableLayer);

    //    Physics.Raycast(LedgeToLedgeFrontHit.point + Vector3.up * gameManager.LedgeToLedgeFrontHitRayLength_up,
    //    Vector3.down * gameManager.LedgeToLedgeFrontHitRayLength_up,
    //    out RaycastHit LedgeToLedgeTopHit, player.hangableLayer);

    //    return LedgeToLedgeTopHit;
    //}
}
