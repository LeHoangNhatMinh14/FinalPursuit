using UnityEngine;
using StarterAssets;

[CreateAssetMenu(menuName = "Perks/Jump Perk")]
public class JumpPerk : Perk
{
    public float jumpHeightBonus;

    public override void ApplyEffect()
    {
        var player = GameObject.FindObjectOfType<FirstPersonController>();
        if (player != null)
        {
            Debug.Log("Chose the jump height buff");
            player.JumpHeight += jumpHeightBonus;
        }
    }
}