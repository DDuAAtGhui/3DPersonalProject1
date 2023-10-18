using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectionType
{
    Jump, Move
}

public class ClimbPoint : MonoBehaviour
{
    [SerializeField] List<Neigbour> neigbours;
}

[System.Serializable]
public class Neigbour
{
    public ClimbPoint point;
    public Vector2 direction;
    public ConnectionType connectionType;
    [Tooltip("양방향으로 왔다 갔다 할 수 있는지")] public bool isTwoWay = true;
}

