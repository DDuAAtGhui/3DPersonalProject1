using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Grounded, Jump 상태에서 Falling으로 진입함
public class PlayeFallingState : PlayerAirborneState
{
    public PlayeFallingState(Player player, PlayerStateMachine stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        anim.SetBool(gameManager.animIDFreeFall, true);

        if (groundToFallState)
            StateTimer = player.CoyoteTime;

        //경사로에서 잘 붙어있을 수 있게 GroundedGravity를 크게 설정해놨기 때문에
        //그 값을 유지한 채 추가로 가속도 붙지않게 0으로 초기화
        verticalVelocity = 0;

        //fallTimeoutDelta 시간 지난 후에 다시 FallingState 진입 가능
        player.StartCoroutine("nowBusy", fallTimeoutDelta);
    }
    public override void Update()
    {
        base.Update();


        fallingTimer += Time.deltaTime;

        if (player.isGrounded)
            stateMachine.ChangeState(player.landingState);


        else if (player._inputJump && StateTimer >= 0f)
            stateMachine.ChangeState(player.jumpState);
    }
    public override void Exit()
    {
        base.Exit();
        //총 낙하 시간 반환
        player.totalFallingTime = fallingTimer;
        fallingTimer = 0;
        anim.SetBool(gameManager.animIDFreeFall, false);

        groundToFallState = false;
        parkourToFallState = false;
    }
}
