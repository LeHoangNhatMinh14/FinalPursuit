using UnityEngine;

[CreateAssetMenu(menuName = "Perks/Health Perk")]
public class HealthPerk : Perk
{
    public float healthBonus;

    public override void ApplyEffect()
    {
        var player = GameObject.FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            player.maxHealth += healthBonus;
            player.currentHealth = player.maxHealth; // Heal fully when applying the perk
            Debug.Log("Health perk applied and healed to full.");
        }
    }
}