using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Map.Stage2d
{
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
        [SerializeField] bool _navMeshBuildOnce = false;
        [SerializeField] Vector3[] doorsPostition;
        [SerializeField] List<GameObject> roomFloorPrefabs = new List<GameObject>();
        [SerializeField] List<GameObject> doorPrefabs = new List<GameObject>();
        [SerializeField] List<GameObject> wallPrefabs = new List<GameObject>();

        public Direction[] DoorsDirections { get => doorsDirections; set => doorsDirections = value; }
        public Vector3[] DoorsPostition { get => doorsPostition; set => doorsPostition = value; }

        private void Awake()
        {
            GenerateRoom();
        }

        public void GenerateRoom()
        {
            Instantiate(roomFloorPrefabs[0], transform.position, doorPrefabs[0].transform.rotation, transform);

            if (IsContainingDoor(Direction.North))
            {
                Instantiate(doorPrefabs[0], transform.position, doorPrefabs[0].transform.rotation, transform);
                //GridSettings.gridSize
            }
            else
                Instantiate(wallPrefabs[0], transform.position, doorPrefabs[0].transform.rotation, transform);

            if (IsContainingDoor(Direction.Est))
                Instantiate(doorPrefabs[1], transform.position, doorPrefabs[0].transform.rotation, transform);
            else
                Instantiate(wallPrefabs[1], transform.position, doorPrefabs[0].transform.rotation, transform);

            if (IsContainingDoor(Direction.South))
                Instantiate(doorPrefabs[2], transform.position, doorPrefabs[0].transform.rotation, transform);
            else
                Instantiate(wallPrefabs[2], transform.position, doorPrefabs[0].transform.rotation, transform);

            if (IsContainingDoor(Direction.West))
                Instantiate(doorPrefabs[3], transform.position, doorPrefabs[0].transform.rotation, transform);
            else
                Instantiate(wallPrefabs[3], transform.position, doorPrefabs[0].transform.rotation, transform);

            CalculateNavMesh();
        }

        bool IsContainingDoor(Direction dir)
        {
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
            var surface2d = GetComponent<NavMeshSurface2d>();
            //surface2d.UpdateNavMesh(surface2d.navMeshData);


            // var sources = surface2d.CollectSources();
           var sources = new List<NavMeshBuildSource>();
            var boundsCenter = Vector3.right * transform.position.x + Vector3.up * transform.position.z + Vector3.forward * transform.position.y;
            var sourcesBounds = new Bounds(Vector3.zero, new Vector3(14f, 1f, 12f));

            Debug.Log("Room, CalculateNavMesh : sourcesBounds = " + sourcesBounds);

            // Use unscaled bounds - this differs in behaviour from e.g. collider components.
            // But is similar to reflection probe - and since navmesh data has no scaling support - it is the right choice here.
            //var sourcesBounds = new Bounds(surface2d.center, Door.Abs(surface2d.size));
            //if (surface2d.collectObjects != CollectObjects2d.Volume) sourcesBounds = surface2d.CalculateWorldBounds(sources);
            if (_navMeshBuildOnce || true)
            {
                //NavMeshBuilder.UpdateNavMeshDataAsync(surface2d.navMeshData, surface2d.GetBuildSettings(), sources, sourcesBounds);

                var data = NavMeshBuilder.BuildNavMeshData(surface2d.GetBuildSettings(),
                        sources, sourcesBounds, transform.position, transform.rotation);


                if (data != null)
                {
                    data.name = gameObject.name;
                    surface2d.RemoveData();
                    surface2d.navMeshData = data;
                    if (isActiveAndEnabled)
                        surface2d.AddData();
                }
            }
            else
            {
                //surface2d.BuildNavMesh();
                var data = NavMeshBuilder.BuildNavMeshData(surface2d.GetBuildSettings(),
                        sources, sourcesBounds, transform.position, transform.rotation);


                if (data != null)
                {
                    data.name = gameObject.name;
                    surface2d.RemoveData();
                    surface2d.navMeshData = data;
                    if (isActiveAndEnabled)
                        surface2d.AddData();
                }
                _navMeshBuildOnce = true;
            }
        }
    }
}
