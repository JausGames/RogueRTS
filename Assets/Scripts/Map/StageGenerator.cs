using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    [SerializeField] List<GameObject> roomPrefabs = new List<GameObject>();
    [SerializeField] List<Direction[]> roomDoorsDirections = new List<Direction[]>();
    [SerializeField] List<Vector3[]> roomDoorPositions = new List<Vector3[]>();
    [SerializeField] int roomCount = 20;
    [SerializeField] Stage stage;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CreateStage());
    }

    bool CheckIfRoomIsSuitable (Room currentRoom, Room checkedRoom, ref List<Direction> dirOut, ref List<Direction> dirIn, ref List<Vector3> doorPos, ref List<Room> retainedRoom, int[,] currentPos)
    { 
        for (int i = 0; i < checkedRoom.DoorsDirections.Length; i++)
        {
            var currIn = checkedRoom.DoorsDirections[i];
            var currOut = GetOppositeDirection(currIn);
            var currPos = -checkedRoom.DoorsPostition[i];

            if (IsDirectionInList(currOut, currentRoom.DoorsDirections) &&
                CheckIfDirection(currOut, currIn) 
                && stage.CheckIsPlaceFree(GetNextRoomPosition(currentPos, currOut))
                )
            {
                dirIn.Add(currIn);
                dirOut.Add(currOut);
                doorPos.Add(currPos);
                retainedRoom.Add(checkedRoom);
            }
        }
        if (dirIn.Count > 0) return true;
        else return false;
    }
    IEnumerator CreateStage()
    {

        for (int i = 0; i < roomPrefabs.Count; i++)
        {
            roomDoorsDirections.Add(roomPrefabs[i].GetComponent<Room>().DoorsDirections);
            roomDoorPositions.Add(roomPrefabs[i].GetComponent<Room>().DoorsPostition);
        }

        var openDoors = 0;
        var openDoorDirections = new List<Direction>();
        var emptySpots = new List<int[,]>();
        var currentRoomCount = 0;
        Room currentRoom = null;

        while (currentRoomCount < roomCount || emptySpots.Count > 0)
        {

            if (currentRoomCount == 0)
            {
                currentRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count - 1)], Vector3.zero, Quaternion.identity, stage.transform).GetComponent<Room>();
                currentRoom.gameObject.name += currentRoomCount;

                //openDoors += currentRoom.DoorsDirections.Length;
                var defaultSpot = new int[1, 2] { { 0, 0 } };
                for (int i = 0; i < currentRoom.DoorsDirections.Length; i++)
                {
                    openDoors++;
                    openDoorDirections.Add(currentRoom.DoorsDirections[i]);
                    var newEmptySpot = GetNextRoomPosition(defaultSpot, currentRoom.DoorsDirections[i]);
                    emptySpots.Add(newEmptySpot);
                }

                stage.AddRoom(currentRoom, defaultSpot);
                currentRoomCount++;
                Debug.Log("StageGenerator, Start : first room = " + currentRoom.gameObject.ToString());
            }
            else if (emptySpots.Count >= Mathf.Floor(roomCount - currentRoomCount))
            {
                //for(int i = 0; i < emptySpots.Count; i++)
                //{
                var i = 0;
                yield return new WaitForSeconds(0.2f);
                List<Direction> neededDirection = new List<Direction>();
                for(int j = 0; j < emptySpots.Count; j++)
                {
                    if (emptySpots[j] == emptySpots[0])
                        neededDirection.Add(GetOppositeDirection(openDoorDirections[i]));
                }
                var emptySpotPosition = (float)emptySpots[0][0, 0] * Vector3.right * 6.5f + (float)emptySpots[0][0, 1] * Vector3.up * 5.5f;

                var prefab = FindSuitableRoom(neededDirection);

                currentRoom = Instantiate(prefab, emptySpotPosition * 2f, Quaternion.identity, stage.transform).GetComponent<Room>();
                currentRoom.gameObject.name += currentRoomCount;
                stage.AddRoom(currentRoom, emptySpots[0]);

                Debug.Log("StageGenerator, Start : new room = " + currentRoom.gameObject.ToString());
                Debug.Log("StageGenerator, Start : openDoors = " + openDoors);
                emptySpots.RemoveAt(0);
                openDoors--;
                openDoorDirections.RemoveAt(0);
                currentRoomCount++;
                //}
            }
            else if (currentRoom != null)
            {
                if (currentRoom.DoorsDirections.Length == 1 && currentRoomCount != 1)
                {
                    Debug.Log("StageGenerator, Start : just 1 door " + currentRoom.gameObject.ToString());
                    currentRoom = null;
                }
                else
                {
                    var dirIn = new List<Direction>();
                    var dirOut = new List<Direction>();
                    var doorPos = new List<Vector3>();
                    var doorMatrixPos = stage.GetPositionByRoom(currentRoom);

                    var retainedRoom = new List<Room>();
                    for (int i = 0; i < roomPrefabs.Count; i++)
                    {
                        var room = roomPrefabs[i].GetComponent<Room>();
                        CheckIfRoomIsSuitable(currentRoom, room, ref dirOut, ref dirIn, ref doorPos, ref retainedRoom, doorMatrixPos);

                    }
                    if (retainedRoom.Count > 0)
                    {
                        yield return new WaitForSeconds(0.2f);
                        var pickedRoomNb = Random.Range(0, retainedRoom.Count - 1);
                        currentRoom = Instantiate(retainedRoom[pickedRoomNb], doorPos[pickedRoomNb] * 2 + currentRoom.transform.position, Quaternion.identity, stage.transform).GetComponent<Room>();
                        currentRoom.gameObject.name += currentRoomCount;
                        var currentRoomPos = GetNextRoomPosition(doorMatrixPos, dirOut[pickedRoomNb]);
                        stage.AddRoom(currentRoom, currentRoomPos);

                        //openDoors += currentRoom.DoorsDirections.Length - 2;
                        //Have to close the previous room door that is used

                        for (int i = 0; i < emptySpots.Count; i++)
                        {
                            // SOMETIMES IT DOES NOT DELETE ALL EMPTY SLOT
                            if (emptySpots[i][0,0] == currentRoomPos[0, 0]
                                && emptySpots[i][0, 1] == currentRoomPos[0, 1]
                                //&& openDoorDirections[i] == dirOut[pickedRoomNb]
                                )
                            {
                                Debug.Log("StageGenerator, Start : close emptySpots i = " + emptySpots[i][0,0] + " ," + +emptySpots[i][0, 1]);
                                openDoors--;
                                emptySpots.RemoveAt(i);
                                openDoorDirections.RemoveAt(i);
                            };
                        }

                        for (int i = 0; i < currentRoom.DoorsDirections.Length; i++)
                        {
                            if(currentRoom.DoorsDirections[i] != GetOppositeDirection(dirOut[pickedRoomNb]) 
                                && stage.CheckIsPlaceFree(GetNextRoomPosition(currentRoomPos, currentRoom.DoorsDirections[i])))
                            {
                                openDoors++;
                                openDoorDirections.Add(currentRoom.DoorsDirections[i]);
                                var newEmptySpot = GetNextRoomPosition(currentRoomPos, currentRoom.DoorsDirections[i]);
                                emptySpots.Add(newEmptySpot);
                            }
                        }

                        Debug.Log("StageGenerator, Start : new room = " + currentRoom.gameObject.ToString());
                        Debug.Log("StageGenerator, Start : openDoors = " + openDoors);
                        currentRoomCount++;
                    }
                    else
                    {
                        Debug.Log("StageGenerator, Start : no room available = " + currentRoom.gameObject.ToString());
                        currentRoom = null;
                    }
                }

            }
            else if (currentRoom == null)
            {
                Debug.Log("StageGenerator, Start : room is null");
                if (stage.RoomList.Count == 0) break;
                currentRoom = stage.RoomList[Random.Range(0, stage.RoomList.Count - 1)];
                Debug.Log("StageGenerator, Start : picked room = " + currentRoom.gameObject.ToString());
            }

            if (openDoors == 0)
            {
                Debug.Log("StageGenerator, Start : no open doors");
                break;
            }
            else if (openDoors >= Mathf.Floor(roomCount - currentRoomCount))
            {
                //Start closing all doors

            }
        }
        stage.StartSettingUpStage();
    }

    private GameObject FindSuitableRoom(List<Direction> neededDirection)
    {

        for(int i = 0; i < roomPrefabs.Count; i++)
        {
            var room = roomPrefabs[i].GetComponent<Room>();
            if(room.DoorsDirections.Length == neededDirection.Count)
            {
                var checkedDir = 0;
                for(int j = 0; j < neededDirection.Count; j++)
                {
                    if (IsDirectionInList(neededDirection[j], room.DoorsDirections)) checkedDir++;
                }
                if (checkedDir == neededDirection.Count) return roomPrefabs[i];
            }
        }
        return null;
    }

    bool IsDirectionInList(Direction dirChecked, Direction[] Roomdirections)
    {
        foreach(Direction dir in Roomdirections)
        {
            if (dir == dirChecked) return true;
        }
    return false;
    }
    bool IsDirectionInList(Direction dirChecked, List<Direction> Roomdirections)
    {
        foreach (Direction dir in Roomdirections)
        {
            if (dir == dirChecked) return true;
        }
        return false;
    }
    bool CheckIfDirection(Direction dirOut, Direction dirIn)
    {
        switch (dirOut, dirIn)
        {
            case (Direction.North, Direction.South):
            case (Direction.Est, Direction.West):
            case (Direction.South, Direction.North):
            case (Direction.West, Direction.Est):
                return true;
            default:
                return false;
        }
    }
    Direction GetOppositeDirection(Direction dirOut)
    {
        switch (dirOut)
        {
            case Direction.North:
                return Direction.South;
            case Direction.Est:
                return Direction.West;
            case Direction.South:
                return Direction.North;
            case Direction.West:
                return Direction.Est;
            default:
                return Direction.None;
        }
    }


    int[,] GetNextRoomPosition(int[,] currentposition, Direction direction)
    {
        var newPos = new int[1,2];
        newPos[0, 0] = currentposition[0, 0];
        newPos[0, 1] = currentposition[0, 1];

        switch (direction)
        {
            case Direction.North:
                newPos[0, 1]++;
                return newPos;
            case Direction.Est:
                newPos[0, 0]++;
                return newPos;
            case Direction.South:
                newPos[0, 1]--;
                return newPos;
            case Direction.West:
                newPos[0, 0]--;
                return newPos;
            default:
                return currentposition;
        }
    }

}
