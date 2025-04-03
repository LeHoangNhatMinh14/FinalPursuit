using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    [SerializeField] private HealthBar healthBar;
    
    // [Header("Visual Feedback")]
    // [SerializeField] private Material hitMaterial;
    // [SerializeField] private GameObject deathEffect;
    
    private float currentHealth;
    private Material originalMaterial;
    private Renderer enemyRenderer;

    void Start()
    {
        enemyRenderer = GetComponentInChildren<Renderer>();
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
            Debug.LogError("HealthBar reference not assigned in Inspector!", this);
        }
    }

    public void TakeDamage(float damage, bool isHeadshot)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        string hitType = isHeadshot ? "HEADSHOT" : "bodyshot";
        
        // Update health bar
        if (healthBar != null)
            healthBar.UpdateHealth(currentHealth, maxHealth);
        
        // // Visual feedback
        // StartCoroutine(HitFlash());
        // Debug.Log($"{hitType}! {damage} damage | HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0) Die();
    }

    // private IEnumerator HitFlash()
    // {
    //     enemyRenderer.material = hitMaterial;
    //     yield return new WaitForSeconds(0.1f);
    //     enemyRenderer.material = originalMaterial;
    // }

    private void Die()
    {
        Debug.Log("Enemy died!");
        
        // // Death effects
        // if (deathEffect != null)
        //     Instantiate(deathEffect, transform.position, Quaternion.identity);
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
}