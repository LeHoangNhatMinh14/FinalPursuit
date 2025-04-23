using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public Transform weaponParent;
    private WeaponBase currentWeapon;

    public void EquipWeapon(GameObject weaponPrefab)
    {
        // Clear current weapon
        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }

        // Instantiate new weapon
        GameObject newWeapon = Instantiate(weaponPrefab, weaponParent);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        
        currentWeapon = newWeapon.GetComponent<WeaponBase>();
    }

    public WeaponBase GetCurrentWeapon()
    {
        return currentWeapon;
    }
}