using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

//FieldOfView 스크립트에 대한 에디터라는것을 명시
[CustomEditor(typeof(FieldOfView))]
public class FieldOfVIewEditor : Editor
{
    private void OnSceneGUI()
    {
        //FieldOfView 스크립트에 접근해 그에 따른
        //에디팅 및 시각화 작업 수행하게 캐스팅
        //target은 에디터 스크립트 내에서 현재 편집중인 대상을 가르키는 키워드
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.white;

        //FieldOfView 클래스를 컴포넌트로 가지고 있는 오브젝트에게
        //360도 호 = 원을 그려줌
        Handles.DrawWireArc(fow.transform.position,
            Vector3.up, Vector3.forward, 360f, fow.viewRadius);

        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        //에디터 스크립트는 오브젝트에 붙어있지않으므로
        //fov 스크립트 붙어있는 오브젝트 트랜스폼 기준으로 벡터값 포인트로
        //선 그리기
        Handles.DrawLine(fow.transform.position,
            fow.transform.position +
            viewAngleA * fow.viewRadius);

        Handles.DrawLine(fow.transform.position,
            fow.transform.position +
            viewAngleB * fow.viewRadius);

    }
}
