﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Player2d
{
    public class Player : Hitable
    {
        Army army;
        PlayerCombat combat;
        PlayerController motor;

        [SerializeField] LayerMask minionMask;
        //here
        [SerializeField] LayerMask doorLayer;


        private void Awake()
        {
            if (combatData != null) combatData = Instantiate(combatData, transform);
        }

        // Start is called before the first frame update
        void Start()
        {
            combat = GetComponent<PlayerCombat>();
            motor = GetComponent<PlayerController>();
            army = GetComponent<Army>();

            motor.updateArmyEvent.AddListener(delegate { army.SetMinionsPosition(transform.position, transform.up); });
        }
        internal void TryAction()
        {
            Debug.Log("Try action");
            var cols = Physics2D.OverlapAreaAll(transform.position - 0.5f * transform.right, transform.position + 0.5f * transform.transform.right + 2f * transform.transform.up, doorLayer);
            if (cols.Length > 0)
            {
                Destroy(cols[0].gameObject);
            }
        }

        public override void Attack(Hitable victim)
        {
            combat.Attack();
        }
        public override void GetHit(float damage)
        {
            base.GetHit(damage);
        }

        void AddMinionToArmy(Minion minion)
        {
            minion.Owner = this;
            army.AddMinion(minion);
            army.SetMinionsPosition(transform.position, transform.up);
        }
        protected override void Die()
        {
            base.Die();
            Debug.Log("Player Die");
        }
    }
}

