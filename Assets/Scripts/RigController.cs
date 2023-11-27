using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigController : MonoBehaviour
{
    [HideInInspector] public Rig rig;
    Player player;

    float targetWeight;
    [Header("Option")]
    [SerializeField] bool AimFeature;
    private void Awake()
    {
        player = GetComponentInParent<Player>();
        rig = GetComponent<Rig>();
    }

    private void Update()
    {
        if (AimFeature)
        {
            rig.weight =
            Mathf.Lerp(rig.weight, targetWeight, 10 * Time.deltaTime);

            if (player._InputAim)
                targetWeight = 1f;

            else
                targetWeight = 0f;
        }
    }

}
