using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Minion : Hitable
{
    [SerializeField] Player owner;

    NavMeshAgent agent;

    [SerializeField] Vector3 position;

    [Header("Stats")]
    [SerializeField] public float damage = 5f;
    [SerializeField] public float hitRadius = 0.15f;
    [SerializeField] public float hitRange = 0.5f;
    [Header("Component")]
    [SerializeField] public Transform hitPoint;
    [SerializeField] public LayerMask ennemyLayer;
    [SerializeField] public ParticleSystem attackParticles;

    public Player Owner { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    private void Update()
    {
        if(owner == null)
        {
            var player = FindObjectOfType<Player>();
            SetPosition(player.transform.position);
            SetRotation((player.transform.position - transform.position).normalized);
            if ((player.transform.position - transform.position).sqrMagnitude < 0.1f) Attack(player);
        }
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
    public override void Attack(Hitable victim)
    {
        Debug.Log("Minion, Attack");
        attackParticles.Play();
        var cols = Physics2D.OverlapAreaAll(hitPoint.position - hitRadius * transform.right, hitPoint.position + hitRadius * transform.right + hitRange * transform.up, ennemyLayer);

        var touchedEnnemy = new List<Hitable>();
        if (cols.Length > 0)
        {
            foreach (Collider2D col in cols)
            {
                Hitable minion = col.gameObject.GetComponent<Hitable>();
                if (!touchedEnnemy.Contains(minion) && minion != null && minion != this)
                {
                    Debug.Log("Minion touched = " + minion);
                    touchedEnnemy.Add(minion);
                }
            }
        }
        var distance = Mathf.Infinity;
        Hitable ennemy = null;
        for (int i = 0; i < touchedEnnemy.Count; i++)
        {
            var checkedDistance = (touchedEnnemy[i].transform.position - hitPoint.position).sqrMagnitude;
            if (checkedDistance < distance) ennemy = touchedEnnemy[i];
        }
        if(ennemy != null) ennemy.GetHit(damage);
    }

}

