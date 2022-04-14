using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class Minion : Hitable
{
    public enum Type{
        Warrior,
        Tank,
        Range,
        Support
    }

    //[SerializeField] public float nextHit = 0f;
    [Header("Status")]
    [SerializeField] private bool fighting = false;
    [SerializeField] private bool moving = false;
    [Header("Minion characteristic")]
    [SerializeField] Type type;
    [Header("Component")]
    [SerializeField] public Transform hitPoint;
    [SerializeField] public LayerMask enemyLayer;
    [SerializeField] public LayerMask friendLayer;
    [SerializeField] Player owner;
    NavMeshAgent agent;

    [HideInInspector]

    public Player Owner { get; set; }
    public bool Fighting { get => fighting; set => fighting = value; }
    public bool Moving { get => moving; set => moving = value; }
    public Type MinionType { get => type; set => type = value; }

    private void Awake()
    {
        if (combatData != null) combatData = Instantiate(combatData);

        agent = GetComponent<NavMeshAgent>();
        //agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = combatData.Speed;
    }

    private void Update()
    {
        if (!Moving) return;
        var cols = Physics.OverlapSphere(transform.position, combatData.HitRange, enemyLayer);

        if (cols.Length > 0)
        {
            Fighting = true;

            var offset = owner ? (owner.transform.position - cols[0].transform.position).normalized * combatData.HitRange : Vector3.zero;
            SetPosition(cols[0].transform.position + offset);
            if (combatData.GetType() == typeof(RangeAttack))
            {
                var rangedAttack = (RangeAttack)combatData;
                var opponentVelocity = cols[0].GetComponent<Minion>() ? cols[0].GetComponent<NavMeshAgent>().velocity : (Vector3)cols[0].GetComponent<Rigidbody>().velocity;
                var opponentPosition = cols[0].transform.position;
                var opponentLastPosition = (cols[0].transform.position + opponentVelocity.normalized * opponentVelocity.magnitude * combatData.HitRange * (1f / rangedAttack.ProjectileSpeed));
                //var opponentFuturePosition = FindNearestPointOnLine(cols[0].transform.position, opponentVelocity, hitPoint.position);
                var opponentFuturePosition = opponentPosition;

                var oppTravelTime = (opponentPosition - (Vector3)opponentFuturePosition).magnitude / opponentVelocity.magnitude;
                var bulletTravelTime = (hitPoint.position - (Vector3)opponentFuturePosition).magnitude / rangedAttack.ProjectileSpeed;

                var it = 0;
                var delta = 50f;
                while (oppTravelTime < bulletTravelTime && opponentFuturePosition != opponentLastPosition && it < 80)
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
                var rot = ((Vector3)opponentFuturePosition - transform.position).x * Vector3.right + ((Vector3)opponentFuturePosition - transform.position).z * Vector3.forward;
                SetRotation(rot.normalized);

            }
            else
            {
                var rot = (cols[0].transform.position - transform.position).x * Vector3.right + (cols[0].transform.position - transform.position).z * Vector3.forward;
                SetRotation(rot.normalized);
            }

            Attack(null);
        }
        else if (cols.Length == 0)
        {
            Fighting = false;
            var player = FindObjectOfType<Player>();

            if (owner == null && FindObjectOfType<Player>() != null)
            {
                SetPosition(player.transform.position);
                var rot = (player.transform.position - transform.position).x * Vector3.right + (player.transform.position - transform.position).z * Vector3.forward;
                SetRotation(rot.normalized);
            }
        }

    }

    public void SetPosition(Vector3 position)
    {
        if (agent && agent.isOnNavMesh) agent.SetDestination(position);
        var color = Fighting ? Color.red : Color.green;
        Debug.DrawLine(transform.position, position, color);
    }

    public void SetRotation(Vector3 direction)
    {
        var angle = Vector3.SignedAngle(transform.forward, direction, transform.up);
        transform.Rotate(transform.up * angle);
    }
    public override void Attack(Hitable victim)
    {
        combatData.Attack(transform, hitPoint, enemyLayer, friendLayer);
    }
    public Vector2 FindNearestPointOnLine(Vector2 origin, Vector2 direction, Vector2 point)
    {
        direction.Normalize();
        Vector2 lhs = point - origin;

        float dotP = Vector2.Dot(lhs, direction);
        return origin + direction * dotP;
    }
    protected override void Die()
    {
        /*if (owner == null)
        {
            var rnd = UnityEngine.Random.Range(0f, 100f);
            if (rnd > 80f)
            {
                owner = FindObjectOfType<Player>();
                var army = FindObjectOfType<Army>();
                army.AddMinion(this);
                enemyLayer = gameObject.layer;
                
                SetLayerToChildrens(transform, CombatLayer.GetMinionLayer());

                transform.parent = GameObject.Find("Army").transform;
                friendLayer = 1 << CombatLayer.GetPlayerLayer() | 1 << CombatLayer.GetMinionLayer();
                enemyLayer = 1 << CombatLayer.GetEnnemyLayer();
                if (fighting) fighting = false;
                combatData.Health = combatData.MAX_HEALTH;
            }
        }
        else*/
            base.Die();

    }
    void SetLayerToChildrens(Transform transform, LayerMask layer)
    {
        transform.gameObject.layer = layer;
        for (int i = 0; i < transform.childCount; i++)
        {
            SetLayerToChildrens(transform.GetChild(i), layer);
        }
    }
}
