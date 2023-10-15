using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class PlayerParkourState : PlayerStates
{
    protected ParkourAction parkourAction;
    public PlayerParkourState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        anim.applyRootMotion = true;
        player.inParkourAction = true;

        anim.SetBool(gameManager.animIDParkouring, true);
        player.SetControllable(false);

        parkourAction = player.parkourActions[player.currentParkourActionIndex];
        anim.SetBool("mirrorAction", parkourAction.Mirror);
    }
    public override void Update()
    {
        base.Update();

        switch (parkourAction.ForwardMovement)
        {
            case 0:
                break;
            default:
                player.transform.position += player.transform.forward * Time.deltaTime * parkourAction.ForwardMovement;
                break;
        }

        if (parkourAction.RotateToObstacle)
            player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation,
                parkourAction.TargetRotation, Time.deltaTime * parkourAction.RotateMultiflier);

        if (isAnimEnd)
            stateMachine.ChangeState(player.idleState);


    }
    public override void Exit()
    {
        base.Exit();
        player.inParkourAction = false;

        player.SetControllable(true);
        anim.SetBool(gameManager.animIDParkouring, false);

        //Å¸°Ù ¸ÅÄª Á¾·á
        player.StopAllCoroutines();
        gameManager.TargetMatchOffsetStandard.SetActive(false);
    }
}
