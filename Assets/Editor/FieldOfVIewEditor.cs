using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

//FieldOfView ��ũ��Ʈ�� ���� �����Ͷ�°��� ���
[CustomEditor(typeof(FieldOfView))]
public class FieldOfVIewEditor : Editor
{
    private void OnSceneGUI()
    {
        //FieldOfView ��ũ��Ʈ�� ������ �׿� ����
        //������ �� �ð�ȭ �۾� �����ϰ� ĳ����
        //target�� ������ ��ũ��Ʈ ������ ���� �������� ����� ����Ű�� Ű����
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.white;

        //FieldOfView Ŭ������ ������Ʈ�� ������ �ִ� ������Ʈ����
        //360�� ȣ = ���� �׷���
        Handles.DrawWireArc(fow.transform.position,
            Vector3.up, Vector3.forward, 360f, fow.viewRadius);

        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        //������ ��ũ��Ʈ�� ������Ʈ�� �پ����������Ƿ�
        //fov ��ũ��Ʈ �پ��ִ� ������Ʈ Ʈ������ �������� ���Ͱ� ����Ʈ��
        //�� �׸���
        Handles.DrawLine(fow.transform.position,
            fow.transform.position +
            viewAngleA * fow.viewRadius);

        Handles.DrawLine(fow.transform.position,
            fow.transform.position +
            viewAngleB * fow.viewRadius);

    }
}
