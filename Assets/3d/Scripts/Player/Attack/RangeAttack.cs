using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RangeAtack", menuName = "Combat Objects/Range Attack/Basic Projectile", order = 1)]
public class RangeAttack : AttackData
{
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed;

    public float ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }
    public GameObject Projectile { get => projectile; set => projectile = value; }

    public override void Attack(Transform owner, Transform hitPoint, LayerMask enemyLayer, LayerMask friendLayer)
    {
        //base.Attack(owner, hitPoint, enemyLayer);

        if (nextHit > Time.time) return;
        nextHit = Time.time + coolDown;

        var projectileGo = Instantiate(projectile, hitPoint.transform.position, owner.transform.rotation * projectile.transform.rotation, null);
        var projectileCmp = projectileGo.GetComponent<Projectile>();
        projectileCmp.Damage = damage;
        projectileCmp.Range = hitRange;
        projectileCmp.Radius = hitRadius;
        projectileCmp.Speed = projectileSpeed;
        projectileCmp.EnemyLayer = enemyLayer;
        projectileCmp.WillDestroyLayer = ~friendLayer & ~(1 << projectileGo.layer);
    }
}
