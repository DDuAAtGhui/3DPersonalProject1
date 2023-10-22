using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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

        player.StartCoroutine("nowBusy", 0.2f);
    }
    public override void Update()
    {
        base.Update();

        ////����Ű ������ ����
        //if (player._inputJump)
        //    player.climbPoint = player.hangableData.HangableHit.transform.GetComponent<ClimbPoint>();


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


                //�� �������� ����ĳ��Ʈ
                Physics.Raycast(player.climbPoint.transform.position + player.climbPoint.transform.forward * gameManager.LedgeToLedgeFrontHitRayLength_forward,
                    player.climbPoint.transform.forward * -gameManager.LedgeToLedgeFrontHitRayLength_forward,
                    out RaycastHit LedgeToLedgeFrontHit, player.hangableLayer);

                Physics.Raycast(LedgeToLedgeFrontHit.point + Vector3.up * gameManager.LedgeToLedgeFrontHitRayLength_up,
                    Vector3.down * gameManager.LedgeToLedgeFrontHitRayLength_up,
                    out RaycastHit LedgeToLedgeTopHit, player.hangableLayer);

                this.LedgeToLedgeTopHit = LedgeToLedgeTopHit;
            }

            if (neighbour.connectionType == ConnectionType.Move)
            {

            }

            if (player._inputXZ != Vector2.zero)
                HangableNetworkSphereObject.transform.position = player.climbPoint.transform.position;

            else
                HangableNetworkSphereObject.transform.position = climbPointAtEnter.transform.position;

        }


        if (neighbour != null)
        {
            if (player._inputJump & !player.isBusy) //��Ÿ�ϸ� ���峪�� �����־ ���Ƶ�
            {
                if (neighbour.direction.y == 1)
                    player.PerformParkourState(LedgeToLedgeTopHit.point, player.PlayerBracedHangHopUpState);

                if (neighbour.direction.y == -1)
                    player.PerformParkourState(LedgeToLedgeTopHit.point, player.PlayerBracedHangHopDownState);

                Debug.Log("HangableHIt name : " + player.hangableData.HangableHit.transform.name);
                Debug.Log("���� �Է� �� neighbour : " + neighbour + "\t Direction : " + neighbour.direction);
            }
        }

        //������ �پ ���������� ������ ƨ�ܳ�����
        if (player._inputJump && player._inputXZ == Vector2.zero)
            stateMachine.ChangeState(player.jumpFromHangingWallState);


    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool(gameManager.animIDParkour_HangingIdle, false);
    }
}
