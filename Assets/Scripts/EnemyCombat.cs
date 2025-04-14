using UnityEngine;
using UnityEngine.AI;

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

    [Header("Patrol Settings")]
    public float patrolRadius = 10f;
    public float patrolWaitTime = 2f;

    private Transform player;
    private float lastAttackTime;
    private AudioSource audioSource;
    private NavMeshAgent agent;
    private Vector3 patrolTarget;
    private float patrolTimer;

    private enum State { Patrol, Attack }
    private State currentState = State.Patrol;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Debug.Log("player found" + player);
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }

        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();

        if (muzzleFlash == null) Debug.LogWarning("Muzzle Flash is not assigned.");
        if (hitEffect == null) Debug.LogWarning("Hit Effect is not assigned.");
        if (gunshotSound == null) Debug.LogWarning("Gunshot Sound is not assigned.");

        ChooseNewPatrolTarget();
    }

    void Update()
    {
        bool canSeePlayer = CanSeePlayer();
        Debug.Log("Can see player: " + canSeePlayer);

        currentState = canSeePlayer ? State.Attack : State.Patrol;

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Attack:
                AttackPlayer();
                break;
        }
    }

    void Patrol()
    {
        Debug.Log("Patrolling...");
        agent.isStopped = false;

        if (Vector3.Distance(transform.position, patrolTarget) < 1f)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= patrolWaitTime)
            {
                ChooseNewPatrolTarget();
                patrolTimer = 0f;
            }
        }
        else
        {
            agent.SetDestination(patrolTarget);
        }
    }

    void ChooseNewPatrolTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolTarget = hit.position;
            Debug.Log("New patrol target chosen: " + patrolTarget);
        }
    }

    void AttackPlayer()
    {
        Debug.Log("Attempting to attack player.");

        agent.isStopped = true;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (ReadyToAttack())
        {
            Debug.Log("Attacking player!");
            AttemptDamage();
            lastAttackTime = Time.time;
        }
        else
        {
            Debug.Log("Waiting for attack cooldown.");
        }
    }

    bool ReadyToAttack()
    {
        return Time.time > lastAttackTime + attackCooldown;
    }

    bool CanSeePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        Debug.Log("Distance to player: " + distance);

        if (distance > attackRange)
        {
            Debug.Log("Player out of range.");
            return false;
        }

        Vector3 eyePosition = transform.position + Vector3.up * 1.5f;
        Vector3 directionToPlayer = (player.position - eyePosition).normalized;

        Debug.DrawRay(eyePosition, directionToPlayer * attackRange, Color.red);

        if (Physics.Raycast(eyePosition, directionToPlayer, out RaycastHit hit, attackRange))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);
            return hit.collider.CompareTag("Player");
        }

        return false;
    }

    void AttemptDamage()
    {
        PlayAttackEffects();

        Vector3 eyePosition = transform.position + Vector3.up * 1.5f;
        Vector3 directionToPlayer = (player.position - eyePosition).normalized;

        Debug.DrawRay(eyePosition, directionToPlayer * attackRange, Color.red, 1f);

        try
        {
            if (Physics.Raycast(eyePosition, directionToPlayer, out RaycastHit hit, attackRange, playerLayer))
            {
                Debug.Log("Raycast hit: " + hit.collider.name);

                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log("Player hit confirmed!");

                    var health = hit.collider.GetComponent<PlayerHealth>();
                    if (health != null)
                    {
                        Debug.Log("Applying damage: " + attackDamage);
                        health.TakeDamage(attackDamage);
                        SpawnHitEffect(hit.point, hit.normal);
                    }
                    else
                    {
                        Debug.LogError("No PlayerHealth component found on Player!");
                    }
                }
                else
                {
                    Debug.Log("Raycast hit non-player: " + hit.collider.tag);
                }
            }
            else
            {
                Debug.Log("Raycast missed.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Exception in AttemptDamage: " + e.Message);
        }
    }


    void PlayAttackEffects()
    {
         if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        if (gunshotSound != null)
        {
            audioSource.PlayOneShot(gunshotSound);
        }
    }

    void SpawnHitEffect(Vector3 position, Vector3 normal)
    {
        if (hitEffect != null)
        {
            Instantiate(hitEffect, position, Quaternion.LookRotation(normal));
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
