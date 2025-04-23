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
        // SAFEST way to find the player - works whether singleton or not
        var player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<FirstPersonController>();
        if (player == null)
        {
            Debug.LogWarning("MeleeWeaponPerk: Player not found!");
            return;
        }

        // Get components - using GetComponentInParent for broader search
        var playerHealth = player.GetComponentInParent<PlayerHealth>();
        var weaponHolder = player.GetComponentInChildren<WeaponHolder>(); // Changed to InChildren
        var audioSource = player.GetComponent<AudioSource>();

        // Apply melee weapon
        if (meleeWeaponPrefab != null)
        {
            if (weaponHolder != null)
            {
                weaponHolder.EquipWeapon(meleeWeaponPrefab);
                
                if (equipSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(equipSound);
                }
            }
            else
            {
                Debug.LogWarning("MeleeWeaponPerk: WeaponHolder component missing!");
            }
        }

        // Apply speed boost
        player.MoveSpeed += speedBonus;

        // Apply health changes
        if (playerHealth != null)
        {
            // Recommended approach - call public method
           // playerHealth.ModifyHealthStats(healthPenalty);
            
            /* Alternative if ModifyHealthStats doesn't exist:
            playerHealth.maxHealth += healthPenalty;
            playerHealth.currentHealth = Mathf.Min(
                playerHealth.currentHealth, 
                playerHealth.maxHealth
            );
            if (playerHealth.healthBar != null)
            {
                playerHealth.healthBar.UpdateHealth(
                    playerHealth.currentHealth, 
                    playerHealth.maxHealth
                );
            }
            */
        }
        else
        {
            Debug.LogWarning("MeleeWeaponPerk: PlayerHealth component missing!");
        }
    }
}