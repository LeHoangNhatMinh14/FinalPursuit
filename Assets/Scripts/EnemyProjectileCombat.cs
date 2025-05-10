using UnityEngine;

public class EnemyProjectileCombat : EnemyCombat
{
    public GameObject projectilePrefab;
    public Transform firePoint;

    protected override void AttemptDamage()
    {
        PlayAttackEffects();

        if (projectilePrefab && firePoint)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            if (projectile.TryGetComponent<EnemyProjectile>(out var proj))
            {
                proj.damage = attackDamage;
            }
        }
    }
}
