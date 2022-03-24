using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] AttackData attackData;
    [SerializeField] public Transform hitPoint;
    [SerializeField] public LayerMask ennemyLayer;
    //private float nextHit;

    private void Awake()
    {
        if(attackData != null) attackData = Instantiate(attackData);
    }
    public void Attack()
    {
        attackData.Attack(transform, hitPoint, ennemyLayer);
    }
}
