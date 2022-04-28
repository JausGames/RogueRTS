using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] CombatData combatData;
    [SerializeField] CombatDataUi ui;
    [SerializeField] public Transform hitPoint;
    [SerializeField] public LayerMask ennemyLayer;
    [SerializeField] public LayerMask friendLayer;

    public CombatData CombatData { get => combatData; set { combatData = value; ui.SetUpStat(combatData); } }


    public void Attack()
    {
        combatData.Attack(transform, hitPoint, ennemyLayer, friendLayer);
    }

    internal void AddBonus(Bonus bonus)
    {
        combatData.AddBonus(bonus);
        ui.UpdateStatUi(combatData, bonus);
    }
}
