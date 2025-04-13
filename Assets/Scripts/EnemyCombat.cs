using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackRange = 10f;
    public float attackDamage = 10f;
    public float attackCooldown = 2f;
    public LayerMask playerLayer;
    
    [Header("Visual Feedback")]
    public ParticleSystem muzzleFlash;
    public GameObject hitEffect;
    public AudioClip gunshotSound;
    
    private Transform player;
    private float lastAttackTime;
    private AudioSource audioSource;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        // Debug.Log("[Enemy] Combat system initialized");
    }

    void Update()
    {
        if (CanSeePlayer() && ReadyToAttack())
        {
            AttackPlayer();
        }
    }

    bool ReadyToAttack()
    {
        bool ready = Time.time > lastAttackTime + attackCooldown;
        // if (!ready) Debug.Log($"[Enemy] Attack on cooldown. Ready in {lastAttackTime + attackCooldown - Time.time:F1}s");
        return ready;
    }

    bool CanSeePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > attackRange)
        {
            // Debug.Log($"[Enemy] Player too far: {distance:F1}m/{attackRange}m");
            return false;
        }

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        
        // Visualize the LOS check in Scene view (only visible in Editor)
        Debug.DrawRay(transform.position, directionToPlayer * attackRange, Color.red, 0.1f);

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, attackRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // Debug.Log("[Enemy] LOS: CLEAR - Player visible");
                return true;
            }
            else
            {
                // Debug.Log($"[Enemy] LOS: BLOCKED by {hit.collider.name}");
                return false;
            }
        }
        
        // Debug.Log("[Enemy] LOS: No obstructions but player not hit (edge case)");
        return false;
    }

    void AttackPlayer()
    {
        // Debug.Log("[Enemy] INITIATING ATTACK");
        
        AttemptDamage();
        
        lastAttackTime = Time.time;
        // Debug.Log($"[Enemy] Next attack available at {lastAttackTime + attackCooldown:F1}");
    }

    void PlayAttackEffects()
    {
        muzzleFlash?.Play();
        audioSource.PlayOneShot(gunshotSound);
        // Debug.Log("[Enemy] Fired weapon");
    }

    void AttemptDamage()
    {
        if (Physics.Raycast(transform.position, 
            (player.position - transform.position).normalized, 
            out RaycastHit hit,
            attackRange,
            playerLayer) && 
            hit.collider.CompareTag("Player"))
        {
            if (hit.collider.TryGetComponent(out PlayerHealth health))
            {
                health.TakeDamage(attackDamage);
                SpawnHitEffect(hit.point, hit.normal);
                // Debug.Log($"[Enemy] HIT: Dealt {attackDamage} damage");
            }
            else
            {
                // Debug.LogError("[Enemy] Player missing PlayerHealth component!");
            }
        }
        else
        {
            // Debug.Log("[Enemy] ATTACK MISSED");
        }
    }

    void SpawnHitEffect(Vector3 position, Vector3 normal)
    {
        if (hitEffect != null)
        {
            Instantiate(hitEffect, position, Quaternion.LookRotation(normal));
            // Debug.Log("[Enemy] Spawned impact effect");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
        if (player != null)
        {
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}