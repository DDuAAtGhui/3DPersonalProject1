using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBracedHangShimmyState : PlayerHangingState
{
    public PlayerBracedHangShimmyState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        ControllableLedgeAction = true;
        isShimmy = true;
    }
    public override void Update()
    {
        base.Update();

        //타겟매칭 이뤄지는 신체부위의 오브젝트
        if (bodyPartHits.Count() > 0)
            player.climbPoint = bodyPartHits[0].transform.GetComponent<ClimbPoint>();


    }
    public override void Exit()
    {
        base.Exit();
        isShimmy = false;
    }


}
