using UnityEngine;
using StarterAssets;

[CreateAssetMenu(menuName = "Perks/Speed Perk")]
public class SpeedPerk : Perk
{
    public float speedBonus;

    public override void ApplyEffect()
    {
        var player = GameObject.FindObjectOfType<FirstPersonController>();
        if (player != null)
        {
            player.MoveSpeed += speedBonus;
        }
    }
}