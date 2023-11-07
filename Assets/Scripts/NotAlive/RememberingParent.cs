using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 발사됐을때의 부모 총을 기억
public class RememberingParent : MonoBehaviour
{
    [HideInInspector] public GameObject parent;

    //Awake로 해야 부모값 기억가능
    private void Awake()
    {
        parent = gameObject.transform.parent.gameObject;
    }
}
