using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] List<Room> roomList = new List<Room>();
    [SerializeField] int[][,] roomMatrix = new int[50][,];
    [SerializeField] GameObject startGO, endGO;

    public List<Room> RoomList { get => roomList;}
    public int[][,] RoomMatrix { get => roomMatrix; }

    public void AddRoom(Room room, int[,] place)
    {
        roomList.Add(room);
        roomMatrix[roomList.Count - 1] = new int[1,2] { { place[0,0], place[0,1] } };
    }

    public int[,] GetPositionByRoom(Room room)
    {
        for(int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i] == room) return roomMatrix[i];
        }
        return new int[0,0];
    }
    public bool CheckIsPlaceFree(int[,] place)
    {
        Debug.Log("StageGenerator, GetNextRoomPosition : place checked = [" + place[0, 0] + ", " + place[0, 1] + "]");

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomMatrix[i][0, 0] == place[0, 0] && roomMatrix[i][0, 1] == place[0, 1])
            {

                Debug.Log("StageGenerator, GetNextRoomPosition : place founded = [" + roomMatrix[i][0, 1] + ", " + roomMatrix[i][0, 1] + "]");
                return false;
            }
        }
        return true;
    }

    public void StartSettingUpStage()
    {
        var startRoom = roomList[0];
        var endRoom = roomList[roomList.Count - 1];


        for(int i = 0; i < RoomMatrix.Length; i++)
        {

        }

        startGO = new GameObject("start");
        startGO.transform.position = startRoom.transform.position;

        endGO = new GameObject("end");
        endGO.transform.position = endRoom.transform.position;

    }
    private void OnDrawGizmos()
    {
        if (startGO == null || endGO == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(startGO.transform.position, 3f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endGO.transform.position, 3f);
    }
}
