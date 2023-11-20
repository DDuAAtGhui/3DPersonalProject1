using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    //시야 원과 각도
    public float viewRadius = 3f;
    [Range(0, 360)] public float viewAngle;

    //목표물 레이어와 장애물 레이어
    public LayerMask targetMask, obstacleMask;

    //시야 안에 들어온 target 레이어 레이로 트랜스폼 받아서 저장할것
    public List<Transform> targetsInSight = new List<Transform>();

    private void Start()
    {
        StartCoroutine(FindTargets(0.1f));
    }

    //일정 간격으로 타겟 탐색
    IEnumerator FindTargets(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            FindTargetsInSight();
        }
    }

    //타겟 탐색 메소드
    void FindTargetsInSight()
    {
        targetsInSight.Clear();

        //viewRadius만큼의 반지름 가지는 구 생성해서
        //구 안에 들어온 targetMask 레이어 안 콜라이더 다 받아옴
        Collider[] targetsInViewRadius =
            Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;

            //내가 타겟을 바라보는 방향
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            //플레이어 앞 방향과 플레이어를 바라보는 방향 사이 각도가 설정한 각도 안이면
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                //타겟과의 거리 받아오기
                float distanceToTarget =
                    Vector3.Distance(transform.position, target.transform.position);

                //탐지된 타겟에게 레이 쏴서 장애물 아니면 시야 내 타겟 리스트에 추가
                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
                {
                    targetsInSight.Add(target);
                }
            }
        }
    }

    //GUI 표현 위함
    public Vector3 DirFromAngle(float angleDegree, bool angleIsGlobal)
    {
        //각도가 글로벌 아니면 글로벌로 변환
        if (!angleIsGlobal)
        {
            angleDegree += transform.eulerAngles.y;
        }
        //삼각함수같은 수학연산 표준형식으로 맞추기위해 라디안화
        //원점을 중심으로 하는 원통표면에 대한 벡터
        return new Vector3(Mathf.Sin(angleDegree * Mathf.Deg2Rad), 0,
            Mathf.Cos(angleDegree * Mathf.Deg2Rad));
    }

    public bool isTargetFound(GameObject target)
    {
        if (targetsInSight.Contains(target.transform))
        {
            return true;
        }
        return false;
    }
}
