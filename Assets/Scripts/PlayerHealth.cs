using StarterAssets;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    [SerializeField] private HealthBar healthBar; // Reference to your health bar
    
    private float currentHealth;
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
        // Disable controls
        if (fpsController != null)
            fpsController.enabled = false;
            
        // Update health bar to empty
        if (healthBar != null)
            healthBar.UpdateHealth(0, maxHealth);
        
        Debug.Log("Player died!");
        // Add any additional death logic here
    }
}