using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

abstract public class CombatData : ScriptableObject
{
    [Header("Stats")]
    [SerializeField] protected float damage;
    [SerializeField] protected float speed;
    [SerializeField] protected float physicArmor;
    [SerializeField] protected float hitRadius;
    [SerializeField] protected float hitRange;
    [SerializeField] protected float coolDown;
    [SerializeField] protected float nextHit = 0f;
    [SerializeField] protected float maxHealth = 10f;
    [SerializeField] protected float health;
    [Header("Component")]
    [SerializeField] public ParticleSystem attackParticles;
    [SerializeField] List<AttackModifier> modifiers = new List<AttackModifier>();

    public float HitRange { get => hitRange;}
    public float MAX_HEALTH { get => maxHealth; set => maxHealth = value; }
    public float Health { get => health; set => health = value; }
    public float Speed { get => speed; set => speed = value; }
    public float PhysicArmor { get => physicArmor; set => physicArmor = value; }

    private void Awake()
    {
        health = MAX_HEALTH;
    }

    AttackData SetAttackData()
    {
        var data = new AttackData(damage, modifiers);
        return data;
    }


    virtual public void Attack(Transform owner, Transform hitPoint, LayerMask enemyLayer, LayerMask friendLayer)
    {
        if (nextHit > Time.time) return;
        nextHit = Time.time + coolDown;
    }
    virtual public void HitTarget(Hitable victim)
    {
        victim.GetHit(SetAttackData());
    }

}


public class AttackData
{
    public float damage;
    public List<Status> statusList = new List<Status>();

    public AttackData(float damage, List<AttackModifier> modifiers)
    {
        this.damage = damage;
        foreach (AttackModifier mod in modifiers)
        {
            mod.ApplyModifier(this);
        }
    }

    internal void AddStatus(Status status)
    {
        statusList.Add(status);
    }
}
