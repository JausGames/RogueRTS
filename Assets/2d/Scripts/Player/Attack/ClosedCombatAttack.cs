using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gameplay.Player2d
{
    [CreateAssetMenu(fileName = "ClosedCombat", menuName = "Combat2d Objects/Closed Combat/Basic Hit", order = 1)]
    public class ClosedCombatAttack : AttackData
    {
        override public void Attack(Transform owner, Transform hitPoint, LayerMask enemyLayer, LayerMask friendLayer)
        {
            //base.Attack(owner, hitPoint, enemyLayer);


            if (nextHit > Time.time) return;
            nextHit = Time.time + coolDown;

            var partclObj = Instantiate(attackParticles, hitPoint.position, owner.transform.rotation * attackParticles.transform.rotation, owner);
            var partcl = partclObj.GetComponent<ParticleSystem>();
            Destroy(partclObj.gameObject, partclObj.main.duration);

            var cols = Physics2D.OverlapAreaAll(hitPoint.position - hitRadius * owner.transform.right, hitPoint.position + hitRadius * owner.transform.right + hitRange * owner.transform.up, enemyLayer);

            var touchedEnnemy = new List<Hitable>();
            if (cols.Length > 0)
            {
                foreach (Collider2D col in cols)
                {
                    Hitable minion = col.gameObject.GetComponent<Hitable>();
                    if (!touchedEnnemy.Contains(minion) && minion != null)
                    {
                        Debug.Log("Minion touched = " + minion);
                        touchedEnnemy.Add(minion);
                    }
                }

                if (touchedEnnemy.Count != 0)
                {
                    var distance = Mathf.Infinity;
                    var closestEnnemyId = 0;
                    for (int i = 0; i < touchedEnnemy.Count; i++)
                    {
                        var checkedDistance = (touchedEnnemy[i].transform.position - hitPoint.position).sqrMagnitude;
                        if (checkedDistance < distance) closestEnnemyId = i;
                    }
                    touchedEnnemy[closestEnnemyId].GetHit(damage);
                }
            }
        }

    }
}