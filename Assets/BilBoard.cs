using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilBoard : MonoBehaviour
{
    [SerializeField] Transform cameraTF;
    void Start()
    {
        cameraTF = Camera.main.transform;
    }
    void Update()
    {
        transform.LookAt(transform.position + cameraTF.rotation * Vector3.forward,
            cameraTF.rotation * Vector3.up);
    }
}
