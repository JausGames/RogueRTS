using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AttackData : ScriptableObject
{
    [Header("Stats")]
    [SerializeField] protected float damage;
    [SerializeField] protected float hitRadius;
    [SerializeField] protected float hitRange;
    [SerializeField] protected float coolDown;
    [SerializeField] protected float nextHit = 0f;
    [Header("Component")]
    [SerializeField] public ParticleSystem attackParticles;

    public float HitRange { get => hitRange;}

    virtual public void Attack(Transform owner, Transform hitPoint, LayerMask enemyLayer)
    {
        if (nextHit > Time.time) return;
        nextHit = Time.time + coolDown;
    }

}
