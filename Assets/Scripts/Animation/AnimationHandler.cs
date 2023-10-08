using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    void AnimationEnd()
    {
        GameManager.instance.player.isAnimEnd();
    }
}
