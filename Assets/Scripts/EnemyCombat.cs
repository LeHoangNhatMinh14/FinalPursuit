using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackRange = 10f;
    public float attackDamage = 10f;
    public float attackCooldown = 2f;
    public LayerMask playerLayer;

    [Header("Patrol Settings")]
    public Transform InitialPosition;
    public Transform patrolNode;
    public float nodeWaitTime = 1f;
    public float nodeReachedDistance = 0.5f;
    public float newPositionInterval = 5f; // Time between getting new positions
    public float wanderRadius = 10f; // Radius for random position generation

    [Header("Visual Feedback")]
    public ParticleSystem muzzleFlash;
    public GameObject hitEffect;
    public AudioClip gunshotSound;

    private Transform player;
    private float lastAttackTime;
    private AudioSource audioSource;
    private NavMeshAgent agent;
    private float waitTimer;
    private Vector3 initialPosition;
    private bool goingToNode = true;
    private float lastPositionUpdateTime;

    private enum State { Patrol, Attack }
    private State currentState = State.Patrol;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();

        // Try to find nearest point on the NavMesh
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas))
        {
            initialPosition = navHit.position;
        }
        else
        {
            // Debug.LogWarning("Initial position not found on NavMesh. Using raw position.");
            initialPosition = transform.position;
        }

        if (!patrolNode)
        {
            // patrolNode = new GameObject("PatrolNode").transform;
            GetNewPosition(); // Generate initial patrol node position
        }

        // Debug.Log($"[Start] Initial position: {initialPosition}, Patrol node: {patrolNode.position}");
        SetNextDestination();
        lastPositionUpdateTime = Time.time;
    }

    void Update()
    {
        if (!player)
        {
            // Debug.LogWarning("[Update] No player found.");
            return;
        }

        if (CanSeePlayer())
        {
            if (currentState != State.Attack)
            {
                // Debug.Log("[State] Switching to ATTACK state.");
                currentState = State.Attack;
            }
            AttackPlayer();
        }
        else
        {
            if (currentState != State.Patrol)
            {
                // Debug.Log("[State] Switching to PATROL state.");
                currentState = State.Patrol;
            }
            Patrol();
        }
    }

    void Patrol()
    {
        agent.isStopped = false;

        // Check if it's time to get a new position
        if (Time.time - lastPositionUpdateTime > newPositionInterval)
        {
            GetNewPosition();
            lastPositionUpdateTime = Time.time;
        }

        Vector3 rawTarget = goingToNode ? patrolNode.position : initialPosition;
        Vector3 flatTarget = new Vector3(rawTarget.x, transform.position.y, rawTarget.z);

        float distanceToTarget = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                                                new Vector3(flatTarget.x, 0, flatTarget.z));

        // Debug.Log($"[Patrol] Moving to {(goingToNode ? "Patrol Node" : "Initial Position")}, Distance (flat): {distanceToTarget}");

        if (distanceToTarget <= nodeReachedDistance)
        {
            waitTimer += Time.deltaTime;
            // Debug.Log($"[Patrol] Reached node. Waiting... Timer: {waitTimer}");

            if (waitTimer >= nodeWaitTime)
            {
                goingToNode = !goingToNode;
                // Debug.Log($"[Patrol] Switching direction. Going to: {(goingToNode ? "Patrol Node" : "Initial Position")}");
                SetNextDestination();
                waitTimer = 0f;
            }
        }
    }

    void GetNewPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += initialPosition;
        
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            patrolNode.position = hit.position;
            // Debug.Log($"[GetNewPosition] New patrol position set to: {hit.position}");
            
            // If we're currently going to the node, update the destination immediately
            if (goingToNode)
            {
                SetNextDestination();
            }
        }
        else
        {
            // Debug.LogWarning("[GetNewPosition] Failed to find valid NavMesh position");
        }
    }

    void SetNextDestination()
    {
        Vector3 rawTarget = goingToNode ? patrolNode.position : initialPosition;
        Vector3 flatTarget = new Vector3(rawTarget.x, transform.position.y, rawTarget.z);
        // Debug.Log($"[SetNextDestination] Setting destination to: {flatTarget}");
        agent.SetDestination(flatTarget);
    }

    void AttackPlayer()
    {
        agent.isStopped = true;
        Vector3 lookTarget = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookTarget);

        if (ReadyToAttack())
        {
            AttemptDamage();
            lastAttackTime = Time.time;
        }
    }

    bool ReadyToAttack()
    {
        bool ready = Time.time > lastAttackTime + attackCooldown;
        return ready;
    }

    bool CanSeePlayer()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackRange) return false;

        Vector3 direction = (player.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, attackRange))
        {
            return hit.collider.CompareTag("Player");
        }
        return false;
    }

    protected virtual void AttemptDamage()
    {
        PlayAttackEffects();

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, attackRange, playerLayer))
        {
            Debug.Log($"[Damage] Hit something: {hit.collider.name}");
            if (hit.collider.TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                Debug.Log("[Damage] Damaging player.");
                health.TakeDamage(attackDamage);
                SpawnHitEffect(hit.point, hit.normal);
            }
        }
    }

    protected virtual void PlayAttackEffects()
    {
        if (muzzleFlash) muzzleFlash.Play();
        if (gunshotSound) audioSource.PlayOneShot(gunshotSound);
    }

    void SpawnHitEffect(Vector3 position, Vector3 normal)
    {
        if (hitEffect)
        {
            Instantiate(hitEffect, position, Quaternion.LookRotation(normal));
        }
    }

    void OnDrawGizmosSelected()
    {
        if (patrolNode)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, patrolNode.position);
        }

        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw wander radius
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(initialPosition, wanderRadius);
    }
}