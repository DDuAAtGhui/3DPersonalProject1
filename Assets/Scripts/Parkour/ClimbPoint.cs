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
        //TwoWay�� �ֵ鸸 ���� ����Ʈ
        var twoWayNeighbours = neighbours.Where(n => n.isTwoWay).ToList();

        //TwoWay true�ξֵ��� ���ΰ� ����ǰ�
        foreach (var neighbour in twoWayNeighbours)
        {
            //�ڱ� �ڽ� Ŭ���� ���.
            //dir => ������ dir�� �ݴ�������� ���
            //(���ʿ��� �ϳ� ��ϵȰŸ� �ݴ��ʿ����� �ڵ� ��ϵǾ��ϴ� ���⵵ �ݴ���)
            neighbour.climbpoint?.CreateConnection(this, -neighbour.direction,
                neighbour.connectionType, neighbour.isTwoWay);

        }

    }

    public void CreateConnection(ClimbPoint climbPoint, Vector2 direction,
        ConnectionType connectionType, bool isTwoway)
    {

        //�ν��Ͻ� �����ϸ鼭 �����ڷ� ����� �ٷ� �ֱ�
        var neighbour = new Neighbour()
        {
            climbpoint = climbPoint,
            direction = direction,
            connectionType = connectionType,
            isTwoWay = isTwoway
        };

        neighbours.Add(neighbour);
    }


    //ClimbPoint�� direction ����� ���ؼ� ���� Neighbour��ȯ
    //�÷��̾� �Է� ���⿡ �°� �ٸ� ledge�� �ٰԲ�
    /// <summary>
    /// 
    /// </summary>
    /// <param name="neighbourDir">�÷��̾� �Է� ���Ⱚ. ClimbPoint�� direction�� ���� ��ġ�ؾ���</param>
    /// <returns></returns>
    public Neighbour GetNeighbour(Vector2 neighbourDir)
    {
        Neighbour neighbour = null;

        //���� ���ǿ� �´� �������� ù��° ��� ��ȯ. ������ �⺻�� ��ȯ
        if (neighbourDir.y != 0)
            neighbour = neighbours.FirstOrDefault(n => n.direction.y == neighbourDir.y);


        //�÷��̾ y�� �Է��ߴµ� �� ���⿡ neighbour������ x ������ �����ϴ��� üũ
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

                ////�ǹ� ��Ʋ���� �� �θ� ����� �ű⿡�� ������ ����
                //Debug.DrawLine(transform.parent.position, neighbour.climbpoint.transform.parent.position,
                //    neighbour.isTwoWay ? Color.green : Color.red);

            }
        }
    }
}

[System.Serializable]
public class Neighbour
{
    [Tooltip("�� ����")] public ClimbPoint climbpoint;
    [Tooltip("�ִϸ��̼� ��и� �̵�����")] public Vector2 direction;
    [Tooltip("�׼� Ÿ��")] public ConnectionType connectionType;
    [Tooltip("��������� �Դ� ���� �� �� �ִ���")] public bool isTwoWay = true;
}

