using UnityEngine;

public class Weapon : WeaponBase
{
    [Header("Weapon Settings")]
    public float fireRate = 0.5f;
    private float nextFireTime;

    private void Start()
    {
        weaponName = "Pistol";
        baseBodyDamage = 30f;
        baseHeadDamage = 100f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, 
                              out RaycastHit hit))
            {
                ProcessHit(hit);
            }
        }
    }

    public override void ProcessHit(RaycastHit hit, bool isHeavyAttack = false)
    {
        bool isHeadshot = hit.collider.CompareTag("Head");
        float damage = isHeadshot ? CurrentHeadDamage : CurrentBodyDamage;
        
        if (hit.collider.GetComponentInParent<Enemy>() is Enemy enemy)
        {
            enemy.TakeDamage(damage, isHeadshot);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * 100f);
    }
}