using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

abstract public class Hitable : MonoBehaviour
{
    public UnityEvent dieEvent;
    [SerializeField] protected bool moving = false;
    [SerializeField] protected CombatData combatData;
    [SerializeField] protected List<Status> currentStatus = new List<Status>();

    public bool Moving { get => moving; set => moving = value; }
    public CombatData CombatData { get => combatData; set => combatData = value; }

    abstract public void Attack(Hitable victim);

    virtual protected void Die()
    {
        dieEvent.Invoke();
        Destroy(gameObject, 0.1f);
    }
    virtual public void GetHit(float damage)
    {
        if (combatData.Health == 0f) return;
        var damageWithArmor = Mathf.Min(damage - combatData.PhysicArmor);
        combatData.Health = Mathf.Max(combatData.Health - damageWithArmor, 0f);

        if (combatData.Health == 0f)
            Die();
    }

    virtual public void GetHit(AttackData attackData)
    {
        if (combatData.Health == 0f) return;
        var damageWithArmor = Mathf.Min(attackData.damage - combatData.PhysicArmor);
        combatData.Health = Mathf.Max(combatData.Health - damageWithArmor, 0f);

        if (combatData.Health == 0f)
            Die();

        for(int i = 0; i < attackData.statusList.Count; i++)
        {
            if(!ContainStatus(attackData.statusList[i]))
            {
                var newStatus = new Status(attackData.statusList[i]);

                currentStatus.Add(newStatus);
                newStatus.onStatusEnd.AddListener(delegate () {
                    currentStatus.Remove(newStatus);
                });
            }
        }
    }
    bool ContainStatus(Status status)
    {
        foreach(Status state in currentStatus)
            if (state.StatusType == status.StatusType) return true;
        
        return false;
    }
    Status FindStatusByType(Status.Type statusType)
    {
        foreach (Status state in currentStatus)
            if (state.StatusType == statusType) return state;

        return null;
    }
    private void LateUpdate()
    {
        ApplyStatus();
    }

    public virtual void StopMotion(bool isMoving) { }
         
    virtual public void AddBonus(Bonus bonus)
    {
        combatData.AddBonus(bonus);
    }

    void ApplyStatus()
    {
        for(int i = 0; i < currentStatus.Count; i++)
        {
            currentStatus[i].ApplyStatus(this);
        }
    }
}
