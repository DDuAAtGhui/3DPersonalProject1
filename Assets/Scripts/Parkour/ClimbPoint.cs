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
    [Tooltip("��������� �Դ� ���� �� �� �ִ���")] public bool isTwoWay = true;
}

