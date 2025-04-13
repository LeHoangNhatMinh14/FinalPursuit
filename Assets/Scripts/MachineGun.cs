using UnityEngine;

public class MachineGun : MonoBehaviour
{
    [Header("Damage")]
    public float bodyDamage = 15f;
    public float headDamage = 45f;

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

    void Start()
    {
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }
        else
        {
            Debug.Log("MachineGun initialized with " + bulletsPerShot + " bullets per shot");
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Debug.Log("Trigger pulled - attempting to fire");
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        Debug.Log("Firing machine gun burst");
        
        // Play effects
        if (muzzleFlash)
        {
            muzzleFlash.Play();
            Debug.Log("Muzzle flash activated");
        }

        if (gunAudio && shootSound)
        {
            gunAudio.PlayOneShot(shootSound);
            Debug.Log("Gunshot sound played");
        }

        // Fire all bullets
        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 shotDirection = GetShotDirection();
            Debug.DrawRay(playerCamera.transform.position, shotDirection * maxDistance, Color.red, 0.1f);
            
            if (Physics.Raycast(playerCamera.transform.position, shotDirection, out RaycastHit hit, maxDistance))
            {
                Debug.Log("Bullet " + (i+1) + " hit: " + hit.collider.name);
                ProcessHit(hit);
            }
            else
            {
                Debug.Log("Bullet " + (i+1) + " missed");
            }
        }
    }

    Vector3 GetShotDirection()
    {
        Vector3 direction = playerCamera.transform.forward;
        
        if (spreadAngle > 0)
        {
            direction = Quaternion.AngleAxis(Random.Range(-spreadAngle, spreadAngle), Vector3.up) * direction;
            direction = Quaternion.AngleAxis(Random.Range(-spreadAngle, spreadAngle), Vector3.right) * direction;
            Debug.Log("Applying bullet spread: " + direction);
        }
        
        return direction;
    }

    void ProcessHit(RaycastHit hit)
    {
        bool isHeadshot = hit.collider.CompareTag("Head");
        float damage = isHeadshot ? headDamage : bodyDamage;
        Debug.Log((isHeadshot ? "HEADSHOT" : "Body shot") + " for " + damage + " damage");
        
        Enemy enemy = hit.transform.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Debug.Log("Hit enemy: " + enemy.name);
            enemy.TakeDamage(damage, isHeadshot);
            
            // Impact effect
            if (bulletImpactPrefab)
            {
                Instantiate(bulletImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Debug.Log("Spawned bullet impact effect");
            }
        }
        else
        {
            Debug.Log("Hit non-enemy object: " + hit.collider.name);
        }
    }

    void OnDrawGizmos()
    {
        if (!playerCamera) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * maxDistance);
    }
}