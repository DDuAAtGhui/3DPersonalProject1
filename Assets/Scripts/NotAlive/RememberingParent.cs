using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �߻�������� �θ� ���� ���
public class RememberingParent : MonoBehaviour
{
    [HideInInspector] public GameObject parent;

    //Awake�� �ؾ� �θ� ��ﰡ��
    private void Awake()
    {
        parent = gameObject.transform.parent.gameObject;
    }
}
