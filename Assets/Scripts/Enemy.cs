using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    [SerializeField] private HealthBar healthBar;
    
    [Header("Visual Feedback (Optional)")]
    [SerializeField] private Material hitMaterial;
    [SerializeField] private GameObject deathEffect;
    
    [Header("Stun Settings")]
    public float stunDuration = 3f;
    public Material stunMaterial; // Visual feedback (optional)
    private bool isStunned = false;
    private Coroutine stunRoutine;
    private float currentHealth;
    private Material originalMaterial;
    private Renderer enemyRenderer;

    void Start()
    {
        enemyRenderer = GetComponentInChildren<Renderer>();
        if (enemyRenderer != null)
            originalMaterial = enemyRenderer.material;
        
        currentHealth = maxHealth;
        
        InitializeHealthBar();
        Debug.Log($"Enemy spawned with {currentHealth}/{maxHealth} HP");

        if (EnemyCounter.Instance != null)
        {
            EnemyCounter.Instance.AddEnemy(gameObject);
            Debug.Log($"Enemy registered: {gameObject.name}");
        }
    }

    void InitializeHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }
        else
        {
            Debug.LogWarning("HealthBar reference not assigned in Inspector. Health bar will not update.", this);
        }
    }

    public void TakeDamage(float damage, bool isHeadshot)
    {
        if (currentHealth <= 0 || isStunned) return;

        currentHealth -= damage;
        string hitType = isHeadshot ? "HEADSHOT" : "bodyshot";
        
        // Update health bar (optional)
        if (healthBar != null)
            healthBar.UpdateHealth(currentHealth, maxHealth);
        
        // Visual feedback (optional)
        if (hitMaterial != null && enemyRenderer != null)
            StartCoroutine(HitFlash());
        
        Debug.Log($"{hitType}! {damage} damage | HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0) Die();
    }

    private IEnumerator HitFlash()
    {
        if (enemyRenderer == null || hitMaterial == null) yield break;
        
        enemyRenderer.material = hitMaterial;
        yield return new WaitForSeconds(0.1f);
        if (originalMaterial != null) // Safety check
            enemyRenderer.material = originalMaterial;
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        
        // Death effects (optional)
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        // Only unregister if the game isn't quitting
        if (EnemyCounter.Instance != null && !GameManager.isApplicationQuitting)
        {
            EnemyCounter.Instance.RemoveEnemy(gameObject);
            Debug.Log($"Enemy unregistered: {gameObject.name}");
        }
    }
    
    public void Stun(float duration)
    {
        if (isStunned)
        {
            Debug.Log($"[STUN] {gameObject.name} is already stunned - ignoring new stun");
            return;
        }
        
        Debug.Log($"[STUN] {gameObject.name} stunned for {duration} seconds");

        // Cancel existing stun if any
        if (stunRoutine != null) 
        {
            Debug.Log($"[STUN] {gameObject.name} had existing stun - replacing");
            StopCoroutine(stunRoutine);
        }
        
        stunRoutine = StartCoroutine(StunRoutine(duration));
    }

    IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        Debug.Log($"[STUN] {gameObject.name} stun started at {Time.time:F2}");

        // Visual feedback (optional)
        if (enemyRenderer != null && stunMaterial != null)
        {
            Debug.Log($"[STUN] {gameObject.name} applying stun material");
            enemyRenderer.material = stunMaterial;
        }
        
        // Disable components
        EnemyCombat combat = GetComponent<EnemyCombat>();
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        
        if (combat != null) 
        {
            Debug.Log($"[STUN] {gameObject.name} disabling combat");
            combat.enabled = false;
        }
        
        if (agent != null)
        {
            Debug.Log($"[STUN] {gameObject.name} stopping movement");
            agent.isStopped = true;
        }

        yield return new WaitForSeconds(duration);

        // Re-enable
        Debug.Log($"[STUN] {gameObject.name} stun ended at {Time.time:F2}");
        isStunned = false;
        
        if (enemyRenderer != null && originalMaterial != null)
        {
            Debug.Log($"[STUN] {gameObject.name} restoring original material");
            enemyRenderer.material = originalMaterial;
        }

        if (combat != null) 
        {
            Debug.Log($"[STUN] {gameObject.name} re-enabling combat");
            combat.enabled = true;
        }
        
        if (agent != null)
        {
            Debug.Log($"[STUN] {gameObject.name} resuming movement");
            agent.isStopped = false;
        }
    }
}