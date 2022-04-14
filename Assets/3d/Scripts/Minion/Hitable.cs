using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

abstract public class Hitable : MonoBehaviour
{
    public UnityEvent dieEvent;
    [SerializeField] protected CombatData combatData;
    abstract public void Attack(Hitable victim);

    virtual public void GetHit(float damage)
    {
        var damageWithArmor = Mathf.Min(damage - combatData.PhysicArmor);
        combatData.Health = Mathf.Max(combatData.Health - damageWithArmor, 0f);

        if (combatData.Health == 0f)
            Die();
    }

    virtual protected void Die()
    {
        dieEvent.Invoke();
        Destroy(gameObject, 0.1f);
    }
}
