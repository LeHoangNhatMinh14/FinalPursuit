using UnityEngine;
using StarterAssets;

[CreateAssetMenu(menuName = "Perks/Melee Weapon Perk")]
public class MeleeWeaponPerk : Perk
{
    public GameObject meleeWeaponPrefab;
    public float speedBonus = 5f;
    public float healthPenalty = -2f;
    public AudioClip equipSound;

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        
        // Weapon part
        if (WeaponHolder.Instance != null && meleeWeaponPrefab != null)
        {
            WeaponHolder.Instance.EquipWeapon(meleeWeaponPrefab);
            
            if (GamePersistentData.Instance != null)
            {
                GamePersistentData.Instance.currentWeaponPrefab = meleeWeaponPrefab;
                if (!GamePersistentData.Instance.unlockedWeapons.Contains(meleeWeaponPrefab))
                {
                    GamePersistentData.Instance.unlockedWeapons.Add(meleeWeaponPrefab);
                }
            }
        }

        // Stats part
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var controller = player.GetComponent<FirstPersonController>();
            var health = player.GetComponent<PlayerHealth>();
            
            if (controller != null)
            {
                controller.MoveSpeed += speedBonus;
                if (GamePersistentData.Instance != null)
                {
                    GamePersistentData.Instance.playerMoveSpeed = controller.MoveSpeed;
                }
            }
            
            if (health != null)
            {
                health.maxHealth += healthPenalty;
                health.currentHealth = Mathf.Min(health.currentHealth, health.maxHealth);
                
                if (GamePersistentData.Instance != null)
                {
                    GamePersistentData.Instance.playerMaxHealth = health.maxHealth;
                }
            }
        }
    }
}