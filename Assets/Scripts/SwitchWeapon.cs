using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

    IEnumerator SwitchWeaponRoutine(int newIndex)
    {
        isSwitching = true;
        
        // 1. Hide current weapon
        if (activeWeaponParent.transform.childCount > 0)
        {
            Transform oldWeapon = activeWeaponParent.transform.GetChild(0);
            oldWeapon.gameObject.SetActive(false); // Disable instead of destroy
            Destroy(oldWeapon.gameObject); // Or keep pooled if you want faster switching
        }

        // 2. Create new weapon
        GameObject newWeapon = Instantiate(weaponPrefabs[newIndex], activeWeaponParent.transform);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        newWeapon.SetActive(true); // Ensure it's active
        
        Debug.Log($"Spawned: {newWeapon.name} at {activeWeaponParent.name}");

        yield return new WaitForSeconds(switchTime);

        isSwitching = false;

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