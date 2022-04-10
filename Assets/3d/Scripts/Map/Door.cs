using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    [SerializeField] List<Room> connectedRooms = new List<Room>();
    private void OnDestroy()
    {
        /*var room = GetComponentInParent<Room>();
        room.CalculateNavMesh();*/

        foreach(Room room in connectedRooms)
        {
            room.SetEnnemyCanMove(true);
        }

        var stageGen = FindObjectOfType<StageGenerator>();
        Debug.Log("Door broken");
        if(stageGen) stageGen.UpdateNavMesh();

    }
    public void SetConnectedRoom(Room room1, Room room2)
    {
        connectedRooms.Add(room1);
        connectedRooms.Add(room2);
    }
}

