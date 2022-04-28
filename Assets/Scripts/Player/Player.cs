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
    [SerializeField] MapUI mapUi;

    [SerializeField] LayerMask minionMask;
    //here
    [SerializeField] LayerMask doorLayer;
    [SerializeField] LayerMask bonusLayer;

    [SerializeField] HealthBar healthUI;
    [SerializeField] ParticleSystem actionParticle;
    [SerializeField] List<GameObject> hidableGo;

    override public void AddBonus(Bonus bonus)
    {
        combat.AddBonus(bonus);
    }

    // Start is called before the first frame update
    private void Awake()
    {
       if (combatData != null) combatData = Instantiate(combatData, transform);
    }
    private void Update()
    {
        mapUi.SetPlayerPosition(transform.position.x / GridSettings.gridSize.x, transform.position.z / GridSettings.gridSize.y);
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
        Debug.Log("Try action -- Door");
        var cols = Physics.OverlapCapsule(transform.position, transform.position + 2f * transform.transform.forward, 0.5f ,doorLayer);
        if (cols.Length > 0)
        {
            actionParticle.Play();
            Destroy(cols[0].gameObject);
        }
        Debug.Log("Try action -- Bonus");
        cols = Physics.OverlapCapsule(transform.position, transform.position + 2f * transform.transform.forward, 0.5f, bonusLayer);
        if (cols.Length > 0)
        {
            var bonus = cols[0].GetComponent<BonusFactory>();
            if(!bonus.Open)
                bonus.OnInteract(this);
            else
            {
                bonus.OnInteract(this);
                Destroy(cols[0].gameObject);
            }
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

    internal void ShowMap(bool isPressed)
    {
        mapUi.SetWholeScreenMap(isPressed);
        foreach(GameObject go in hidableGo)
        {
            go.SetActive(isPressed);
        }
    }
    public override void GetHit(float damage)
    {
        base.GetHit(damage);
        healthUI.SetHealth(combatData.Health);
    }

    public override void GetHit(AttackData attackData)
    {
        base.GetHit(attackData);
        healthUI.SetHealth(combatData.Health);
    }
    protected override void Die()
    {
        base.Die();
        Debug.Log("Player Die");
    }
    public override void StopMotion(bool isMoving)
    {
        motor.StopMotion(isMoving);
    }
}


