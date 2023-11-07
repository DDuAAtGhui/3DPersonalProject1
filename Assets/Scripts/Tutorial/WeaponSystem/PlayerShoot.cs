using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public static Action shootInput; //액션 선언
    public static Action reloadInput;

    [SerializeField] KeyCode reloadKey;
    private void Update()
    {
        if (Input.GetMouseButton(0))
            //shootInput Action에 델리게이트로
            //등록한 메소드 실행
            shootInput?.Invoke();

        if (Input.GetKeyDown(reloadKey))
            reloadInput?.Invoke();
    }
}
