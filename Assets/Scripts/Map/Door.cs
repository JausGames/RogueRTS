using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    [SerializeField] LayerMask mask;

    

    private void OnDestroy()
    {
        //var surface2d = FindObjectOfType<NavMeshSurface2d>();
        //surface2d.UpdateNavMesh(surface2d.navMeshData);

        /*//Bounds bounds = new Bounds();
        Bounds bounds = surface2d.navMeshData.sourceBounds;
        //bounds.center = new Vector3(transform.position.x - 20, -10, transform.position.z - 20);
        //bounds.center = new Vector3(20, 0, -38);
        //bounds.extents = new Vector3(75, 0, 60);
        //bounds.max = new Vector3(bounds.center.x + 2, bounds.center.y + 10, bounds.center.z + 2);
        //bounds.min = new Vector3(bounds.center.x - 2, bounds.center.y - 10, bounds.center.z - 2);
        //bounds.size = new Vector3(150, 0, 120);

        var sources = new List<NavMeshBuildSource>(); 

        NavMeshBuilder.CollectSources(
            bounds,
            mask,
            NavMeshCollectGeometry.PhysicsColliders,
            0,
            new List<NavMeshBuildMarkup>(),
            sources);

        NavMeshBuilder.UpdateNavMeshDataAsync(surface2d.navMeshData, surface2d.GetBuildSettings(), sources, bounds);*/
    }
}
