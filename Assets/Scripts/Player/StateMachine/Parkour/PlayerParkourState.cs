using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;


public class PlayerParkourState : PlayerStates
{
    protected ParkourAction parkourAction;

    protected Neighbour neighbour;
    protected GameObject HangableNetworkSphereObject;
    protected Bounds neighbourBounds;
    protected ClimbPoint climbPointAtEnter;

    protected Vector3 matchTargetPosition;

    protected RaycastHit[] bodyPartHits;
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

        //   gameManager.CustomTargetMatchingObject = gameManager.StoredObjectForSwitching;

        //GroundGraivty가 클 때 파쿠르 동작 진입시 급강하 방지
        verticalVelocity = 0;

        HangableNetworkSphereObject = gameManager.HangableNetworkSphereObject;
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

        switch (parkourAction.MatchBodyPart)
        {
            case AvatarTarget.RightHand:
                matchTargetPosition = anim.GetBoneTransform(HumanBodyBones.RightHand).position;
                break;
            case AvatarTarget.LeftHand:
                matchTargetPosition = anim.GetBoneTransform(HumanBodyBones.LeftHand).position;
                break;
            case AvatarTarget.RightFoot:
                matchTargetPosition = anim.GetBoneTransform(HumanBodyBones.RightFoot).position;
                break;
            case AvatarTarget.LeftFoot:
                matchTargetPosition = anim.GetBoneTransform(HumanBodyBones.LeftFoot).position;
                break;

            default:
                break;
        }

        bodyPartHits = Physics.SphereCastAll(matchTargetPosition, 0.25f, Vector3.up, 0, player.hangableLayer);

        foreach (var item in bodyPartHits)
        {
            Debug.Log("bodyPartHits : " + item.transform.name);
        }

        if (parkourAction.RotateToObstacle)
            player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation,
                parkourAction.TargetRotation, Time.deltaTime * parkourAction.RotateMultiflier);

        if (isAnimEnd)
            stateMachine.ChangeState(player.idleState);


        if (parkourToFallState)
            fallingTimer += Time.deltaTime;


    }
    public override void Exit()
    {
        base.Exit();
        player.inParkourAction = false;

        player.SetControllable(true);
        anim.SetBool(gameManager.animIDParkouring, false);

        gameManager.StandardTargetMatchingObject.SetActive(false);
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;

        Gizmos.DrawSphere(matchTargetPosition, 0.25f);
    }
}
