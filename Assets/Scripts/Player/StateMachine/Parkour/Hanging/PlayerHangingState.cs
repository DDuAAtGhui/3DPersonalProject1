using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//매달린 상태에서 다른 Ledge 감지
public class PlayerHangingState : PlayerParkourState
{
    protected Neighbour neighbour;
    protected GameObject HangableObject;
    protected Bounds neighbourBounds;
    public PlayerHangingState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        HangableObject = gameManager.HangableObject;

        player.climbPoint = player.hangableData.HangableHit.transform.gameObject?.GetComponent<ClimbPoint>();
        neighbour = player.climbPoint?.GetNeighbour(new Vector2(Mathf.Round(player._inputXZ.x), Mathf.Round(player._inputXZ.y)));

    }

    public override void Update()
    {
        base.Update();

        //구 크기 커가지고 키 입력할때만 보이게했음
        if (gameManager.Visible_MatchPosition && player._inputXZ != Vector2.zero)
            HangableObject.SetActive(true);

        else
            HangableObject.SetActive(false);

        player.climbPoint = player.hangableData.HangableHit.transform.gameObject?.GetComponent<ClimbPoint>();
        neighbour = player.climbPoint?.GetNeighbour(new Vector2(Mathf.Round(player._inputXZ.x), Mathf.Round(player._inputXZ.y)));


        if (neighbour != null)
        {
            neighbourBounds = player.climbPoint.GetComponent<Renderer>().bounds;
            Debug.Log(neighbourBounds);
            //   var neighbourBoundsForwardTopLedgePoint = neighbourBounds.

            if (neighbour.connectionType == ConnectionType.Jump)
            {
                player.climbPoint = neighbour.climbpoint;

                //if (neighbour.direction.y == 1)
                //    anim.SetBool(gameManager.animIDParkour_BracedHangHopUp, true);

                //if (neighbour.direction.x == 1)
                //    anim.SetBool(gameManager.animIDParkour_BracedHangHopRight, true);
            }

            if (neighbour.connectionType == ConnectionType.Move)
            {

            }
            Debug.Log(neighbour.direction);

            HangableObject.transform.position = neighbour.climbpoint.transform.transform.position;
        }

        if (isAnimEnd)
            stateMachine.ChangeState(player.hangingIdleWallState);


    }
    public override void Exit()
    {
        base.Exit();
       // gameManager.CustomTargetMatchingObject = gameManager.StoredObjectForSwitching;

    }
}
