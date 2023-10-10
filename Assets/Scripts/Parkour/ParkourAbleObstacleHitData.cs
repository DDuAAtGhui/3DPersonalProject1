using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ParkourAbleObstacleHitData
{
    public bool forwardHitFound;
    public bool heightHitFound;
    public bool backSideHitFound; //두께 체크용

    public RaycastHit forwardHit;
    public RaycastHit heighHit;
    public RaycastHit backSideHit; //두께 체크용
}
