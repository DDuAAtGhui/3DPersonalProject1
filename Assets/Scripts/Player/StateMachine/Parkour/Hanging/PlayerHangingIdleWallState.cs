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
        ControllableLedgeAction = true;
    }
    public override void Update()
    {
        base.Update();

        //레이캐스트 오브젝트 이름 디버그
        //for (int i = 0; i < bodyPartHits.Count(); i++)
        //{
        //    Debug.Log("bodyPartHits : " + bodyPartHits[i].transform.gameObject.name);

        //}

        #region 모서리액션
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


                ////뛸 예정지의 레이캐스트
                //RaycastHit LedgeToLedgeTopHit = LedgeToLedgeCheck();

                //this.LedgeToLedgeTopHit = LedgeToLedgeTopHit;

                //연타하면 고장나는 버그있어서 막아둠
                if (player._inputJump && StateTimer <= 0f)
                {
                    if (neighbour.direction.y == 1)
                        player.PerformParkourState(this.LedgeToLedgeTopHit.point, player.bracedHangHopUpState);

                    if (neighbour.direction.y == -1)
                        player.PerformParkourState(this.LedgeToLedgeTopHit.point, player.bracedHangHopDownState);

                    if (neighbour.direction.x == 1)
                    {
                        matchTargetPosition = anim.GetBoneTransform(HumanBodyBones.LeftHand).position;
                        player.PerformParkourState(this.LedgeToLedgeTopHit.point, player.bracedHangHopRightState);
                    }
                    if (neighbour.direction.x == -1)
                    {
                        matchTargetPosition = anim.GetBoneTransform(HumanBodyBones.RightHand).position;
                        player.PerformParkourState(this.LedgeToLedgeTopHit.point, player.bracedHangHopLeftState);
                    }
                }
            }

            else if (neighbour.connectionType == ConnectionType.Move)
            {
                player.climbPoint = neighbour.climbpoint;

                ////뛸 예정지의 레이캐스트
                //RaycastHit LedgeToLedgeTopHit = LedgeToLedgeCheck();

                //this.LedgeToLedgeTopHit = LedgeToLedgeTopHit;

                Debug.Log("current LedgeToLedgeTopHit pos : " + this.LedgeToLedgeTopHit.point);

                if (neighbour.direction.x == 1 && bodypartXpositiveHitFound)
                    player.PerformParkourState(this.LedgeToLedgeTopHit.point, player.bracedHangShimmyRightState);

                if (neighbour.direction.x == -1 && bodypartXnegativeHitFound)
                    player.PerformParkourState(this.LedgeToLedgeTopHit.point, player.bracedHangShimmyLeftState);
            }
        }
        #endregion


        //가만히 붙어서 점프누르면 밖으로 튕겨나가기
        if (player._inputJump && player._inputXZ == Vector2.zero)
            stateMachine.ChangeState(player.jumpFromHangingWallState);

        if (!bodypartYpositiveHitFound && player._inputXZ.y == 1 && player._inputJump)
            player.PerformParkourState(LedgeToLedgeTopHit.point, player.bracedHangToCrouchState);
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool(gameManager.animIDParkour_HangingIdle, false);
    }
}
