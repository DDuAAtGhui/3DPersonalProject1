using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagController : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject, 30f);
    }
}
