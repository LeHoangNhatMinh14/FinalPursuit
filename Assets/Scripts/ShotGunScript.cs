using UnityEngine;

public class Shotgun : WeaponBase
{
    [Header("Behavior")]
    public float fireRate = 1f;
    public float spreadAngle = 10f;
    public float maxDistance = 50f;
    public int pelletsPerShot = 8;
    
    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public AudioSource gunAudio;
    public AudioClip shootSound;
    public GameObject bulletImpactPrefab;

    private float nextFireTime;
    private Camera playerCamera;

    private void Start()
    {
        weaponName = "Shotgun";
        baseBodyDamage = 25f;
        baseHeadDamage = 75f;
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            FireShotgun();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FireShotgun()
    {
        muzzleFlash?.Play();
        gunAudio?.PlayOneShot(shootSound);

        for (int i = 0; i < pelletsPerShot; i++)
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