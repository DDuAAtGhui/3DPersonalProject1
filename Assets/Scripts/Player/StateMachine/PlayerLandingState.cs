using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandingState : PlayerGroundedState
{
    public PlayerLandingState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (player.totalFallingTime >= 0.5f && player._inputXZ != Vector2.zero)
            anim.SetBool(player.animIDLanding_Roll, true);

        if (player.totalFallingTime < 0.5f || player._inputXZ == Vector2.zero)
            anim.SetBool(player.animIDLanding_Small, true);
    }
    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        player._inputWalk = false;
        anim.SetBool(player.animIDLanding_Roll, false);
        anim.SetBool(player.animIDLanding_Small, false);
        base.Exit();
    }
}
