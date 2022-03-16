using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace TwoDLocal
{

    public class Minion : MonoBehaviour
    {
        Player owner;
        NavMeshAgent agent;

        [SerializeField] Vector3 position;

        public Player Owner { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
            agent.SetDestination(position);
        }

        public void SetRotation(Vector3 direction)
        {
            var angle = Vector3.SignedAngle(transform.up, direction, transform.forward);
            transform.Rotate(transform.forward * angle);
        }
    }
}
