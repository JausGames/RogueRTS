using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StageGenerator : MonoBehaviour
{
    [SerializeField] NavMeshSurface2d surface2D;
    [SerializeField] List<GameObject> roomPrefabs = new List<GameObject>();
    [SerializeField] List<Direction[]> roomDoorsDirections = new List<Direction[]>();
    [SerializeField] List<Vector3[]> roomDoorPositions = new List<Vector3[]>();
    [SerializeField] int roomCount = 10;
    [SerializeField] bool buildOnce = false;
    //Vector2 GridSetting.gridSize = new Vector2(6.75f, 5.75f);
    [SerializeField] Stage stage;

    

    IEnumerator Start()
    {
        while (true)
        {
            if(buildOnce) surface2D.UpdateNavMesh(surface2D.navMeshData);
            yield return null;
        }
    }

    // Start is called before the first frame update
    void Awake()
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

            if (IsDirectionInList(currOut, currentRoom.DoorsDirections)
                && stage.CheckIsPlaceFree(GetNextRoomPosition(currentPos, currOut))
                && stage.CheckNextDoorRoom(GetNextRoomPosition(currentPos, currOut), checkedRoom.DoorsDirections)
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
                yield return new WaitForSeconds(0.2f);
                List<Direction> neededDirection = new List<Direction>();
                var emptySpotFounded = new List<int[,]>();
                for(int j = 0; j < emptySpots.Count; j++)
                {
                    if (emptySpots[j][0, 0] == emptySpots[0][0, 0] && emptySpots[j][0, 1] == emptySpots[0][0, 1])
                    {
                        emptySpotFounded.Add(emptySpots[j]);
                        neededDirection.Add(GetOppositeDirection(openDoorDirections[j]));
                    }
                }
                var emptySpotPosition =(float)emptySpots[0][0, 0] * Vector3.right * (GridSettings.gridSize.x - GridSettings.wallSize) + (float)emptySpots[0][0, 1] * Vector3.up * (GridSettings.gridSize.y - GridSettings.wallSize);

                var prefab = FindSuitableRoom(neededDirection);

                currentRoom = Instantiate(prefab, emptySpotPosition, Quaternion.identity, stage.transform).GetComponent<Room>();
                currentRoom.gameObject.name += currentRoomCount;
                stage.AddRoom(currentRoom, emptySpots[0]);

                Debug.Log("StageGenerator, Start : CLOSING EMPTY SPOTS = " + currentRoom.gameObject.ToString());
                for(int i = 0; i < emptySpotFounded.Count; i++)
                {
                    var id = emptySpots.IndexOf(emptySpotFounded[i]);
                    emptySpots.Remove(emptySpotFounded[i]);
                    openDoors--;
                    openDoorDirections.RemoveAt(id);
                }
                currentRoomCount++;
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
                        var currentRoomPos = GetNextRoomPosition(doorMatrixPos, dirOut[pickedRoomNb]); 
                        var emptySpotPosition = (float)currentRoomPos[0, 0] * Vector3.right * (GridSettings.gridSize.x - GridSettings.wallSize) + (float)currentRoomPos[0, 1] * Vector3.up * (GridSettings.gridSize.y - GridSettings.wallSize);

                        currentRoom = Instantiate(retainedRoom[pickedRoomNb], emptySpotPosition, Quaternion.identity, stage.transform).GetComponent<Room>();
                        currentRoom.gameObject.name += currentRoomCount;
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

                        Debug.Log("StageGenerator, Start : NEW ROOM = " + currentRoom.gameObject.ToString());
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

        yield return new WaitForSeconds(0.2f);

        surface2D.BuildNavMeshAsync();

        yield return new WaitForSeconds(0.2f);

        buildOnce = true;


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
