using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    //�þ� ���� ����
    public float viewRadius = 3f;
    [Range(0, 360)] public float viewAngle;

    //��ǥ�� ���̾�� ��ֹ� ���̾�
    public LayerMask targetMask, obstacleMask;

    //�þ� �ȿ� ���� target ���̾� ���̷� Ʈ������ �޾Ƽ� �����Ұ�
    public List<Transform> targetsInSight = new List<Transform>();

    private void Start()
    {
        StartCoroutine(FindTargets(0.1f));
    }

    //���� �������� Ÿ�� Ž��
    IEnumerator FindTargets(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            FindTargetsInSight();
        }
    }

    //Ÿ�� Ž�� �޼ҵ�
    void FindTargetsInSight()
    {
        targetsInSight.Clear();

        //viewRadius��ŭ�� ������ ������ �� �����ؼ�
        //�� �ȿ� ���� targetMask ���̾� �� �ݶ��̴� �� �޾ƿ�
        Collider[] targetsInViewRadius =
            Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;

            //���� Ÿ���� �ٶ󺸴� ����
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            //�÷��̾� �� ����� �÷��̾ �ٶ󺸴� ���� ���� ������ ������ ���� ���̸�
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                //Ÿ�ٰ��� �Ÿ� �޾ƿ���
                float distanceToTarget =
                    Vector3.Distance(transform.position, target.transform.position);

                //Ž���� Ÿ�ٿ��� ���� ���� ��ֹ� �ƴϸ� �þ� �� Ÿ�� ����Ʈ�� �߰�
                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
                {
                    targetsInSight.Add(target);
                }
            }
        }
    }

    //GUI ǥ�� ����
    public Vector3 DirFromAngle(float angleDegree, bool angleIsGlobal)
    {
        //������ �۷ι� �ƴϸ� �۷ι��� ��ȯ
        if (!angleIsGlobal)
        {
            angleDegree += transform.eulerAngles.y;
        }
        //�ﰢ�Լ����� ���п��� ǥ���������� ���߱����� ����ȭ
        //������ �߽����� �ϴ� ����ǥ�鿡 ���� ����
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
