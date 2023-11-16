using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerJumpState : PlayerAirborneState
{
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(gameManager.animIDJump, true);

        //최소 점프 보장 시간
        StateTimer = 0.2f;

        //jumpTimeout 시간이 지난 후에 다시 점프 가능
        player.StartCoroutine("nowBusy", player.jumpTimeout);

        // 루트 높이 * -2f * 중력 = 원하는 높이까지 도달하는 벨로시티값
        verticalVelocity = Mathf.Sqrt(player.jumpHeight * -2f * player.gravity);
    }


    public override void Update()
    {
        base.Update();

        if (StateTimer > 0)
            player.can_MoveHorizontally = false;


        if (!player.isGrounded && verticalVelocity <= 0f && !player.isBusy)
            stateMachine.ChangeState(player.fallingState);

        else if (player.isGrounded && StateTimer <= 0f)
            stateMachine.ChangeState(player.idleState);
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        //루트모션 적용할 때 벨로시티 초기화되는 버그있어서
        //일단 점프에서 CC.Move 할 수 있도록함
        CC.Move(targetDirection.normalized * (speed * Time.deltaTime)
    + new Vector3(0, verticalVelocity, 0) * Time.deltaTime);

    }
    public override void Exit()
    {
        base.Exit();

        //점프키 누르고 있을때 콜백에서 계속 값 읽어들여서 반복점프 되는거 방지
        player._inputJump = false;
        anim.SetBool(gameManager.animIDJump, false);
    }


}
