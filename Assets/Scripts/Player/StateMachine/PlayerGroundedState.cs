using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerStates
{
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        base.Update();

        Move();

        if (speed == 0)
            stateMachine.ChangeState(player.idleState);

        else if (speed != 0 && speed <= 7f)
            stateMachine.ChangeState(player.walkState);

        else if (speed > 7f)
            stateMachine.ChangeState(player.runState);

        Jump();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }
    public override void Exit()
    {
        base.Exit();
    }
    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }

    public override void OnCollisionStay(Collision collision)
    {
        base.OnCollisionStay(collision);
    }
    #region 메소드들
    #endregion
}
