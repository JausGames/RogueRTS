using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinionSpawner", menuName = "Spawner/Minion", order = 2)]
public class MinionSpawner : ScriptableObject
{
    [SerializeField] List<SpawnData> spawnDataList = new List<SpawnData>();
    Transform roomTransform;

    public Transform RoomTransform { get => roomTransform; set => roomTransform = value; }
    internal List<SpawnData> SpawnDataList { get => spawnDataList; set => spawnDataList = value; }

    public void SpawnMinion()
    {
        foreach (SpawnData data in SpawnDataList)
            Instantiate(data.prefab, RoomTransform.position + data.position, data.rotation, RoomTransform);
    }

}
