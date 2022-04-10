using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    private void OnDestroy()
    {
        /*var room = GetComponentInParent<Room>();
        room.CalculateNavMesh();*/



        var stageGen = FindObjectOfType<StageGenerator>();
        Debug.Log("Door broken");
        if(stageGen) stageGen.UpdateNavMesh();

    }
}

