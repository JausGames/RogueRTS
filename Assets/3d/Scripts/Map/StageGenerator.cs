using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


public class StageGenerator : MonoBehaviour
{
    [SerializeField] NavMeshSurface surface;
    [SerializeField] List<GameObject> roomPrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> doorPrefabs = new List<GameObject>();
    [SerializeField] List<Direction[]> roomDoorsDirections = new List<Direction[]>();


    [SerializeField] List<Vector3[]> roomDoorPositions = new List<Vector3[]>();
    [SerializeField] int pathLenght = 10;
    [SerializeField] private int minRoom = 25;
    [SerializeField] Stage stage;

    void AppendModifierVolumes(ref List<NavMeshBuildSource> sources)
    {
#if UNITY_EDITOR
        var myStage = StageUtility.GetStageHandle(gameObject);
        if (!myStage.IsValid())
            return;
#endif
        // Modifiers
        List<NavMeshModifierVolume> modifiers;
        if (surface.collectObjects == CollectObjects.Children)
        {
            modifiers = new List<NavMeshModifierVolume>(GetComponentsInChildren<NavMeshModifierVolume>());
            modifiers.RemoveAll(x => !x.isActiveAndEnabled);
        }
        else
        {
            modifiers = NavMeshModifierVolume.activeModifiers;
        }

        foreach (var m in modifiers)
        {
            if ((surface.layerMask & (1 << m.gameObject.layer)) == 0)
                continue;
            if (!m.AffectsAgentType(surface.agentTypeID))
                continue;
#if UNITY_EDITOR
            if (!myStage.Contains(m.gameObject))
                continue;
#endif
            var mcenter = m.transform.TransformPoint(m.center);
            var scale = m.transform.lossyScale;
            var msize = new Vector3(m.size.x * Mathf.Abs(scale.x), m.size.y * Mathf.Abs(scale.y), m.size.z * Mathf.Abs(scale.z));

            var src = new NavMeshBuildSource();
            src.shape = NavMeshBuildSourceShape.ModifierBox;
            src.transform = Matrix4x4.TRS(mcenter, m.transform.rotation, Vector3.one);
            src.size = msize;
            src.area = m.area;
            sources.Add(src);
        }
    }
    List<NavMeshBuildSource> CollectSources()
    {
        var sources = new List<NavMeshBuildSource>();
        var markups = new List<NavMeshBuildMarkup>();

        List<NavMeshModifier> modifiers;
        if (surface.collectObjects == CollectObjects.Children)
        {
            modifiers = new List<NavMeshModifier>(GetComponentsInChildren<NavMeshModifier>());
            modifiers.RemoveAll(x => !x.isActiveAndEnabled);
        }
        else
        {
            modifiers = NavMeshModifier.activeModifiers;
        }

        foreach (var m in modifiers)
        {
            if ((surface.layerMask & (1 << m.gameObject.layer)) == 0)
                continue;
            if (!m.AffectsAgentType(surface.agentTypeID))
                continue;
            var markup = new NavMeshBuildMarkup();
            markup.root = m.transform;
            markup.overrideArea = m.overrideArea;
            markup.area = m.area;
            markup.ignoreFromBuild = m.ignoreFromBuild;
            markups.Add(markup);
        }

#if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            if (surface.collectObjects == CollectObjects.All)
            {
                UnityEditor.AI.NavMeshBuilder.CollectSourcesInStage(
                    null, surface.layerMask, surface.useGeometry, surface.defaultArea, markups, gameObject.scene, sources);
            }
            else if (surface.collectObjects == CollectObjects.Children)
            {
                UnityEditor.AI.NavMeshBuilder.CollectSourcesInStage(
                    transform, surface.layerMask, surface.useGeometry, surface.defaultArea, markups, gameObject.scene, sources);
            }
            else if (surface.collectObjects == CollectObjects.Volume)
            {
                Matrix4x4 localToWorld = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                var worldBounds = GetWorldBounds(localToWorld, new Bounds(surface.center, surface.size));

                UnityEditor.AI.NavMeshBuilder.CollectSourcesInStage(
                    worldBounds, surface.layerMask, surface.useGeometry, surface.defaultArea, markups, gameObject.scene, sources);
            }
        }
        else
#endif
        {
            if (surface.collectObjects == CollectObjects.All)
            {
                NavMeshBuilder.CollectSources(null, surface.layerMask, surface.useGeometry, surface.defaultArea, markups, sources);
            }
            else if (surface.collectObjects == CollectObjects.Children)
            {
                NavMeshBuilder.CollectSources(transform, surface.layerMask, surface.useGeometry, surface.defaultArea, markups, sources);
            }
            else if (surface.collectObjects == CollectObjects.Volume)
            {
                Matrix4x4 localToWorld = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                var worldBounds = GetWorldBounds(localToWorld, new Bounds(surface.center, surface.size));
                NavMeshBuilder.CollectSources(worldBounds, surface.layerMask, surface.useGeometry, surface.defaultArea, markups, sources);
            }
        }

        if (surface.ignoreNavMeshAgent)
            sources.RemoveAll((x) => (x.component != null && x.component.gameObject.GetComponent<NavMeshAgent>() != null));

        if (surface.ignoreNavMeshObstacle)
            sources.RemoveAll((x) => (x.component != null && x.component.gameObject.GetComponent<NavMeshObstacle>() != null));

        AppendModifierVolumes(ref sources);

        return sources;
    }
    static Vector3 Abs(Vector3 v)
    {
        return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
    }
    static Bounds GetWorldBounds(Matrix4x4 mat, Bounds bounds)
    {
        var absAxisX = Abs(mat.MultiplyVector(Vector3.right));
        var absAxisY = Abs(mat.MultiplyVector(Vector3.up));
        var absAxisZ = Abs(mat.MultiplyVector(Vector3.forward));
        var worldPosition = mat.MultiplyPoint(bounds.center);
        var worldSize = absAxisX * bounds.size.x + absAxisY * bounds.size.y + absAxisZ * bounds.size.z;
        return new Bounds(worldPosition, worldSize);
    }

    Bounds CalculateWorldBounds(List<NavMeshBuildSource> sources)
    {
        // Use the unscaled matrix for the NavMeshSurface
        Matrix4x4 worldToLocal = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        worldToLocal = worldToLocal.inverse;

        var result = new Bounds();
        foreach (var src in sources)
        {
            switch (src.shape)
            {
                case NavMeshBuildSourceShape.Mesh:
                    {
                        var m = src.sourceObject as Mesh;
                        result.Encapsulate(GetWorldBounds(worldToLocal * src.transform, m.bounds));
                        break;
                    }
                case NavMeshBuildSourceShape.Terrain:
                    {
                        // Terrain pivot is lower/left corner - shift bounds accordingly
                        var t = src.sourceObject as TerrainData;
                        result.Encapsulate(GetWorldBounds(worldToLocal * src.transform, new Bounds(0.5f * t.size, t.size)));
                        break;
                    }
                case NavMeshBuildSourceShape.Box:
                case NavMeshBuildSourceShape.Sphere:
                case NavMeshBuildSourceShape.Capsule:
                case NavMeshBuildSourceShape.ModifierBox:
                    result.Encapsulate(GetWorldBounds(worldToLocal * src.transform, new Bounds(Vector3.zero, src.size)));
                    break;
            }
        }
        // Inflate the bounds a bit to avoid clipping co-planar sources
        result.Expand(0.1f);
        return result;
    }
    public void UpdateNavMesh()
    {
        var data = surface.navMeshData;
        var setting = surface.GetBuildSettings();
        var sources = CollectSources();
        var bounds = CalculateWorldBounds(sources);
        NavMeshBuilder.UpdateNavMeshDataAsync(data, setting, sources, bounds);
    }


    void Awake()
    {
        StartCoroutine(CreateStage());
    }

    bool CheckIfRoomIsSuitable(Room currentRoom, Room checkedRoom, ref List<Direction> dirOut, ref List<Direction> dirIn, ref List<Vector3> doorPos, ref List<Room> retainedRoom, int[,] currentPos)
    {
        for (int i = 0; i < checkedRoom.DoorsDirections.Count; i++)
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
    class DoorNeedData
    {
        public List<Room> roomList = new List<Room>();
        public List<Vector3> posList = new List<Vector3>();
        public List<Direction> dirList = new List<Direction>();

        public void AddNeededDoor(Room room, Vector3 pos, Direction dir)
        {
            for(int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i] == room && dirList[i] == dir) return; 
            }
            roomList.Add(room);
            posList.Add(pos);
            dirList.Add(dir);
        }
    }
    IEnumerator CreateStage()
    {

        /*for (int i = 0; i < roomPrefabs.Count; i++)
        {
            roomDoorsDirections.Add(roomPrefabs[i].GetComponent<Room>().DoorsDirections);
            roomDoorPositions.Add(roomPrefabs[i].GetComponent<Room>().DoorsPostition);
        }*/

        Debug.Log("StageGenerator, GenerateStage : Start");
        Debug.Log("StageGenerator, GenerateStage : min room = " + minRoom);
        Debug.Log("StageGenerator, GenerateStage : path lenght = " + pathLenght);

        var doorData = new DoorNeedData();

        var halfLength = Random.Range(1, pathLenght - 2);
        //var sign = Mathf.Sign(Random.Range(-1f, 1f));
        var signX = (int)Mathf.Sign(Random.Range(-1, 1));
        var signY = (int)Mathf.Sign(Random.Range(-1, 1));

        var startRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count - 1)], Vector3.zero, roomPrefabs[0].transform.rotation, stage.transform).GetComponent<Room>();
        var endRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count - 1)],
            signX * halfLength * GridSettings.gridSize.x * Vector3.right + (pathLenght - halfLength) * signY * GridSettings.gridSize.y * Vector3.forward,
            roomPrefabs[0].transform.rotation, stage.transform).GetComponent<Room>();

        var startPos = new int[,] { { 0, 0 } };
        var endPos = new int[,] { { signX * halfLength, (pathLenght - halfLength) * signY } };

        Debug.Log("StageGenerator, GenerateStage : start pos = [" + (startPos[0, 0]) + "," + (startPos[0, 1]) + "]");
        Debug.Log("StageGenerator, GenerateStage : end pos = [" + (endPos[0, 0]) + "," + (endPos[0, 1]) + "]");

        StageTrace.Trace("StageGenerator, GenerateStage : start pos = [" + (startPos[0, 0]) + "," + (startPos[0, 1]) + "]");
        StageTrace.Trace("StageGenerator, GenerateStage : end pos = [" + (endPos[0, 0]) + "," + (endPos[0, 1]) + "]");

        stage.AddRoom(startRoom, startPos);

        CreatePath(doorData, startRoom, endRoom, startPos, endPos);

        while(stage.RoomList.Count <= minRoom)
        {
            //var randomPathLenght = Random.Range(1, Mathf.Min(6, minRoom - stage.RoomList.Count));
            var randomPathLenght = Random.Range(1, Mathf.Min(6, 6));

            halfLength = Random.Range(1, randomPathLenght);
            //var sign = Mathf.Sign(Random.Range(-1f, 1f));
            signX = (int)Mathf.Sign(Random.Range(-1, 1));
            signY = (int)Mathf.Sign(Random.Range(-1, 1));


            StageTrace.Trace("StageGenerator, GenerateStage : stage.RoomList.Count = " + stage.RoomList.Count);
            StageTrace.Trace("StageGenerator, GenerateStage : place free ? " + stage.CheckIsPlaceFree(new int[,] { { startPos[0, 0] + halfLength * signX, startPos[0, 1] + (randomPathLenght - halfLength) * signY } }));

            if (stage.CheckIsPlaceFree(new int[,] { { startPos[0,0] + halfLength * signX, startPos[0, 1] + (randomPathLenght - halfLength) * signY } }))
            { 

                var roomId = Random.Range(0, stage.RoomList.Count - 1);
                startRoom = stage.RoomList[roomId];
                endRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count - 1)],
                    startRoom.transform.position + signX * halfLength * GridSettings.gridSize.x * Vector3.right + (randomPathLenght - halfLength) * signY * GridSettings.gridSize.y * Vector3.forward,
                    roomPrefabs[0].transform.rotation, stage.transform).GetComponent<Room>();
                startPos = stage.SpotList[roomId];
                endPos = new int[,] { { startPos[0, 0] + signX * halfLength, startPos[0, 1] + (randomPathLenght - halfLength) * signY } };

                StageTrace.Trace("StageGenerator, GenerateStage : Create path");
                StageTrace.Trace("StageGenerator, GenerateStage : start pos = [" + (startPos[0, 0]) + "," + (startPos[0, 1]) + "]");
                StageTrace.Trace("StageGenerator, GenerateStage : end pos = [" + (endPos[0, 0]) + "," + (endPos[0, 1]) + "]");
                CreatePath(doorData, startRoom, endRoom, startPos, endPos);
            }
        }

        CreateDoors(doorData);

        stage.StartSettingUpStage();

        yield return new WaitForSeconds(0.2f);

        surface.BuildNavMesh();

        StageTrace.ExportTrace();
        yield return null;
    }

    private void CreateDoors(DoorNeedData doorData)
    {
        for (int i = 0; i < doorData.roomList.Count; i++)
        {
            switch (doorData.dirList[i])
            {
                case Direction.North:

                    StageTrace.Trace("StageGenerator, CreateDoors : roomList["+i+ "] = [" + doorData.roomList[i].transform.position.x / GridSettings.gridSize.x + "," + doorData.roomList[i].transform.position.y / GridSettings.gridSize.y + "]");
                    StageTrace.Trace("StageGenerator, CreateDoors : door NORTH"); 
                    Instantiate(doorPrefabs[0], doorData.roomList[i].transform.position, doorPrefabs[0].transform.rotation, doorData.roomList[i].transform);
                    break;
                case Direction.Est:
                    StageTrace.Trace("StageGenerator, CreateDoors : roomList[" + i + "] = [" + doorData.roomList[i].transform.position.x / GridSettings.gridSize.x + "," + doorData.roomList[i].transform.position.y / GridSettings.gridSize.y + "]");
                    StageTrace.Trace("StageGenerator, CreateDoors : door EST");
                    Instantiate(doorPrefabs[1], doorData.roomList[i].transform.position, doorPrefabs[1].transform.rotation, doorData.roomList[i].transform);
                    break;
                case Direction.South:
                    StageTrace.Trace("StageGenerator, CreateDoors : roomList[" + i + "] = [" + doorData.roomList[i].transform.position.x / GridSettings.gridSize.x + "," + doorData.roomList[i].transform.position.y / GridSettings.gridSize.y + "]");
                    StageTrace.Trace("StageGenerator, CreateDoors : door SOUTH");
                    Instantiate(doorPrefabs[2], doorData.roomList[i].transform.position, doorPrefabs[2].transform.rotation, doorData.roomList[i].transform);
                    break;
                case Direction.West:
                    StageTrace.Trace("StageGenerator, CreateDoors : roomList[" + i + "] = [" + doorData.roomList[i].transform.position.x / GridSettings.gridSize.x + "," + doorData.roomList[i].transform.position.y / GridSettings.gridSize.y + "]");
                    StageTrace.Trace("StageGenerator, CreateDoors : door WEST");
                    Instantiate(doorPrefabs[3], doorData.roomList[i].transform.position, doorPrefabs[3].transform.rotation, doorData.roomList[i].transform);
                    break;
                case Direction.None:
                    break;
                default:
                    break;
            }
        }
    }

    private void CreatePath(DoorNeedData doorData, Room startRoom, Room endRoom, int[,] startPos, int[,] endPos)
    {
        var currentPos = startPos;
        var currentRoom = startRoom;
        while (!(currentPos[0, 0] == endPos[0, 0] && currentPos[0, 1] == endPos[0, 1]))
        {
            if (currentPos[0, 0] < endPos[0, 0])
            {
                doorData.AddNeededDoor(currentRoom, GridSettings.gridSize.x * 0.5f * Vector3.right, Direction.Est);

                if(!currentRoom.DoorsDirections.Contains(Direction.Est)) currentRoom.DoorsDirections.Add(Direction.Est);
                if(!stage.CheckIsPlaceFree(new int[,] { { currentPos[0, 0] + 1, currentPos[0, 1] } }))
                    currentRoom = stage.GetRoomByPosition(new int[,] { { currentPos[0, 0] + 1, currentPos[0, 1] } });
                else if (!(currentPos[0, 0] + 1 == endPos[0, 0] && currentPos[0, 1] == endPos[0, 1]))
                    currentRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count - 1)], (currentPos[0, 0] + 1f) * GridSettings.gridSize.x * Vector3.right + (currentPos[0, 1]) * GridSettings.gridSize.y * Vector3.forward, roomPrefabs[0].transform.rotation, stage.transform).GetComponent<Room>();
                else currentRoom = endRoom;
                currentRoom.DoorsDirections.Add(Direction.West);
                currentPos[0, 0] += 1;
            }
            else if (currentPos[0, 0] > endPos[0, 0])
            {
                doorData.AddNeededDoor(currentRoom, -GridSettings.gridSize.x * 0.5f * Vector3.right, Direction.West);

                if (!currentRoom.DoorsDirections.Contains(Direction.West)) currentRoom.DoorsDirections.Add(Direction.West);
                if (!stage.CheckIsPlaceFree(new int[,] { { currentPos[0, 0] - 1, currentPos[0, 1] } }))
                    currentRoom = stage.GetRoomByPosition(new int[,] { { currentPos[0, 0] - 1, currentPos[0, 1] } });
                else if (!(currentPos[0, 0] - 1 == endPos[0, 0] && currentPos[0, 1] == endPos[0, 1]))
                    currentRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count - 1)], (currentPos[0, 0] - 1f) * GridSettings.gridSize.x * Vector3.right + (currentPos[0, 1]) * GridSettings.gridSize.y * Vector3.forward, roomPrefabs[0].transform.rotation, stage.transform).GetComponent<Room>();
                else currentRoom = endRoom;
                currentRoom.DoorsDirections.Add(Direction.Est);
                currentPos[0, 0] -= 1;
            }

            else if (currentPos[0, 1] < endPos[0, 1])
            {
                doorData.AddNeededDoor(currentRoom, GridSettings.gridSize.y * 0.5f * Vector3.right, Direction.North);

                if (!currentRoom.DoorsDirections.Contains(Direction.North)) currentRoom.DoorsDirections.Add(Direction.North);
                if (!stage.CheckIsPlaceFree(new int[,] { { currentPos[0, 0], currentPos[0, 1] + 1} }))
                    currentRoom = stage.GetRoomByPosition(new int[,] { { currentPos[0, 0], currentPos[0, 1] + 1} });
                else if (!(currentPos[0, 0] == endPos[0, 0] && currentPos[0, 1] + 1 == endPos[0, 1]))
                    currentRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count - 1)], (currentPos[0, 0]) * GridSettings.gridSize.x * Vector3.right + (currentPos[0, 1] + 1) * GridSettings.gridSize.y * Vector3.forward, roomPrefabs[0].transform.rotation, stage.transform).GetComponent<Room>();
                else currentRoom = endRoom;
                currentRoom.DoorsDirections.Add(Direction.South);
                currentPos[0, 1] += 1;
            }

            else if (currentPos[0, 1] > endPos[0, 1])
            {
                doorData.AddNeededDoor(currentRoom, -GridSettings.gridSize.y * 0.5f * Vector3.right, Direction.South);

                if (!currentRoom.DoorsDirections.Contains(Direction.South)) currentRoom.DoorsDirections.Add(Direction.South);
                if (!stage.CheckIsPlaceFree(new int[,] { { currentPos[0, 0], currentPos[0, 1] - 1 } }))
                    currentRoom = stage.GetRoomByPosition(new int[,] { { currentPos[0, 0], currentPos[0, 1] - 1 } });
                else if (!(currentPos[0, 0] == endPos[0, 0] && currentPos[0, 1] - 1 == endPos[0, 1]))
                    currentRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count - 1)], (currentPos[0, 0]) * GridSettings.gridSize.x * Vector3.right + (currentPos[0, 1] - 1) * GridSettings.gridSize.y * Vector3.forward, roomPrefabs[0].transform.rotation, stage.transform).GetComponent<Room>();
                else currentRoom = endRoom;
                currentRoom.DoorsDirections.Add(Direction.North);
                currentPos[0, 1] -= 1;
            }
            //else currentPos = endPos;
            Debug.Log("StageGenerator, GenerateStage : current pos = [" + (currentPos[0, 0]) + "," + (currentPos[0, 1]) + "]");
            StageTrace.Trace("StageGenerator, CreatePath : current pos = [" + (currentPos[0, 0]) + "," + (currentPos[0, 1]) + "]");

            stage.AddRoom(currentRoom, currentPos);
        }
    }

    private GameObject FindSuitableRoom(List<Direction> neededDirection)
    {

        for (int i = 0; i < roomPrefabs.Count; i++)
        {
            var room = roomPrefabs[i].GetComponent<Room>();
            if (room.DoorsDirections.Count == neededDirection.Count)
            {
                var checkedDir = 0;
                for (int j = 0; j < neededDirection.Count; j++)
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
        foreach (Direction dir in Roomdirections)
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
        var newPos = new int[1, 2];
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
