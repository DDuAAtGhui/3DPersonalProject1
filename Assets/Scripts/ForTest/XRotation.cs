using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class XRotation : MonoBehaviour
{
    [SerializeField] Transform Player;

    private void Update()
    {
        transform.LookAt(new Vector3(Player.transform.position.x, 
            transform.position.y, Player.transform.position.z));
    }
}
