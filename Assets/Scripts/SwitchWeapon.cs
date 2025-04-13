using UnityEngine;
using System.Collections.Generic;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Weapons")]
    public GameObject activeWeaponParent;
    public List<GameObject> weaponPrefabs;
    public int currentWeaponIndex = 0;

    [Header("Settings")]
    public KeyCode switchKey = KeyCode.Q;
    public float switchTime = 0.5f;
    
    private bool isSwitching = false;

    void Start()
    {
        // Activate starting weapon
        SwitchWeapon(currentWeaponIndex);
        Debug.Log($"Weapon Switcher initialized. Starting with weapon #{currentWeaponIndex}");
    }

    void Update()
    {
        if (Input.GetKeyDown(switchKey) && !isSwitching && weaponPrefabs.Count > 1)
        {
            int previousIndex = currentWeaponIndex;
            currentWeaponIndex = (currentWeaponIndex + 1) % weaponPrefabs.Count;
            
            Debug.Log($"Switching from weapon #{previousIndex} to weapon #{currentWeaponIndex}");
            StartCoroutine(SwitchWeaponRoutine(currentWeaponIndex));
        }
    }

    IEnumerator<WaitForSeconds> SwitchWeaponRoutine(int newIndex)
    {
        isSwitching = true;
        Debug.Log($"Weapon switch started to weapon #{newIndex}");
        
        // Disable current weapon
        if (activeWeaponParent.transform.childCount > 0)
        {
            GameObject oldWeapon = activeWeaponParent.transform.GetChild(0).gameObject;
            Debug.Log($"Destroying current weapon: {oldWeapon.name}");
            Destroy(oldWeapon);
        }

        // Instantiate new weapon
        GameObject newWeapon = Instantiate(weaponPrefabs[newIndex], activeWeaponParent.transform);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        
        Debug.Log($"Instantiated new weapon: {newWeapon.name}");

        // Wait for switch animation/time
        yield return new WaitForSeconds(switchTime);
        
        isSwitching = false;
        Debug.Log($"Weapon switch completed to weapon #{newIndex}");
    }

    // Call this directly if you don't want animation time
    public void SwitchWeapon(int newIndex)
    {
        if (newIndex >= 0 && newIndex < weaponPrefabs.Count)
        {
            Debug.Log($"Direct weapon switch requested to weapon #{newIndex}");
            currentWeaponIndex = newIndex;
            StartCoroutine(SwitchWeaponRoutine(newIndex));
        }
        else
        {
            Debug.LogWarning($"Invalid weapon index requested: {newIndex}");
        }
    }
}