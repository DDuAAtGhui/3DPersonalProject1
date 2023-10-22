using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ConnectionType
{
    Jump, Move
}

public class ClimbPoint : MonoBehaviour
{
    [SerializeField] List<Neighbour> neighbours;
    private void Awake()
    {
        //TwoWay인 애들만 모인 리스트
        var twoWayNeighbours = neighbours.Where(n => n.isTwoWay).ToList();

        //TwoWay true인애들은 서로가 연결되게
        foreach (var neighbour in twoWayNeighbours)
        {
            //자기 자신 클래스 등록.
            //dir => 언제나 dir의 반대방향으로 등록
            //(한쪽에서 하나 등록된거면 반대쪽에서도 자동 등록되야하니 방향도 반대임)
            neighbour.climbpoint?.CreateConnection(this, -neighbour.direction,
                neighbour.connectionType, neighbour.isTwoWay);

        }

    }

    public void CreateConnection(ClimbPoint climbPoint, Vector2 direction,
        ConnectionType connectionType, bool isTwoway)
    {

        //인스턴스 생성하면서 생성자로 멤버에 바로 넣기
        var neighbour = new Neighbour()
        {
            climbpoint = climbPoint,
            direction = direction,
            connectionType = connectionType,
            isTwoWay = isTwoway
        };

        neighbours.Add(neighbour);
    }


    //ClimbPoint의 direction 멤버와 비교해서 같은 Neighbour반환
    //플레이어 입력 방향에 맞게 다른 ledge로 뛰게끔
    /// <summary>
    /// 
    /// </summary>
    /// <param name="neighbourDir">플레이어 입력 방향값. ClimbPoint의 direction과 값이 일치해야함</param>
    /// <returns></returns>
    public Neighbour GetNeighbour(Vector2 neighbourDir)
    {
        Neighbour neighbour = null;

        //이하 조건에 맞는 시퀀스의 첫번째 요소 반환. 없으면 기본값 반환
        if (neighbourDir.y != 0)
            neighbour = neighbours.FirstOrDefault(n => n.direction.y == neighbourDir.y);


        //플레이어가 y축 입력했는데 그 방향에 neighbour없으면 x 축으로 존재하는지 체크
        if (neighbour == null && neighbourDir.x != 0)
            neighbour = neighbours.FirstOrDefault(n => n.direction.x == neighbourDir.x);


        if (neighbour != null)
        {
            Debug.Log("Found Neighbor: " + neighbour.climbpoint.gameObject.name);
        }
        else
        {
            Debug.Log("Neighbor Not Found");
        }

        return neighbour;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward * 2f, Color.blue);
        foreach (var neighbour in neighbours)
        {
            if (neighbour.climbpoint != null)
            {
                Debug.DrawLine(transform.position, neighbour.climbpoint.transform.position,
                    neighbour.isTwoWay ? Color.green : Color.red);

                ////피벗 뒤틀려서 빈 부모 만들고 거기에서 나오게 설정
                //Debug.DrawLine(transform.parent.position, neighbour.climbpoint.transform.parent.position,
                //    neighbour.isTwoWay ? Color.green : Color.red);

            }
        }
    }
}

[System.Serializable]
public class Neighbour
{
    [Tooltip("선 지점")] public ClimbPoint climbpoint;
    [Tooltip("애니메이션 사분면 이동방향")] public Vector2 direction;
    [Tooltip("액션 타입")] public ConnectionType connectionType;
    [Tooltip("양방향으로 왔다 갔다 할 수 있는지")] public bool isTwoWay = true;
}

