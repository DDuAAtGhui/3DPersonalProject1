using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//스크립터블 오브젝트같은건 게임중에 값 변하고 게임 꺼져도 초기화 안되니까
//중간중간 변해야 하는값 같은거 있으면 임시 변수 만들어서 그거 활용해야함
[CreateAssetMenu(menuName = "Parkour System/New parkour action")]

//거리 높이등의 제한조건 필요하거나 타겟매칭이 필요한 액션들 조건설정
public class ParkourAction : ScriptableObject
{
    #region 파쿠르 할 장애물 정보와 애니메이션 이름
    [Header("Parkour Condition")]
    [SerializeField] string animName; //bool방식 말고 CrossFade, Play 방식 사용할 시
    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;
    [SerializeField] float minDistance;
    [SerializeField] float maxDistance;
    [SerializeField] string obstacleTag;


    #endregion
    #region 파쿠르 시 장애물 방향으로 회전
    [Header("Rotate To Obstacle")]
    [SerializeField] bool rotateToObstacle;
    //애니메이션 여러개 섞어서 사용 할 때 컨트롤 복귀에 딜레이 걸거면 사용
    [SerializeField] float postActionDelay;
    [SerializeField] float rotateMultiflier = 150f;
    public Quaternion TargetRotation { get; set; }
    #endregion

    #region 타겟매칭
    //matchStartTime와 matchTargetTime시간 사이에서 보정이 이루어진다
    [Header("Target Matching")]
    [SerializeField] bool enableTargetMatching = true;
    [SerializeField] protected AvatarTarget matchBodyPart; //원본 바디파트
    protected AvatarTarget temp_BodyPart; //데이터 오염 방지



    //애니메이션 클립에서 캐릭터가 땅으로부터 떨어지는 시간(퍼센트)
    [SerializeField] float matchStartTime;

    //애니메이션 클립에서 매칭하려는 신체부위로 착지하려는 시간(퍼센트)
    [SerializeField] float matchTargetTime;
    //x,y,z 축의 가중치. 1이면 매칭포지션을 그 축에 정확히 맞추는것
    [SerializeField] Vector3 matchPositionWeight = new Vector3(0, 1, 0);
    [SerializeField] Vector3 matchPositionOffset = Vector3.zero;
    [SerializeField] float matchPositionRotateWeight = 0f;


    //매칭이 일어날 좌표
    public Vector3 MatchPosition { get; set; }


    //애니메이션 뒤집는 중인지 체크.
    //애니메이터 인스펙터에서 파라미터로 연결해줄것
    public bool Mirror { get; set; }
    [Tooltip("어떤 종류의 액션인지 설정")]
    [Header("액션 종류")]
    [SerializeField] protected bool isHangingAction;

    [Space(10)]
    [Header("Optional")]
    [SerializeField] float forwardMovement = 0f;
    [SerializeField] bool usingLedgeAngle = false;
    [SerializeField] float ledgeAngleLimit = 50f;
    [SerializeField] bool usingObstacleTag = true;
    [Tooltip("현재 기본 타겟매칭 포지션 : 플레이어 앞 장애물의 맨 위쪽 모서리")][SerializeField] bool useCustomTargetMatchingPosition;
    [Tooltip("이 오브젝트의 포지션으로 타겟매칭됨")][SerializeField] public GameObject customTargetMatcingPositionOfObject;
    #endregion

    //기본 사용
    public virtual bool CheckIfPossible(ParkourAbleObstacleHitData hitdata, Transform playerTransform)
    {
        float height = hitdata.heighHit.point.y - playerTransform.position.y;

        //비어있지 않거나 설정된 태그에 맞을때 아니면 false 반환
        if (!string.IsNullOrEmpty(obstacleTag) && hitdata.forwardHit.transform.tag != obstacleTag)
            return false;

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
    public virtual bool CheckIfPossible()
    {

        temp_BodyPart = matchBodyPart;
        //Resources 폴더에 프리팹 넣고 불러온다음에 위치 변경시키면서 쓰기

        var player = GameManager.instance.player;
        var hitData = player.hitData;

        float height = player.heightToObstacle;
        float distance = player.distanceToObstacle;

        //비어있지 않거나 설정된 태그에 맞을때 아니면 false 반환
        if (hitData.forwardHitFound && hitData.forwardHit.transform.tag != obstacleTag
        && !string.IsNullOrEmpty(obstacleTag))
        {
            if (GameManager.instance.Log_ParkourActionSuccessInfo)
                Debug.Log(animName + " 장애물 태그 or 앞에 오브젝트 존재하지않음으로 통과 실패");
            return false;
        }

        if (usingObstacleTag && string.IsNullOrEmpty(obstacleTag))
        {
            if (GameManager.instance.Log_ParkourActionSuccessInfo)
                Debug.Log(animName + " obstacleTag필드 비어있음으로 통과 실패");
            return false;
        }



        //거리와 높이 체크(입력했을때만)
        if (minHeight != 0 && maxHeight != 0 && minDistance != 0 && maxDistance != 0)
            if (!hitData.heightHitFound || height < minHeight || height > maxHeight || distance < minDistance || distance > maxDistance)
            {
                if (GameManager.instance.Log_ParkourActionSuccessInfo)
                    Debug.Log(animName + " 높이 or 거리 통과 실패");
                return false;
            }

        //모서리에서(= 앞에 장애물이 없고 플레이어 앞이 땅일때)
        //각도 제한 있을 때 특정 각도 이하면 false 반환
        if (usingLedgeAngle)
        {
            if (player.LedgeData.angle > ledgeAngleLimit)
            {
                if (GameManager.instance.Log_ParkourActionSuccessInfo)
                    Debug.Log(animName + " 각도 제한으로 통과 실패");

                return false;
            }

            if (hitData.forwardHitFound)
            {
                if (GameManager.instance.Log_ParkourActionSuccessInfo)
                    Debug.Log(animName + " 앞에 장애물 감지되어 모서리가 아님");

                return false;
            }
        }

        if (isHangingAction && !player.hangableData.hitFound)
            return false;

        if (rotateToObstacle)
        {
            //  if (!isHangingAction)
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);

            //else
            //    TargetRotation = Quaternion.LookRotation(-player.hangableLedgeFrontHit.normal);
        }

        //if (enableTargetMatching && !useCustomTargetMatchingPosition)
        //    MatchPosition = hitData.heighHit.point;

        if (GameManager.instance.Log_ParkourActionSuccessInfo)
            Debug.Log(animName + " 통과");

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
    public bool UseCustomTargetMatchingPosition => useCustomTargetMatchingPosition;


}
