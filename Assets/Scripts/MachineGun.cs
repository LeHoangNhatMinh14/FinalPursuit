using UnityEngine;

public class MachineGun : WeaponBase
{
    [Header("Behavior")]
    public float fireRate = 0.1f;
    public float spreadAngle = 3f;
    public float maxDistance = 100f;
    public int bulletsPerShot = 3;
    
    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public AudioSource gunAudio;
    public AudioClip shootSound;
    public GameObject bulletImpactPrefab;

    private float nextFireTime;
    private Camera playerCamera;

    private void Start()
    {
        weaponName = "MachineGun";
        baseBodyDamage = 15f;
        baseHeadDamage = 45f;
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        muzzleFlash?.Play();
        gunAudio?.PlayOneShot(shootSound);

        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 shotDirection = GetShotDirection();
            if (Physics.Raycast(playerCamera.transform.position, shotDirection, 
                              out RaycastHit hit, maxDistance))
            {
                ProcessHit(hit);
            }
        }
    }

    Vector3 GetShotDirection()
    {
        Vector3 direction = playerCamera.transform.forward;
        direction = Quaternion.AngleAxis(Random.Range(-spreadAngle, spreadAngle), Vector3.up) * direction;
        direction = Quaternion.AngleAxis(Random.Range(-spreadAngle, spreadAngle), Vector3.right) * direction;
        return direction;
    }

    public override void ProcessHit(RaycastHit hit, bool isHeavyAttack = false)
    {
        bool isHeadshot = hit.collider.CompareTag("Head");
        float damage = isHeadshot ? CurrentHeadDamage : CurrentBodyDamage;

        if (hit.collider.GetComponentInParent<Enemy>() is Enemy enemy)
        {
            enemy.TakeDamage(damage, isHeadshot);
        }
        
        Instantiate(bulletImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
    }
}