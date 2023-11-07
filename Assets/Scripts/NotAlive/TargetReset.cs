using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetReset : MonoBehaviour
{
    [SerializeField] List<PracticeRoomTarget> targets;

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var target in targets)
        {
            target.isDead = false;
            target.health = target.initialHealth;
        }
    }
}
