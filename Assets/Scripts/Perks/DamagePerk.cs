// using UnityEngine;

// [CreateAssetMenu(menuName = "Perks/Damage Perk")]
// public class DamagePerk : Perk
// {
//     public float damageBonus;

//     public override void ApplyEffect()
//     {
//         var weaponHolder = GameObject.FindObjectOfType<WeaponHolder>();
//         if (player != null)
//         {
//             Debug.Log("Chose the health buff");
//             foreach (Transform child in weaponHolder.weaponParent)
//             {
//                 var weapon = child.GetComponent<Weapon>();
//                 if (weapon != null)
//                 {
//                     weapon.damage += damageBonus;
//                     Debug.Log($"Increased {weapon.name}'s damage by {damageBonus}");
//                 }
//             }
//         }
//     }
// }