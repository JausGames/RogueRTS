using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

// Tagging component for use with the LocalNavMeshBuilder
// Supports mesh-filter and terrain - can be extended to physics and/or primitives
[DefaultExecutionOrder(-200)]
public class NavMeshSourceTag : MonoBehaviour
{
    // Global containers for all active mesh/terrain tags
    public static List<Tilemap> m_tileMaps = new List<Tilemap>();
    public static List<Vector3Int> m_tilePositions = new List<Vector3Int>();

    void OnEnable()
    {
        var m = GetComponentsInChildren<Tilemap>();
        if (m.Length > 0)
        {
            for(int i = 0; i < m.Length; i++)
            {
                m_tileMaps.Add(m[i]);
                m_tilePositions.Add(m[i].WorldToCell(m[i].transform.position));
                /*var x = Mathf.CeilToInt(m[i].transform.position.x);
                var y = Mathf.CeilToInt(m[i].transform.position.y);
                var z = Mathf.CeilToInt(m[i].transform.position.z);
                m_tilePositions.Add(x * Vector3Int.right + y * Vector3Int.up + z * Vector3Int.forward);*/
            }
        }
    }

    void OnDisable()
    {
        var m = GetComponentsInChildren<Tilemap>();
        if (m.Length > 0)
        {
            for (int i = 0; i < m.Length; i++)
            {
                m_tileMaps.Remove(m[i]);
            }
        }
    }

    // Collect all the navmesh build sources for enabled objects tagged by this component
    public static void Collect(ref List<NavMeshBuildSource> sources)
    {
        sources.Clear();

        for(int i = 0; i < m_tileMaps.Count; i++)
        {
            var src = new NavMeshBuildSource();
            src.transform = Matrix4x4.Translate(m_tileMaps[i].GetCellCenterWorld(m_tilePositions[i]));
            src.shape = NavMeshBuildSourceShape.Box;
            src.size = Vector3.one;
            sources.Add(src);
        }

/*
        for (var i = 0; i < m_Meshes.Count; ++i)
        {
            var mf = m_Meshes[i];
            if (mf == null) continue;

            var m = mf.sharedMesh;
            if (m == null) continue;

            var s = new NavMeshBuildSource();
            s.shape = NavMeshBuildSourceShape.Mesh;
            s.sourceObject = m;
            s.transform = mf.transform.localToWorldMatrix;
            s.area = 0;
            sources.Add(s);
        }

        for (var i = 0; i < m_Terrains.Count; ++i)
        {
            var t = m_Terrains[i];
            if (t == null) continue;

            var s = new NavMeshBuildSource();
            s.shape = NavMeshBuildSourceShape.Terrain;
            s.sourceObject = t.terrainData;
            // Terrain system only supports translation - so we pass translation only to back-end
            s.transform = Matrix4x4.TRS(t.transform.position, Quaternion.identity, Vector3.one);
            s.area = 0;
            sources.Add(s);
        }*/
    }
}