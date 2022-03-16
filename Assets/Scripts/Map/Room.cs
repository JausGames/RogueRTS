using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    North,
    Est,
    South,
    West,
    None
}


public class Room : MonoBehaviour
{
    [SerializeField] Direction[] doorsDirections;
    [SerializeField] Vector3[] doorsPostition;

    public Direction[] DoorsDirections { get => doorsDirections; set => doorsDirections = value; }
    public Vector3[] DoorsPostition { get => doorsPostition; set => doorsPostition = value; }
}
