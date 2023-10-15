using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.PlayerLoop;

//��ũ���ͺ� ������Ʈ������ �����߿� �� ���ϰ� ���� ������ �ʱ�ȭ �ȵǴϱ�
//�߰��߰� ���ؾ� �ϴ°� ������ ������ �ӽ� ���� ���� �װ� Ȱ���ؾ���
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
    [SerializeField] protected AvatarTarget matchBodyPart; //���� �ٵ���Ʈ
    protected AvatarTarget temp_BodyPart; //������ ���� ����


    //�ִϸ��̼� Ŭ������ ĳ���Ͱ� �����κ��� �������� �ð�(�ۼ�Ʈ)
    [SerializeField] float matchStartTime;

    //�ִϸ��̼� Ŭ������ ��Ī�Ϸ��� ��ü������ �����Ϸ��� �ð�(�ۼ�Ʈ)
    [SerializeField] float matchTargetTime;
    //x,y,z ���� ����ġ. 1�̸� ��Ī�������� �� �࿡ ��Ȯ�� ���ߴ°�
    [SerializeField] Vector3 matchPositionWeight = new Vector3(0, 1, 0);
    [SerializeField] Vector3 matchPositionOffset = Vector3.zero;
    [SerializeField] float matchPositionRotateWeight = 0f;


    //��Ī�� �Ͼ ��ǥ
    public Vector3 MatchPosition { get; set; }

    [Header("Optional")]
    [SerializeField] float forwardMovement = 0f;
    [SerializeField] bool usingLedgeAngle = false;
    [SerializeField] float ledgeAngleLimit = 50f;

    //�ִϸ��̼� ������ ������ üũ.
    //�ִϸ����� �ν����Ϳ��� �Ķ���ͷ� �������ٰ�
    public bool Mirror { get; set; }
    #endregion

    //�⺻ ���
    public virtual bool CheckIfPossible(ParkourAbleObstacleHitData hitdata, Transform playerTransform)
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

    public bool excutable;
    //�̱���, ���¸ӽŰ� ���
    public virtual bool CheckIfPossible()
    {
        excutable = false;

        temp_BodyPart = matchBodyPart;

        var player = GameManager.instance.player;
        var hitData = player.hitData;

        float height = player.heightToObstacle;
        float distance = player.distanceToObstacle;



        //������� �ʰų� ������ �±׿� ������ �ƴϸ� false ��ȯ
        if (hitData.forwardHitFound && !string.IsNullOrEmpty(obstacleTag) && hitData.forwardHit.transform.tag != obstacleTag)
        {
            Debug.Log(animName + " ��ֹ� �±� or �տ� ������Ʈ ���������������� ��� ����");
            return false;
        }

        //�Ÿ��� ���� üũ(�Է���������)
        if (minHeight != 0 && maxHeight != 0 && minDistance != 0 && maxDistance != 0)
            if (!hitData.heightHitFound || height < minHeight || height > maxHeight || distance < minDistance || distance > maxDistance)
            {
                Debug.Log(animName + " ���� or �Ÿ� ��� ����");
                return false;
            }

        //���� ���� ���� �� Ư�� ���� ���ϸ� false ��ȯ
        if (usingLedgeAngle && player.LedgeData.angle > ledgeAngleLimit)
        {
            Debug.Log(animName + " ���� �������� ��� ����");
            return false;
        }

        if (rotateToObstacle)
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);

        if (enableTargetMatching)
            MatchPosition = hitData.heighHit.point;

        Debug.Log(animName + "���");
        excutable = true;
        return true;
    }

    public string AniName => animName;
    public bool RotateToObstacle => rotateToObstacle;
    public float PostActionDelay => postActionDelay;
    public float RotateMultiflier => rotateMultiflier;
    public bool EnableTargetMatching => enableTargetMatching;
    public AvatarTarget MatchBodyPart => temp_BodyPart;
    public float MatchStartTime => matchStartTime;
    public float MatchTargetTime => matchTargetTime;
    public Vector3 MatchPositionWeight => matchPositionWeight;
    public Vector3 MatchPositionOffset => matchPositionOffset;
    public float MatchPositionRotateWeight => matchPositionRotateWeight;
    //Optional
    public float ForwardMovement => forwardMovement;
}
