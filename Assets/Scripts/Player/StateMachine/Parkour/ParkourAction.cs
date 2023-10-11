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
    #endregion

    #region ���� �� ��ֹ� �������� ȸ��
    [Header("Rotate To Obstacle")]
    [SerializeField] bool rotateToObstacle;
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

    //��Ī�� �Ͼ ��ǥ
    public Vector3 MatchPosition { get; set; }
    #endregion

    //�⺻ ���
    public bool CheckIfPossible(ParkourAbleObstacleHitData hitdata, Transform playerTransform)
    {
        float height = hitdata.heighHit.point.y - playerTransform.position.y;



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

        if (height < minHeight || height > maxHeight)
            return false;

        if (rotateToObstacle)
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);

        if (enableTargetMatching)
            // MatchPosition = hitData.heighHit.point;
            MatchPosition = hitData.heighHit.point;


        Debug.Log("MatchPosition :" + MatchPosition);
        return true;
    }

    public string AniName => animName;
    public bool RotateToObstacle => rotateToObstacle;
    public float RotateMultiflier => rotateMultiflier;
    public bool EnableTargetMatching => enableTargetMatching;
    public AvatarTarget MatchBodyPart => matchBodyPart;
    public float MatchStartTime => matchStartTime;
    public float MatchTargetTime => matchTargetTime;
}
