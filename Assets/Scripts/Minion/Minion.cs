using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;


public class Minion : Hitable
{

    //[SerializeField] public float nextHit = 0f;
    [Header("Status")]
    [SerializeField] private bool fighting = false;
    [Header("Component")]
    [SerializeField] AttackData attackData;
    [SerializeField] public Transform hitPoint;
    [SerializeField] public LayerMask enemyLayer;
    [SerializeField] Player owner;
    NavMeshAgent agent;

    [HideInInspector]

    public Player Owner { get; set; }
    public bool Fighting { get => fighting; set => fighting = value; }

    private void Awake()
    {
        if (attackData != null) attackData = Instantiate(attackData);

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        var cols = Physics2D.OverlapCircleAll(transform.position, attackData.HitRange, enemyLayer);

        if (cols.Length > 0)
        {
            Fighting = true;
            Attack(null);

            var offset = owner ? (owner.transform.position - cols[0].transform.position).normalized * attackData.HitRange : Vector3.zero;
            SetPosition(cols[0].transform.position + offset);
            if(attackData.GetType() == typeof(RangeAttack))
            {
                var rangedAttack = (RangeAttack)attackData;
                var opponentVelocity = cols[0].GetComponent<Minion>() ? cols[0].GetComponent<NavMeshAgent>().velocity : (Vector3)cols[0].GetComponent<Rigidbody2D>().velocity;
                var opponentPosition = cols[0].transform.position;
                var opponentLastPosition = (cols[0].transform.position + opponentVelocity.normalized * opponentVelocity.magnitude * attackData.HitRange * (1f / rangedAttack.ProjectileSpeed));
                //var opponentFuturePosition = FindNearestPointOnLine(cols[0].transform.position, opponentVelocity, hitPoint.position);
                var opponentFuturePosition = opponentPosition;

                var oppTravelTime = (opponentPosition - (Vector3)opponentFuturePosition).magnitude / opponentVelocity.magnitude;
                var bulletTravelTime = (hitPoint.position - (Vector3)opponentFuturePosition).magnitude / rangedAttack.ProjectileSpeed;

                var it = 0;
                var delta = 50f;
                while(oppTravelTime < bulletTravelTime && opponentFuturePosition != opponentLastPosition && it < 80)
                {
                    if ((opponentFuturePosition - opponentLastPosition).magnitude < Time.deltaTime) opponentFuturePosition = opponentLastPosition;
                    opponentFuturePosition += (opponentLastPosition - opponentFuturePosition).normalized * Time.deltaTime * delta;
                    oppTravelTime = (opponentPosition - opponentFuturePosition).magnitude / opponentVelocity.magnitude;
                    bulletTravelTime = (hitPoint.position - opponentFuturePosition).magnitude / rangedAttack.ProjectileSpeed;
                    it++;
                }
                Debug.Log("it = " + it);



                Debug.DrawLine(transform.position, opponentFuturePosition, Color.red);
                Debug.DrawLine(transform.position, opponentLastPosition, Color.black);
                Debug.DrawLine(opponentPosition, opponentFuturePosition, Color.cyan);
                //Debug.DrawLine(opponentPosition, opponentLastPosition, Color.blue);
                SetRotation(((Vector3)opponentFuturePosition - transform.position).normalized);

            }
            else  SetRotation((cols[0].transform.position - transform.position).normalized);
        }
        else if (cols.Length == 0)
        {
            Fighting = false;
            var player = FindObjectOfType<Player>();

            if (owner == null && FindObjectOfType<Player>() != null)
            {
                SetPosition(player.transform.position);
                SetRotation((player.transform.position - transform.position).normalized);
            }
        }
    }

    public void SetPosition(Vector3 position)
    {
        if(agent && agent.isOnNavMesh) agent.SetDestination(position);
        var color = Fighting ? Color.red : Color.green;
        Debug.DrawLine(transform.position, position, color);
    }

    public void SetRotation(Vector3 direction)
    {
        var angle = Vector3.SignedAngle(transform.up, direction, transform.forward);
        transform.Rotate(transform.forward * angle);
    }
    public override void Attack(Hitable victim)
    {
        attackData.Attack(transform, hitPoint, enemyLayer);
    }
    public Vector2 FindNearestPointOnLine(Vector2 origin, Vector2 direction, Vector2 point)
    {
        direction.Normalize();
        Vector2 lhs = point - origin;

        float dotP = Vector2.Dot(lhs, direction);
        return origin + direction * dotP;
    }
}

