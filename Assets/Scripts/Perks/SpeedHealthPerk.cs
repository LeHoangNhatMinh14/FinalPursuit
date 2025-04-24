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
        // Health upgrade
        PlayerHealth healthComponent = GameObject.FindObjectOfType<PlayerHealth>();
        if (healthComponent != null)
        {
            healthComponent.maxHealth += healthBonus;
            healthComponent.currentHealth = healthComponent.maxHealth;
            Debug.Log($"[Perk] +{healthBonus} Max Health | Healed to full: {healthComponent.currentHealth}/{healthComponent.maxHealth}");
        }

        // Speed upgrade
        FirstPersonController playerController = GameObject.FindObjectOfType<FirstPersonController>();
        if (playerController != null)
        {
            playerController.MoveSpeed += speedBonus;
            Debug.Log($"[Perk] +{speedBonus} Movement Speed | New Speed: {playerController.MoveSpeed}");
        }
    }
}
