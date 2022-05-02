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
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        Null
    }

    //[SerializeField] public float nextHit = 0f;
    [Header("Status")]
    [SerializeField] private bool fighting = false;
    [Header("Minion characteristic")]
    [SerializeField] Type type;
    [Header("Component")]
    [SerializeField] public Transform hitPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask friendLayer;
    [SerializeField] public BeetlingAnimatorController animator;
    [SerializeField] Player owner;
    [SerializeField] Vector3 randomDestination;
    NavMeshAgent agent;

    [HideInInspector]

    public Player Owner { get => owner; set => owner = value; }
    public bool Fighting { get => fighting; set => fighting = value; }
    public Type MinionType { get => type; set => type = value; }
    public LayerMask EnemyLayer { get => enemyLayer; set => enemyLayer = value; }
    public LayerMask FriendLayer { get => friendLayer; set => friendLayer = value; }

    private void Start()
    {
        if (combatData != null) combatData = Instantiate(combatData);

        agent = GetComponent<NavMeshAgent>();
        //agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = combatData.Speed;

        var renderer = transform.Find("visual").Find("sprite").GetComponent<SpriteRenderer>();
        var mat = new Material(renderer.material);
        BeetlingColor.TypeToColor.TryGetValue(type, out Color color);
        mat.SetColor("_Color", color);
        var leafColor = owner != null ? BeetlingColor.LeafColors[0] : BeetlingColor.LeafColors[1];
        mat.SetColor("_LeafColor", leafColor);
        renderer.material = mat;
        if (owner == null) mat.SetInt("_isEnemy", 1);
        randomDestination = transform.position;
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

            if (owner == null && player != null)
            {
                cols = Physics.OverlapSphere(transform.position, 3f, enemyLayer);

                if (cols.Length > 0)
                {
                    SetPosition(cols[0].transform.position);
                    var rot = (cols[0].transform.position - transform.position).x * Vector3.right + (cols[0].transform.position - transform.position).z * Vector3.forward;
                    SetRotation(rot.normalized);
                }
                else
                {
                    Debug.DrawLine(transform.position, randomDestination);
                    if ((transform.position - randomDestination).sqrMagnitude < 0.2f)
                    {
                        var rndX = UnityEngine.Random.Range(-1f, 1f);
                        var rndY = UnityEngine.Random.Range(-1f, 1f);
                        var distance = 5f;
                        randomDestination = (rndX * Vector3.right + rndY * Vector3.forward).normalized * distance + transform.position;

                        
                        SetPosition(randomDestination);
                        var rot = (randomDestination - transform.position).x * Vector3.right + (randomDestination - transform.position).z * Vector3.forward;
                        SetRotation(rot.normalized);
                    }
                }
            }
        }
        UpdateAnimator();
    }
    public void SetLayers(LayerMask friend, LayerMask enemy)
    {
        enemyLayer = enemy;
        friendLayer = friend;
    }
    private void UpdateAnimator()
    {
        if(Mathf.Abs(agent.velocity.x) > Mathf.Abs(agent.velocity.z) && Mathf.Abs(agent.velocity.x) > 0.2f)
        {
            var Direction = agent.velocity.x < 0 ? Minion.Direction.Left : Minion.Direction.Right;
            animator.SetController(agent.velocity.magnitude, Direction);
        }
        else if(Mathf.Abs(agent.velocity.x) < Mathf.Abs(agent.velocity.z) && Mathf.Abs(agent.velocity.z) > 0.2f)
        {
            var Direction = agent.velocity.z < 0 ? Minion.Direction.Down : Minion.Direction.Up;
            animator.SetController(agent.velocity.magnitude, Direction);
        }
        else
            animator.SetController(0f, Minion.Direction.Null);
    }

    public void SetPosition(Vector3 position)
    {
        if (!agent || !agent.isOnNavMesh) return;
        var canPass = false;
        for(float i = 0f; i <= 1f; i += 0.1f)
        {
            NavMeshPath navMeshPath = new NavMeshPath();
            //create path and check if it can be done
            // and check if navMeshAgent can reach its target
            if (agent.CalculatePath(position * (1f - i) + transform.position * i, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetPath(navMeshPath);
                randomDestination = position * (1f - i) + transform.position * i;
                var color = Fighting ? Color.red : Color.green;
                canPass = true;
                Debug.DrawLine(transform.position, position, color);
                break;
            }
        }

        if(!canPass) randomDestination = transform.position;
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
        if (!owner)
        {
            var dropRateRND = UnityEngine.Random.Range(0f, 1f);

            if (dropRateRND < CombatData.DropRate)
            {
                var GO = Instantiate(Resources.Load<GameObject>("Prefabs/Pickables/PickableContainer"), transform.position, Quaternion.identity);
                var pickables = Resources.LoadAll<Pickable>("Prefabs/Pickables/Presets");
                var rates = new List<float>();
                var totalRate = 0f;
                for (int i = 0; i < pickables.Length; i++)
                {
                    rates.Add(pickables[i].Rate);
                    totalRate += pickables[i].Rate;
                }
                var rnd = UnityEngine.Random.Range(0f, totalRate);
                var result = 0f;

                for (int i = 0; i < rates.Count; i++)
                {
                    var sumRate = 0f;
                    for (int j = 0; j <= i; j++)
                    {
                        sumRate += rates[j];
                    }
                    if (rnd <= sumRate)
                    {
                        GO.GetComponent<PickableContainer>().Item = Instantiate(pickables[i], transform.position, GO.transform.rotation, GO.transform);
                        break;
                    }
                }
                GO.GetComponentInChildren<SpriteRenderer>().sprite = GO.GetComponentInChildren<PickableContainer>().Item.Sprite;

            }
        }
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
    public override void StopMotion(bool isMoving)
    {
        if(isMoving)
        {
            moving = true;
        }
        else
        {
            moving = false;
            agent.SetDestination(transform.position);
            agent.velocity = Vector3.zero;
        }
    }
}
