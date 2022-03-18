using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Hitable : MonoBehaviour
{
    protected const float MAX_HEALTH = 10f;
    protected float health = MAX_HEALTH;
    abstract public void Attack(Hitable victim);

    virtual public void GetHit(float damage)
    {
        health = Mathf.Max(health - damage, 0f);

        if (health == 0f)
        {
            Die();
        }
    }

    virtual protected void Die()
    {
        Destroy(gameObject);
    }
}
