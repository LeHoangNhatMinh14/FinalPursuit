using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [Header("Damage")]
    public float bodyDamage = 25f;
    public float headDamage = 75f;

    [Header("Behavior")]
    public float fireRate = 1f; // 1 second between shots
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

    void Start()
    {
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }
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
        if (muzzleFlash) muzzleFlash.Play();
        if (gunAudio && shootSound) gunAudio.PlayOneShot(shootSound);

        for (int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 shotDirection = GetShotDirection();
            Debug.DrawRay(playerCamera.transform.position, shotDirection * maxDistance, Color.green, 0.2f);

            if (Physics.Raycast(playerCamera.transform.position, shotDirection, out RaycastHit hit, maxDistance))
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

    void ProcessHit(RaycastHit hit)
    {
        bool isHeadshot = hit.collider.CompareTag("Head");
        float damage = isHeadshot ? headDamage : bodyDamage;

        if (hit.collider.GetComponentInParent<Enemy>() is Enemy enemy)
        {
            enemy.TakeDamage(damage, isHeadshot);
        }

        if (bulletImpactPrefab)
        {
            Instantiate(bulletImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
}
