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
    [SerializeField] Direction[] doorsDirections;
    [SerializeField] Vector3[] doorsPostition;
    [SerializeField] List<GameObject> roomFloorPrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> doorPrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> wallPrefabs = new List<GameObject>();


    // The size of the build bounds
    public Vector3 m_Size = new Vector3(14.0f, 12.0f, 2.0f);
    // The center of the build
    public Transform m_Tracked;
    List<NavMeshBuildSource> m_Sources = new List<NavMeshBuildSource>();
    AsyncOperation m_Operation;
    NavMeshDataInstance m_Instance;
    NavMeshData m_NavMesh;

    public Direction[] DoorsDirections { get => doorsDirections; set => doorsDirections = value; }
    public Vector3[] DoorsPostition { get => doorsPostition; set => doorsPostition = value; }

    private void Awake()
    {
        GenerateRoom();
    }

    IEnumerator Start()
    {
        while (true)
        {
            UpdateNavMesh(true);
            yield return m_Operation;
        }
    }
    void OnEnable()
    {
        // Construct and add navmesh
        m_NavMesh = new NavMeshData();
        m_Instance = NavMesh.AddNavMeshData(m_NavMesh);
        if (m_Tracked == null)
            m_Tracked = transform;
        UpdateNavMesh(false);
    }
    void UpdateNavMesh(bool asyncUpdate = false)
    {
        NavMeshSourceTag.Collect(ref m_Sources);
        var defaultBuildSettings = NavMesh.GetSettingsByID(0);
        var bounds = QuantizedBounds();

        if (asyncUpdate)
            m_Operation = NavMeshBuilder.UpdateNavMeshDataAsync(m_NavMesh, defaultBuildSettings, m_Sources, bounds);
        else
            NavMeshBuilder.UpdateNavMeshData(m_NavMesh, defaultBuildSettings, m_Sources, bounds);
    }
    Bounds QuantizedBounds()
    {
        // Quantize the bounds to update only when theres a 10% change in size
        var center = m_Tracked ? m_Tracked.position : transform.position;
        return new Bounds(Quantize(center, 0.1f * m_Size), m_Size);
    }
    static Vector3 Quantize(Vector3 v, Vector3 quant)
    {
        float x = quant.x * Mathf.Floor(v.x / quant.x);
        float y = quant.y * Mathf.Floor(v.y / quant.y);
        float z = quant.z * Mathf.Floor(v.z / quant.z);
        return new Vector3(x, y, z);
    }
    public void GenerateRoom()
    {
        Instantiate(roomFloorPrefabs[0], transform.position, Quaternion.identity, transform);

        if(IsContainingDoor(Direction.North))
        {
            Instantiate(doorPrefabs[0], transform.position, Quaternion.identity, transform);
            //GridSettings.gridSize
        }
        else
            Instantiate(wallPrefabs[0], transform.position, Quaternion.identity, transform);

        if (IsContainingDoor(Direction.Est))
            Instantiate(doorPrefabs[1], transform.position, Quaternion.identity, transform);
        else
            Instantiate(wallPrefabs[1], transform.position, Quaternion.identity, transform);

        if (IsContainingDoor(Direction.South))
            Instantiate(doorPrefabs[2], transform.position, Quaternion.identity, transform);
        else
            Instantiate(wallPrefabs[2], transform.position, Quaternion.identity, transform);

        if (IsContainingDoor(Direction.West))
            Instantiate(doorPrefabs[3], transform.position, Quaternion.identity, transform);
        else
            Instantiate(wallPrefabs[3], transform.position, Quaternion.identity, transform);
    }

    bool IsContainingDoor(Direction dir)
    {
        foreach(Direction d in doorsDirections)
        {
            if (d == dir) return true;
        }
        return false;
    }
}
