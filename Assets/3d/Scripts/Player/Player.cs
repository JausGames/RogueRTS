using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : Hitable
{
    Army army;
    PlayerCombat combat;
    PlayerController motor;

    [SerializeField] LayerMask minionMask;
    //here
    [SerializeField] LayerMask doorLayer;

    [SerializeField] HealthBar healthUI;

    // Start is called before the first frame update
    private void Awake()
    {
        //if (combatData != null) combatData = Instantiate(combatData, transform);
    }
    void Start()
    {
        combat = GetComponent<PlayerCombat>();
        combat.CombatData = combatData;
        motor = GetComponent<PlayerController>();
        army = GetComponent<Army>();

        motor.SetSpeed(combatData.Speed);
        motor.updateArmyEvent.AddListener(delegate { army.SetMinionsPosition(transform.position, transform.forward); });

        healthUI.SetMaxHealth(combatData.MAX_HEALTH);
        healthUI.SetHealth(combatData.Health);
    }
    internal void TryAction()
    {
        Debug.Log("Try action");
        var cols = Physics.OverlapCapsule(transform.position, transform.position + 2f * transform.transform.forward, 0.5f ,doorLayer);
        if (cols.Length > 0)
        {
            /*foreach(Collider2D col in cols)
            {
                if (col && col.gameObject) Destroy(col.gameObject);
            }*/
            Destroy(cols[0].gameObject);
        }
    }

    public override void Attack(Hitable victim)
    {
        combat.Attack();
    }

    void AddMinionToArmy(Minion minion)
    {
        minion.Owner = this;
        army.AddMinion(minion);
        army.SetMinionsPosition(transform.position, transform.forward);
    }
    public override void GetHit(float damage)
    {
        base.GetHit(damage);
        healthUI.SetHealth(combatData.Health);
    }
    protected override void Die()
    {
        base.Die();
        Debug.Log("Player Die");
    }
}


