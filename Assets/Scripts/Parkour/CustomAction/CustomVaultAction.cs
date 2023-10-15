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

        // hitdata.forwardHit������Ʈ�� Transform��������
        // hitdata.forwardHit.point�� position�� Local��ǥ��� ��ȯ
        var LocalhitPoint = hitdata.forwardHit.transform.
            InverseTransformPoint(hitdata.forwardHit.point);

        // ������Ʈ�� �߽��� 0�̹Ƿ� X�� 0���� ������ ����
        // Z�� 0���� �������� ����ĳ��Ʈ�� �浹������ �ű� ��ü�� ������
        if (LocalhitPoint.z < 0 && LocalhitPoint.x < 0 ||
            LocalhitPoint.z > 0 && LocalhitPoint.x > 0)
        {
            //�ִϸ��̼� ������
            Mirror = true;

            //Ÿ�ٸ�Ī �Ͼ�� ��ü������ �������ֱ�
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

        // ������
        else
        {
            //������ �ʱ�
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

        // hitdata.forwardHit������Ʈ�� Transform��������
        // hitdata.forwardHit.point�� position�� Local��ǥ��� ��ȯ
        var LocalhitPoint = hitData.forwardHit.transform.
            InverseTransformPoint(hitData.forwardHit.point);

        // ������Ʈ�� �߽��� 0�̹Ƿ� X�� 0���� ������ ����
        // Z�� 0���� �������� ����ĳ��Ʈ�� �浹������ �ű� ��ü�� ������
        if (LocalhitPoint.z < 0 && LocalhitPoint.x < 0 ||
            LocalhitPoint.z > 0 && LocalhitPoint.x > 0)
        {
            //�ִϸ��̼� ������
            Mirror = true;

            //Ÿ�ٸ�Ī �Ͼ�� ��ü������ �������ֱ�
            //���� �ٵ���Ʈ�� ����� �ӽ� ���� ���� Ȱ���ؾ� ������ ���� �ȵ�
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

        // ������
        else
        {
            //������ �ʱ�
            Mirror = false;
            temp_BodyPart = matchBodyPart;
        }

        return true;
    }
}
