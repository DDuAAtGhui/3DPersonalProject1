using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/New parkour action")]
public class ParkourAction : ScriptableObject
{
    #region 파쿠르 할 장애물 정보와 애니메이션 이름
    [Header("Parkour Condition")]
    [SerializeField] string animName; //bool방식 말고 CrossFade, Play 방식 사용할 시
    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;
    [SerializeField] float minDistance;
    [SerializeField] float maxDistance;
    #endregion

    #region 파쿠르 시 장애물 방향으로 회전
    [Header("Rotate To Obstacle")]
    [SerializeField] bool rotateToObstacle;
    [SerializeField] float rotateMultiflier = 150f;
    public Quaternion TargetRotation { get; set; }
    #endregion

    #region 타겟매칭
    //matchStartTime와 matchTargetTime시간 사이에서 보정이 이루어진다
    [Header("Target Matching")]
    [SerializeField] bool enableTargetMatching = true;
    [SerializeField] AvatarTarget matchBodyPart;

    //애니메이션 클립에서 캐릭터가 땅으로부터 떨어지는 시간(퍼센트)
    [SerializeField] float matchStartTime;

    //애니메이션 클립에서 매칭하려는 신체부위로 착지하려는 시간(퍼센트)
    [SerializeField] float matchTargetTime;

    //매칭이 일어날 좌표
    public Vector3 MatchPosition { get; set; }
    #endregion

    //기본 사용
    public bool CheckIfPossible(ParkourAbleObstacleHitData hitdata, Transform playerTransform)
    {
        float height = hitdata.heighHit.point.y - playerTransform.position.y;



        if (height < minHeight || height > maxHeight)
            return false;

        //파쿠르 가능한 경우
        if (rotateToObstacle)
            TargetRotation = Quaternion.LookRotation(-hitdata.forwardHit.normal);

        if (enableTargetMatching)
            MatchPosition = hitdata.heighHit.point;

        return true;
    }

    //싱글톤, 상태머신과 사용
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
