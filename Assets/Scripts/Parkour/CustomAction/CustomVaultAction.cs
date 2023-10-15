using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/Custom Actions/New parkour action")]
public class CustomVaultAction : ParkourAction
{
    public override bool CheckIfPossible(ParkourAbleObstacleHitData hitdata, Transform playerTransform)
    {
        if (!base.CheckIfPossible(hitdata, playerTransform))
            return false;

        // hitdata.forwardHit오브젝트의 Transform기준으로
        // hitdata.forwardHit.point의 position을 Local좌표계로 변환
        var LocalhitPoint = hitdata.forwardHit.transform.
            InverseTransformPoint(hitdata.forwardHit.point);

        // 오브젝트의 중심이 0이므로 X가 0보다 작으면 왼쪽
        // Z가 0보다 작은곳에 레이캐스트가 충돌했으면 거긴 물체의 뒤쪽임
        if (LocalhitPoint.z < 0 && LocalhitPoint.x < 0 ||
            LocalhitPoint.z > 0 && LocalhitPoint.x > 0)
        {
            //애니메이션 뒤집기
            Mirror = true;

            //타겟매칭 일어나는 신체부위도 뒤집어주기
            switch (matchBodyPart)
            {
                case AvatarTarget.LeftHand:
                    matchBodyPart = AvatarTarget.RightHand; break;
                case AvatarTarget.RightHand:
                    matchBodyPart = AvatarTarget.LeftHand; break;
                case AvatarTarget.LeftFoot:
                    matchBodyPart = AvatarTarget.RightFoot; break;
                case AvatarTarget.RightFoot:
                    matchBodyPart = AvatarTarget.LeftFoot; break;
                default:
                    break;
            }
        }

        // 오른쪽
        else
        {
            //뒤집지 않기
            Mirror = false;
            matchBodyPart = AvatarTarget.LeftHand;
        }

        return true;
    }

    public override bool CheckIfPossible()
    {
        if (!base.CheckIfPossible())
            return false;

        var player = GameManager.instance.player;
        var hitData = player.hitData;

        // hitdata.forwardHit오브젝트의 Transform기준으로
        // hitdata.forwardHit.point의 position을 Local좌표계로 변환
        var LocalhitPoint = hitData.forwardHit.transform.
            InverseTransformPoint(hitData.forwardHit.point);

        // 오브젝트의 중심이 0이므로 X가 0보다 작으면 왼쪽
        // Z가 0보다 작은곳에 레이캐스트가 충돌했으면 거긴 물체의 뒤쪽임
        if (LocalhitPoint.z < 0 && LocalhitPoint.x < 0 ||
            LocalhitPoint.z > 0 && LocalhitPoint.x > 0)
        {
            //애니메이션 뒤집기
            Mirror = true;

            //타겟매칭 일어나는 신체부위도 뒤집어주기
            //원본 바디파트는 남기고 임시 변수 만들어서 활용해야 데이터 오염 안됨
            switch (temp_BodyPart)
            {
                case AvatarTarget.LeftHand:
                    temp_BodyPart = AvatarTarget.RightHand; break;
                case AvatarTarget.RightHand:
                    temp_BodyPart = AvatarTarget.LeftHand; break;
                case AvatarTarget.LeftFoot:
                    temp_BodyPart = AvatarTarget.RightFoot; break;
                case AvatarTarget.RightFoot:
                    temp_BodyPart = AvatarTarget.LeftFoot; break;
                default:
                    break;
            }

        }

        // 오른쪽
        else
        {
            //뒤집지 않기
            Mirror = false;
            temp_BodyPart = matchBodyPart;
        }

        return true;
    }
}
