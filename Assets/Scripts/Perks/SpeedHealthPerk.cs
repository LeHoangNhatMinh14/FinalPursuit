using StarterAssets;
using UnityEngine;

[CreateAssetMenu(menuName = "Perks/Machine Gun Perk")]
public class SpeedHealthPerk : Perk
{
    public GameObject weaponPrefab;

    public override void ApplyEffect()
    {
        var health = GameObject.FindObjectOfType<PlayerHealth>();
        var speed = GameObject.FindObjectOfType<FirstPersonController>();
        if (health != null && speed != null) 
        {
            health.maxHealth += 1;
            speed.MoveSpeed += 1;
        }
    }
}