using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackModifier : MonoBehaviour
{
    Status status;

    internal void ApplyModifier(AttackData data)
    {
        if (status != null) data.AddStatus(status);
    }
}
