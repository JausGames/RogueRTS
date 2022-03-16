using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;

    [SerializeField] Vector2 move = Vector2.zero;
    [SerializeField] Vector2 look = Vector2.zero;

    private void Awake()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        agent.SetDestination(transform.position + (Vector3)move);

        var angle = Vector3.SignedAngle(transform.up, look, transform.forward);
        transform.Rotate(transform.forward * angle);
    }

    public void SetMove(Vector2 move)
    {
        this.move = move;
    }

    public void SetLook(Vector2 look)
    {
        this.look = look;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)move);
    }
}
