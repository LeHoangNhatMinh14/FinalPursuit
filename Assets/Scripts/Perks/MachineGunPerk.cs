using UnityEngine;

[CreateAssetMenu(menuName = "Perks/Machine Gun Perk")]
public class MachineGunPerk : Perk
{
    public GameObject weaponPrefab;

    public override void ApplyEffect()
    {
        var weaponHolder = GameObject.FindObjectOfType<WeaponHolder>();
        if (weaponHolder != null)
        {
            Debug.Log("Chose the machine gun buff");
            weaponHolder.EquipOnlyWeapon(weaponPrefab);
        }
    }
}