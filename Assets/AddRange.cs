using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRange : MonoBehaviour
{
    [SerializeField] PlayerCombat combat;

    public void AddRangeEvent()
    {
        combat.CombatData.Radius += .05f;
    }
}
