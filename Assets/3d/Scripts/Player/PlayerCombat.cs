using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] CombatData combatData;
    [SerializeField] public Transform hitPoint;
    [SerializeField] public LayerMask ennemyLayer;
    [SerializeField] public LayerMask friendLayer;

    public CombatData CombatData { get => combatData; set => combatData = value; }

    //private float nextHit;

    public void Attack()
    {
        combatData.Attack(transform, hitPoint, ennemyLayer, friendLayer);
    }
}
