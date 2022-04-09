using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Map.Stage2d
{
    public class Door : MonoBehaviour
    {
        private void OnDestroy()
        {
            /*var room = GetComponentInParent<Room>();
            room.CalculateNavMesh();*/



            var stageGen = FindObjectOfType<StageGenerator>();
            stageGen.UpdateNavMesh();

        }
    }
}
