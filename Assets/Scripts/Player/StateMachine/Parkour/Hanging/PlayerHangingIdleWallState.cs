using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//현재 버그 원인 : player.climbPoint를 레이캐스트 위치로 잡고있는데 파쿠르중엔 꺼놨으니 갱신이 안되기때문
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

        ////점프키 누를때 갱신
        //if (player._inputJump)
        //    player.climbPoint = player.hangableData.HangableHit.transform.GetComponent<ClimbPoint>();


        //타겟매칭 이뤄지는 신체부위의 오브젝트
        if (bodyPartHits.Count() > 0)
            player.climbPoint = bodyPartHits[0].transform.GetComponent<ClimbPoint>();

        //입력 방향따라 결정
        neighbour = player.climbPoint?.GetNeighbour(new Vector2(Mathf.Round(player._inputXZ.x), Mathf.Round(player._inputXZ.y)));

        if (neighbour != null)
        {
            if (neighbour.connectionType == ConnectionType.Jump)
            {
                player.climbPoint = neighbour.climbpoint;


                //뛸 예정지의 레이캐스트
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
            if (player._inputJump & !player.isBusy) //연타하면 고장나는 버그있어서 막아둠
            {
                if (neighbour.direction.y == 1)
                    player.PerformParkourState(LedgeToLedgeTopHit.point, player.PlayerBracedHangHopUpState);

                if (neighbour.direction.y == -1)
                    player.PerformParkourState(LedgeToLedgeTopHit.point, player.PlayerBracedHangHopDownState);

                Debug.Log("HangableHIt name : " + player.hangableData.HangableHit.transform.name);
                Debug.Log("점프 입력 후 neighbour : " + neighbour + "\t Direction : " + neighbour.direction);
            }
        }

        //가만히 붙어서 점프누르면 밖으로 튕겨나가기
        if (player._inputJump && player._inputXZ == Vector2.zero)
            stateMachine.ChangeState(player.jumpFromHangingWallState);


    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool(gameManager.animIDParkour_HangingIdle, false);
    }
}
