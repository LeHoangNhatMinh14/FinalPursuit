using UnityEngine;

[CreateAssetMenu(menuName = "Perks/Weapon Damage Perk")]
public class WeaponDamagePerk : Perk
{
    public float damageMultiplier = 1.5f;

    public override void ApplyEffect()
    {
        var holder = FindObjectOfType<WeaponHolder>();
        if (holder == null) return;

        var currentWeapon = holder.GetCurrentWeapon();
        if (currentWeapon == null) return;

        currentWeapon.ApplyDamageMultiplier(damageMultiplier);
    }
}