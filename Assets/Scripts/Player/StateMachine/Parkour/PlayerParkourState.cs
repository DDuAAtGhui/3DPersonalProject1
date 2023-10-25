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
    protected GameObject lastBodyPartHitsObject;


    //신체 말단부위가 허공직전인지 판정
    protected bool bodypartXpositiveHitFound;
    protected bool bodypartXnegativeHitFound;
    protected bool bodypartYpositiveHitFound;
    protected bool bodypartYnegativeHitFound;
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

        if (parkourAction.RotateToObstacle)
            player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation,
                parkourAction.TargetRotation, Time.deltaTime * parkourAction.RotateMultiflier);

        if (isAnimEnd)
            stateMachine.ChangeState(player.idleState);


        if (parkourToFallState)
            fallingTimer += Time.deltaTime;


        bodypartXpositiveHitFound = Physics.Raycast(matchTargetPosition + player.bodyPartRayX_Space, player.transform.forward,
            out RaycastHit bodypartXpositiveHit, 3f);

        bodypartXnegativeHitFound = Physics.Raycast(matchTargetPosition - player.bodyPartRayX_Space, player.transform.forward,
            out RaycastHit bodypartXnegativeHit, 3f);

        bodypartYpositiveHitFound = Physics.Raycast(matchTargetPosition + player.bodyPartRayY_Space, player.transform.forward,
            out RaycastHit bodypartYpositiveHit, 3f);

        bodypartYnegativeHitFound = Physics.Raycast(matchTargetPosition - player.bodyPartRayY_Space, player.transform.forward,
            out RaycastHit bodypartYnegativeHit, 3f);


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

        Gizmos.color = Color.red;

        Gizmos.DrawRay(matchTargetPosition + player.bodyPartRayX_Space, player.transform.forward * 3f);
        Gizmos.DrawRay(matchTargetPosition - player.bodyPartRayX_Space, player.transform.forward * 3f);
        Gizmos.DrawRay(matchTargetPosition + player.bodyPartRayY_Space, player.transform.forward * 3f);
        Gizmos.DrawRay(matchTargetPosition - player.bodyPartRayY_Space, player.transform.forward * 3f);
    }
}
