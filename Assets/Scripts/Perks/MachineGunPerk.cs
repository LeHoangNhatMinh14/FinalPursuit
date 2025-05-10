using UnityEngine;

[CreateAssetMenu(menuName = "Perks/Machine Gun Perk")]
public class MachineGunPerk : Perk
{
    public GameObject weaponPrefab;

    public override void ApplyEffect()
    {
        if (WeaponHolder.Instance != null && weaponPrefab != null)
        {
            WeaponHolder.Instance.EquipWeapon(weaponPrefab);
            
            // Update persistent data
            if (GamePersistentData.Instance != null)
            {
                GamePersistentData.Instance.currentWeaponPrefab = weaponPrefab;
                if (!GamePersistentData.Instance.unlockedWeapons.Contains(weaponPrefab))
                {
                    GamePersistentData.Instance.unlockedWeapons.Add(weaponPrefab);
                }
            }
        }
    }
}