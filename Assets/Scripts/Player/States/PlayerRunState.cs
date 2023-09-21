using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerGroundState
{
    public PlayerRunState(Player player, PlayerStateMachine stateMachine, string AnimationName) : base(player, stateMachine, AnimationName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        player.SetVelocityXZ(X_Input * player.MoveSpeed, Y_Input * player.MoveSpeed);

        Vector3 movement = new Vector3(X_Input, 0, Y_Input).normalized * player.MoveSpeed * Time.fixedDeltaTime;
        Quaternion targetRotation = Quaternion.LookRotation(movement);

        if (movement != Vector3.zero)
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, targetRotation, Time.fixedDeltaTime * 25);
    }

    public override void Update()
    {
        base.Update();




        if (X_Input == 0 && Y_Input == 0)
            stateMachine.ChangeState(player.idleState);
    }
    public override void Exit()
    {
        base.Exit();
    }
}
