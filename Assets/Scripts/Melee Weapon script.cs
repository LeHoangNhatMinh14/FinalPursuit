using UnityEngine;

public class Katana : MonoBehaviour
{
    [Header("Damage")]
    public float lightAttackDamage = 15f;
    public float heavyAttackDamage = 30f;
    public float headDamageMultiplier = 2f;

    [Header("Behavior")]
    public float lightAttackCooldown = 0.5f;
    public float heavyAttackCooldown = 1.2f;
    public float attackRange = 2f;
    public float attackWidth = 0.5f; // Width of the sword swing arc

    [Header("Effects")]
    public AudioSource swordAudio;
    public AudioClip swingSound;
    public AudioClip hitSound;
    public ParticleSystem slashEffect;
    public GameObject bloodEffectPrefab;

    private float nextAttackTime;
    private Camera playerCamera;
    private bool isAttacking;

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
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime) // Light attack
        {
            PerformAttack(lightAttackDamage, lightAttackCooldown);
        }
        else if (Input.GetMouseButtonDown(1) && Time.time >= nextAttackTime) // Heavy attack
        {
            PerformAttack(heavyAttackDamage, heavyAttackCooldown);
        }
    }

    void PerformAttack(float damage, float cooldown)
    {
        nextAttackTime = Time.time + cooldown;
        isAttacking = true;

        // Play effects
        if (slashEffect) slashEffect.Play();
        if (swordAudio && swingSound) swordAudio.PlayOneShot(swingSound);

        // Detect hits
        CheckForHits(damage);

        // Reset attack state after animation would complete
        Invoke(nameof(ResetAttack), cooldown * 0.8f);
    }

    void CheckForHits(float baseDamage)
    {
        Vector3 attackCenter = playerCamera.transform.position + playerCamera.transform.forward * (attackRange / 2f);
        Collider[] hitColliders = Physics.OverlapBox(attackCenter, 
            new Vector3(attackWidth, attackWidth, attackRange / 2f), 
            playerCamera.transform.rotation);

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player") || hit.isTrigger) continue;

            bool isHeadshot = hit.CompareTag("Head");
            float damage = isHeadshot ? baseDamage * headDamageMultiplier : baseDamage;

            if (hit.GetComponentInParent<Enemy>() is Enemy enemy)
            {
                enemy.TakeDamage(damage, isHeadshot);
                PlayHitEffects(hit.ClosestPoint(transform.position));
            }
        }
    }

    void PlayHitEffects(Vector3 hitPosition)
    {
        if (swordAudio && hitSound) swordAudio.PlayOneShot(hitSound);
        if (bloodEffectPrefab)
        {
            Instantiate(bloodEffectPrefab, hitPosition, Quaternion.identity);
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    // Visualize attack range in editor
    void OnDrawGizmosSelected()
    {
        if (playerCamera == null) return;
        
        Vector3 center = playerCamera.transform.position + playerCamera.transform.forward * (attackRange / 2f);
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(center, playerCamera.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(attackWidth * 2, attackWidth * 2, attackRange));
    }
}