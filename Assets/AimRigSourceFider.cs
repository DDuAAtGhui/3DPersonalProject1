using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AimRigSourceFider : MonoBehaviour
{
    CameraController cameraController;

    void Start()
    {
        cameraController = GameObject.FindObjectOfType<CameraController>();

        MultiAimConstraint a = GetComponent<MultiAimConstraint>();

        a.data.sourceObjects.Clear();

        a.data.sourceObjects.Add(new WeightedTransform()
        {
            transform = cameraController.lookAt.transform,
            weight = 1f
        });
    }
}

