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
            Debug.Log("Chose the health buff");
            player.maxHealth += healthBonus;
        }
    }
}