using UnityEngine;
using StarterAssets;

[CreateAssetMenu(menuName = "Perks/Damage-Speed Tradeoff")]
public class DamageSpeedPerk : Perk
{
    [Header("Damage Modifier")]
    [Tooltip("1.0 = normal damage, 0.5 = half damage, 2.0 = double damage")]
    public float damageMultiplier = 0.5f; // Default: -50% damage

    [Header("Speed Modifier")] 
    public float speedBonus = 1f; // Default: +1 speed

    public override void ApplyEffect()
    {
        ApplyWeaponDamageMod();
        ApplySpeedBonus();
    }

    private void ApplyWeaponDamageMod()
    {
        var holder = FindObjectOfType<WeaponHolder>();
        if (holder == null) return;

        var currentWeapon = holder.GetCurrentWeapon();
        if (currentWeapon == null) return;

        currentWeapon.ApplyDamageMultiplier(damageMultiplier);
        Debug.Log($"Weapon damage multiplied by {damageMultiplier}x");
    }

    private void ApplySpeedBonus()
    {
        var player = FindObjectOfType<FirstPersonController>();
        if (player != null)
        {
            player.MoveSpeed += speedBonus;
            Debug.Log($"Speed increased by {speedBonus}");
        }
    }
}