using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpOver_RollState : PlayerParkourState
{
    public PlayerJumpOver_RollState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(player.animIDParkour_JumpOver_Roll, true);
    }
    public override void Update()
    {
        base.Update();
        player.transform.position += player.transform.forward * Time.deltaTime * 4;
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool(player.animIDParkour_JumpOver_Roll, false);
    }
}
