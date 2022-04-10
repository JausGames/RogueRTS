using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] List<Direction> doorsDirections = new List<Direction>();
    [SerializeField] bool _navMeshBuildOnce = false;
    [SerializeField] List<Vector3> doorsPostition = new List<Vector3>();
    [SerializeField] List<GameObject> roomFloorPrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> doorPrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> wallPrefabs = new List<GameObject>();

    public List<Direction> DoorsDirections { get => doorsDirections; set => doorsDirections = value; }
    public List<Vector3> DoorsPostition { get => doorsPostition; set => doorsPostition = value; }

    private void Awake()
    {
        DoorsDirections = new List<Direction>();
        DoorsPostition = new List<Vector3>();
        //GenerateRoom();
    }

    public void GenerateRoom()
    {
        Instantiate(roomFloorPrefabs[0], transform.position, doorPrefabs[0].transform.rotation, transform);

        if (!IsContainingDoor(Direction.North))
            Instantiate(wallPrefabs[0], transform.position, doorPrefabs[0].transform.rotation, transform);

        if (!IsContainingDoor(Direction.Est))
            Instantiate(wallPrefabs[1], transform.position, doorPrefabs[0].transform.rotation, transform);

        if (!IsContainingDoor(Direction.South))
            Instantiate(wallPrefabs[2], transform.position, doorPrefabs[0].transform.rotation, transform);

        if (!IsContainingDoor(Direction.West))
            Instantiate(wallPrefabs[3], transform.position, doorPrefabs[0].transform.rotation, transform);

        CalculateNavMesh();
    }

    bool IsContainingDoor(Direction dir)
    {
        if (doorsDirections.Count == 0) return false;
        foreach (Direction d in doorsDirections)
        {
            if (d == dir) return true;
        }
        return false;
    }
    public Bounds GetBounds()
    {
        return new Bounds(Vector3.zero, new Vector3(14f, 1f, 12f));
    }

    public void CalculateNavMesh()
    {
        //var surface2d = GetComponent<NavMeshSurface2d>();
        var surface = GetComponent<NavMeshSurface>();
        //surface2d.UpdateNavMesh(surface2d.navMeshData);


        /*var sources = NavMeshBuilder.CollectSources()
;
        var boundsCenter = Vector3.right * transform.position.x + Vector3.up * transform.position.z + Vector3.forward * transform.position.y;
        var sourcesBounds = new Bounds(Vector3.zero, new Vector3(14f, 1f, 12f));

        Debug.Log("Room, CalculateNavMesh : sourcesBounds = " + sourcesBounds);

            var data = NavMeshBuilder.BuildNavMeshData(surface.GetBuildSettings(),
                    sources, sourcesBounds, transform.position, transform.rotation);


            if (data != null)
            {
                data.name = gameObject.name;
                surface.RemoveData();
                surface.navMeshData = data;
                if (isActiveAndEnabled)
                    surface.AddData();
            }*/
            
    }
}
