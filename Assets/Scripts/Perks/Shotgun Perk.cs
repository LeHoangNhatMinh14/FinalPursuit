using UnityEngine;

[CreateAssetMenu(menuName = "Perks/Shotgun Perk")]
public class ShotgunPerk : Perk
{
    [Header("Weapon Assignment")]
    public GameObject shotgunPrefab;
    
    [Header("Shotgun Buffs")]
    [Range(0.1f, 2f)] public float damageMultiplier = 1.2f;
    [Range(1, 20)] public int extraPellets = 2;
    [Range(-0.5f, 0.5f)] public float spreadReduction = -0.1f; // Negative = tighter spread

    public override void ApplyEffect()
    {
        if (WeaponHolder.Instance != null && shotgunPrefab != null)
        {
            // Equip the shotgun
            WeaponHolder.Instance.EquipWeapon(shotgunPrefab);
            
            // Apply shotgun-specific buffs
           // Shotgun shotgun = WeaponHolder.Instance.CurrentWeapon.GetComponent<Shotgun>();
          //  if (shotgun != null)
          //  {
         //       shotgun.damageMultiplier *= damageMultiplier;
        //        shotgun.pelletsPerShot += extraPellets;
         //       shotgun.spreadAngle += spreadReduction;
         //   }

            // Update persistent data
            if (GamePersistentData.Instance != null)
            {
                GamePersistentData.Instance.currentWeaponPrefab = shotgunPrefab;
                if (!GamePersistentData.Instance.unlockedWeapons.Contains(shotgunPrefab))
                {
                    GamePersistentData.Instance.unlockedWeapons.Add(shotgunPrefab);
                }
            }
        }
    }
}