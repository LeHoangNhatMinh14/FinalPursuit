using UnityEngine;
using StarterAssets;

[CreateAssetMenu(menuName = "Perks/Health and Speed Perk")]
public class HealthSpeedPerk : Perk
{
    [Header("Bonus Values")]
    public float healthBonus;
    public float speedBonus;

    public override void ApplyEffect()
    {
        // Apply health bonus
        var healthComponent = GameObject.FindObjectOfType<PlayerHealth>();
        if (healthComponent != null)
        {
            Debug.Log($"Applying health bonus: +{healthBonus}");
            healthComponent.maxHealth += healthBonus;
            
            // Safer way to handle current health:
           // healthComponent.currentHealth = Mathf.Min(
           //     healthComponent.currentHealth + healthBonus, 
          //      healthComponent.maxHealth
           // );
            
            // Alternative: Just heal to full health
            // healthComponent.currentHealth = healthComponent.maxHealth;
        }

        // Apply speed bonus
        var playerController = GameObject.FindObjectOfType<FirstPersonController>();
        if (playerController != null)
        {
            Debug.Log($"Applying speed bonus: +{speedBonus}");
            playerController.MoveSpeed += speedBonus;
        }
    }
}