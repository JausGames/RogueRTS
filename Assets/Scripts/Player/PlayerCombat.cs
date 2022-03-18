using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] public float damage = 5f;
    [SerializeField] public float hitRadius = 0.15f;
    [SerializeField] public float hitRange = 0.5f;
    [Header("Component")]
    [SerializeField] public Transform hitPoint;
    [SerializeField] public LayerMask ennemyLayer;
    [SerializeField] public ParticleSystem attackParticles;


    public void Attack()
    {
        Debug.Log("OMG ! this dude is attacking");
        attackParticles.Play();
        var cols = Physics2D.OverlapAreaAll(hitPoint.position - hitRadius * transform.right, hitPoint.position + hitRadius * transform.right + hitRange * transform.up, ennemyLayer);

        var touchedEnnemy = new List<Minion>();
        if(cols.Length > 0)
        {
            foreach(Collider2D col in cols)
            {
                Minion minion = col.gameObject.GetComponent<Minion>();
                if (!touchedEnnemy.Contains(minion) && minion != null)
                {
                    Debug.Log("Minion touched = " + minion);
                    touchedEnnemy.Add(minion);
                }
            }
        }
        var distance = Mathf.Infinity;
        var closestEnnemyId = 0;
        for (int i = 0; i < touchedEnnemy.Count; i++)
        {
            var checkedDistance = (touchedEnnemy[i].transform.position - hitPoint.position).sqrMagnitude;
            if (checkedDistance < distance) closestEnnemyId = i;
        }
        touchedEnnemy[closestEnnemyId].GetHit(damage);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hitPoint.position - hitRadius * transform.right, hitRadius / 3f);
        Gizmos.DrawSphere(hitPoint.position + hitRadius * transform.right, hitRadius / 3f);
        Gizmos.DrawSphere(hitPoint.position + hitRadius * transform.right + hitRange * transform.up, hitRadius / 3f);
        Gizmos.DrawSphere(hitPoint.position - hitRadius * transform.right + hitRange * transform.up, hitRadius / 3f);
        Gizmos.DrawLine(hitPoint.position - hitRadius * transform.right, hitPoint.position + hitRadius * transform.right);
        Gizmos.DrawLine(hitPoint.position + hitRadius * transform.right, hitPoint.position + hitRadius * transform.right + hitRange * transform.up);
        Gizmos.DrawLine(hitPoint.position - hitRadius * transform.right + hitRange * transform.up, hitPoint.position + hitRadius * transform.right + hitRange * transform.up);
        Gizmos.DrawLine(hitPoint.position - hitRadius * transform.right, hitPoint.position - hitRadius * transform.right + hitRange * transform.up);
    }
}
