using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/New parkour action")]
public class ParkourAction : ScriptableObject
{
    #region ���� �� ��ֹ� ������ �ִϸ��̼� �̸�
    [Header("Parkour Condition")]
    [SerializeField] string animName; //bool��� ���� CrossFade, Play ��� ����� ��
    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;
    [SerializeField] float minDistance;
    [SerializeField] float maxDistance;
    [SerializeField] string obstacleTag;
    #endregion
    #region ���� �� ��ֹ� �������� ȸ��
    [Header("Rotate To Obstacle")]
    [SerializeField] bool rotateToObstacle;
    //�ִϸ��̼� ������ ��� ��� �� �� ��Ʈ�� ���Ϳ� ������ �ɰŸ� ���
    [SerializeField] float postActionDelay;
    [SerializeField] float rotateMultiflier = 150f;
    public Quaternion TargetRotation { get; set; }
    #endregion

    #region Ÿ�ٸ�Ī
    //matchStartTime�� matchTargetTime�ð� ���̿��� ������ �̷������
    [Header("Target Matching")]
    [SerializeField] bool enableTargetMatching = true;
    [SerializeField] AvatarTarget matchBodyPart;

    //�ִϸ��̼� Ŭ������ ĳ���Ͱ� �����κ��� �������� �ð�(�ۼ�Ʈ)
    [SerializeField] float matchStartTime;

    //�ִϸ��̼� Ŭ������ ��Ī�Ϸ��� ��ü������ �����Ϸ��� �ð�(�ۼ�Ʈ)
    [SerializeField] float matchTargetTime;
    //x,y,z ���� ����ġ. 1�̸� ��Ī�������� �� �࿡ ��Ȯ�� ���ߴ°�
    [SerializeField] Vector3 matchPositionWeight = new Vector3(0, 1, 0);
    [SerializeField] float matchPositionRotateWeight = 0f;


    //��Ī�� �Ͼ ��ǥ
    public Vector3 MatchPosition { get; set; }

    [Header("Optional")]
    [SerializeField] float forwardMovement = 0f;
    #endregion

    //�⺻ ���
    public bool CheckIfPossible(ParkourAbleObstacleHitData hitdata, Transform playerTransform)
    {
        float height = hitdata.heighHit.point.y - playerTransform.position.y;

        //������� �ʰų� ������ �±׿� ������ �ƴϸ� false ��ȯ
        if (!string.IsNullOrEmpty(obstacleTag) && hitdata.forwardHit.transform.tag != obstacleTag)
            return false;

        if (height < minHeight || height > maxHeight)
            return false;

        //���� ������ ���
        if (rotateToObstacle)
            TargetRotation = Quaternion.LookRotation(-hitdata.forwardHit.normal);

        if (enableTargetMatching)
            MatchPosition = hitdata.heighHit.point;

        return true;
    }

    //�̱���, ���¸ӽŰ� ���
    public bool CheckIfPossible()
    {
        var player = GameManager.instance.player;
        var hitData = player.hitData;

        float height = player.heightToObstacle;
        float distance = player.distanceToObstacle;

        //������� �ʰų� ������ �±׿� ������ �ƴϸ� false ��ȯ
        if (!string.IsNullOrEmpty(obstacleTag) && hitData.forwardHit.transform.tag != obstacleTag)
            return false;

        //�Ÿ��� ���� üũ
        if (!hitData.heightHitFound || height < minHeight || height > maxHeight || distance < minDistance || distance > maxDistance)
            return false;

        if (rotateToObstacle)
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);

        if (enableTargetMatching)
            MatchPosition = hitData.heighHit.point;

        return true;
    }

    public string AniName => animName;
    public bool RotateToObstacle => rotateToObstacle;
    public float PostActionDelay => postActionDelay;
    public float RotateMultiflier => rotateMultiflier;
    public bool EnableTargetMatching => enableTargetMatching;
    public AvatarTarget MatchBodyPart => matchBodyPart;
    public float MatchStartTime => matchStartTime;
    public float MatchTargetTime => matchTargetTime;
    public Vector3 MatchPositionWeight => matchPositionWeight;
    public float MatchPositionRotateWeight => matchPositionRotateWeight;
    //Optional
    public float ForwardMovement => forwardMovement;
}
