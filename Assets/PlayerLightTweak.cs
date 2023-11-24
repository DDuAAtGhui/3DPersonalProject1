using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLightTweak : MonoBehaviour
{
    [SerializeField] float lightCheckSphereRadius = 10f;
    [SerializeField] List<Light> lights;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawSphere(transform.position, lightCheckSphereRadius);
    }
}
