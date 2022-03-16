using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TwoDLocal
{

    public class Player : MonoBehaviour
    {
        NavMeshAgent agent;
        Army army;
        [SerializeField] LayerMask minionMask;
        // Start is called before the first frame update
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            army = GetComponent<Army>(); 
        }

        // Update is called once per frame
        void Update()
        {
            /*if (Input.GetMouseButtonDown(0))
            {
                var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                agent.SetDestination(mouseWorldPos - Camera.main.transform.position.z * Vector3.forward);
                var dir = (mouseWorldPos.x - transform.position.x) * Vector3.right + (mouseWorldPos.y - transform.position.y) * Vector3.up;
                var angle = Vector3.SignedAngle(transform.up, dir, transform.forward);
                transform.Rotate(transform.forward * angle);
                army.SetMinionsPosition(mouseWorldPos + -Camera.main.transform.position.z * Vector3.forward, dir);
                
            }*/
            var minionColliders = Physics2D.OverlapCircleAll(transform.position, 1f, minionMask);

            if (minionColliders.Length > 0)
            {
                foreach(Collider2D col in minionColliders)
                {
                    var minion = col.GetComponentInParent<Minion>();
                    if (!minion.Owner) AddMinionToArmy(minion);
                }
            }
        }

        void AddMinionToArmy(Minion minion)
        {
            minion.Owner = this;
            army.AddMinion(minion);
            army.SetMinionsPosition(agent.destination, transform.up);
        }
    }

}
