using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gameplay.Player2d
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 5f;
        [SerializeField] float damage = 5f;
        [SerializeField] float radius = 5f;
        [SerializeField] float range = 5f;
        [SerializeField] Vector3 startPosition = Vector3.zero;
        private LayerMask enemyLayer;

        public float Speed { get => speed; set => speed = value; }
        public float Damage { get => damage; set => damage = value; }
        public float Radius { get => radius; set => radius = value; }
        public float Range { get => range; set => range = value; }
        public LayerMask EnemyLayer { get => enemyLayer; set => enemyLayer = value; }

        private void Awake()
        {
            startPosition = transform.position;
        }
        // Start is called before the first frame update
        void Update()
        {
            if ((transform.position - startPosition).magnitude >= range) Destroy(gameObject);

            transform.position += transform.up * speed * Time.deltaTime;

            var cols = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);
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

                var distance = Mathf.Infinity;
                var closestEnnemyId = 0;
                for (int i = 0; i < touchedEnnemy.Count; i++)
                {
                    var checkedDistance = (touchedEnnemy[i].transform.position - transform.position).sqrMagnitude;
                    if (checkedDistance < distance) closestEnnemyId = i;
                }
                touchedEnnemy[closestEnnemyId].GetHit(damage);
                Destroy(gameObject);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}