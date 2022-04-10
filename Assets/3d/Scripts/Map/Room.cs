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

public enum Type
{
    Start,
    End,
    TroupsBonus,
    Default
}


public class Room : MonoBehaviour
{
    [SerializeField] Type roomType = Type.Default;
    [SerializeField] List<Direction> doorsDirections = new List<Direction>();

    [SerializeField] bool _navMeshBuildOnce = false;
    [SerializeField] List<Vector3> doorsPostition = new List<Vector3>();
    [SerializeField] List<GameObject> roomFloorPrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> doorPrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> wallPrefabs = new List<GameObject>();

    [SerializeField] List<MinionSpawner> minionSpawnerList = new List<MinionSpawner>();
    [SerializeField] MinionSpawner minionSpawner;

    public List<Direction> DoorsDirections { get => doorsDirections; set => doorsDirections = value; }
    public List<Vector3> DoorsPostition { get => doorsPostition; set => doorsPostition = value; }
    public Type RoomType { get => roomType; set => roomType = value; }

    private void Awake()
    {
        DoorsDirections = new List<Direction>();
        DoorsPostition = new List<Vector3>();
    }
    public void SetEnnemyCanMove(bool canMove)
    {
        var minions = GetComponentsInChildren<Minion>();
        var sleepingOpponent = new List<Minion>();

        foreach (Minion min in minions)
            if (min.Owner == null) sleepingOpponent.Add(min);

        foreach (Minion min in sleepingOpponent)
            min.Moving = canMove;
    }
    public void GenerateRoom()
    {

        if (!IsContainingDoor(Direction.North))
            Instantiate(wallPrefabs[0], transform.position, doorPrefabs[0].transform.rotation, transform);

        if (!IsContainingDoor(Direction.Est))
            Instantiate(wallPrefabs[1], transform.position, doorPrefabs[0].transform.rotation, transform);

        if (!IsContainingDoor(Direction.South))
            Instantiate(wallPrefabs[2], transform.position, doorPrefabs[0].transform.rotation, transform);

        if (!IsContainingDoor(Direction.West))
            Instantiate(wallPrefabs[3], transform.position, doorPrefabs[0].transform.rotation, transform);

        switch (roomType)
        {
            case Type.Start:
                Instantiate(roomFloorPrefabs[1], transform.position, doorPrefabs[0].transform.rotation, transform);
                break;
            case Type.End:
                Instantiate(roomFloorPrefabs[2], transform.position, doorPrefabs[0].transform.rotation, transform);
                break;
            case Type.TroupsBonus:
                Instantiate(roomFloorPrefabs[3], transform.position, doorPrefabs[0].transform.rotation, transform);
                break;
            case Type.Default:
                var rndSpawn = Random.Range(0, minionSpawnerList.Count);
                minionSpawner = new MinionSpawner();
                minionSpawner.SpawnDataList = new List<SpawnData>();
                minionSpawner.SpawnDataList.AddRange(minionSpawnerList[rndSpawn].SpawnDataList);
                minionSpawner.RoomTransform = this.transform;
                Instantiate(roomFloorPrefabs[0], transform.position, doorPrefabs[0].transform.rotation, transform);
                minionSpawner.SpawnMinion();
                break;
            default:
                break;
        }
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

}
