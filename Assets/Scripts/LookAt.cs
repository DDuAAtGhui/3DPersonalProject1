using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] GameObject lookAt;
    Vector3 lookAtInitialPosition;
    [SerializeField] CinemachineFreeLook freelook;
    [SerializeField] CinemachineVirtualCamera aimCamera;
    private void Awake()
    {
        lookAtInitialPosition = lookAt.transform.position;
    }
    void Update()
    {
        lookAt.SetActive(aimCamera.isActiveAndEnabled);

        if (!lookAt.activeSelf)
            lookAt.transform.position = lookAtInitialPosition;

    }
}
