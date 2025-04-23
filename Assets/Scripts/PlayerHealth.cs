using StarterAssets;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Lives")]
    public int maxLives = 3;
    public int currentLives = 3;
    [Header("Health Settings")]
    public float maxHealth = 100f;
    [SerializeField] private HealthBar healthBar; // Reference to your health bar
    
    public float currentHealth;
    private FirstPersonController fpsController;

    void Start()
    {
        currentHealth = maxHealth;
        fpsController = GetComponentInParent<FirstPersonController>();
        
        // Initialize health bar
        if (healthBar != null)
            healthBar.UpdateHealth(currentHealth, maxHealth);
        else
            Debug.LogWarning("HealthBar reference not set in PlayerHealth!");
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        // Update health bar
        if (healthBar != null)
            healthBar.UpdateHealth(currentHealth, maxHealth);
        else
            Debug.LogWarning("Took damage but HealthBar reference is missing!");

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        currentLives--;
        Debug.Log($"Player lost a life! Remaining lives: {currentLives}");

        if (currentLives > 0)
        {
            // Respawn or revive logic
            currentHealth = maxHealth;
            fpsController.enabled = true;

            if (healthBar != null)
                healthBar.UpdateHealth(currentHealth, maxHealth);

            // Notify EnemyCounter
            EnemyCounter.Instance?.OnPlayerRespawn(currentLives);
        }
        else
        {
            // Final death — game over
            if (fpsController != null)
                fpsController.enabled = false;

            Debug.Log("Out of lives. Game over!");
            EnemyCounter.Instance?.OnPlayerGameOver();
        }
    }
}