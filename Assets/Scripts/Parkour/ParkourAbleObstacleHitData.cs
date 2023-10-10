using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ParkourAbleObstacleHitData
{
    public bool forwardHitFound;
    public bool heightHitFound;
    public bool backSideHitFound; //�β� üũ��

    public RaycastHit forwardHit;
    public RaycastHit heighHit;
    public RaycastHit backSideHit; //�β� üũ��
}
