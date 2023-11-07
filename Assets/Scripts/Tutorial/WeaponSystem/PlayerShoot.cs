using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public static Action shootInput; //�׼� ����
    public static Action reloadInput;

    [SerializeField] KeyCode reloadKey;
    private void Update()
    {
        if (Input.GetMouseButton(0))
            //shootInput Action�� ��������Ʈ��
            //����� �޼ҵ� ����
            shootInput?.Invoke();

        if (Input.GetKeyDown(reloadKey))
            reloadInput?.Invoke();
    }
}
