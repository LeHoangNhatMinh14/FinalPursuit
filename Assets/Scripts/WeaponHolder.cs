using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public Transform weaponParent; // Assign this to "Active weapon" itself in the Inspector

    public void EquipOnlyWeapon(GameObject newWeaponPrefab)
    {
        // Delete the current weapon
        foreach (Transform child in weaponParent)
        {
            Destroy(child.gameObject);
        }

        // Spawn new weapon
        GameObject newWeapon = Instantiate(newWeaponPrefab, weaponParent);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;

        Debug.Log($"Equipped new weapon: {newWeapon.name}");
    }
}
