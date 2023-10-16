using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUtil
{
    //레이캐스트 3개니까 List로 out
    public static bool ThreeRayCasts
        (Vector3 origin, Vector3 dir, float RaySpacing, Transform transform,
        out List<RaycastHit> hits, float distance, LayerMask layerMask, bool VisibleRay = true)
    {
        bool centerHitFound =
            Physics.Raycast(origin, dir, out RaycastHit centerHit, distance, layerMask);

        bool leftHitFound =
            Physics.Raycast(origin - transform.right * RaySpacing, dir, out RaycastHit leftHit, distance, layerMask);

        bool rightHitFound =
            Physics.Raycast(origin + transform.right * RaySpacing, dir, out RaycastHit rightHit, distance, layerMask);

        
        hits = new List<RaycastHit>()
        {
            centerHit,leftHit,rightHit,
        };

        bool anyHitFound = centerHitFound || leftHitFound || rightHitFound;

        if (anyHitFound && VisibleRay)
        {
            //DrawLine이 더 사용 쉬움(start-end 포인트라서)
            Debug.DrawLine(origin, centerHit.point, Color.green);
            Debug.DrawLine(origin - transform.right * RaySpacing, leftHit.point, Color.green);
            Debug.DrawLine(origin + transform.right * RaySpacing, rightHit.point, Color.green);
        }

        return anyHitFound;
    }
}
