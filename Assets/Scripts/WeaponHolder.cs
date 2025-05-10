using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponHolder : MonoBehaviour
{
    public static WeaponHolder Instance;
    public Transform weaponParent;
    private WeaponBase currentWeapon;
    private void Start()
    {
        if (weaponParent == null)
        {
            weaponParent = GameObject.Find("WeaponParent").transform;
            GameObject.Find("ActiveWeapon").SetActive(false);
        }
        if (GamePersistentData.Instance != null)
        {
            // Load either the saved weapon or default
            var weaponToEquip = GamePersistentData.Instance.currentWeaponPrefab ?? 
                            GamePersistentData.Instance.defaultWeaponPrefab;
            EquipWeapon(weaponToEquip);
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-parent the weapon to the new scene's weapon parent
        if (currentWeapon != null)
        {
            currentWeapon.transform.SetParent(weaponParent);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
        }
        else if (GamePersistentData.Instance != null && GamePersistentData.Instance.currentWeaponPrefab != null)
        {
            EquipWeapon(GamePersistentData.Instance.currentWeaponPrefab);
        }
    }

public void EquipWeapon(GameObject weaponPrefab)
{
    if (weaponPrefab == null) return;

    // Clear current weapon
    if (currentWeapon != null)
    {
        Destroy(currentWeapon.gameObject);
    }

    // Instantiate new weapon - prefab handles its own positioning
    GameObject newWeapon = Instantiate(weaponPrefab, weaponParent);
    currentWeapon = newWeapon.GetComponent<WeaponBase>();

    // Update persistent data
    if (GamePersistentData.Instance != null)
    {
        GamePersistentData.Instance.currentWeaponPrefab = weaponPrefab;
        if (!GamePersistentData.Instance.unlockedWeapons.Contains(weaponPrefab))
        {
            GamePersistentData.Instance.unlockedWeapons.Add(weaponPrefab);
        }
    }
}

    public WeaponBase GetCurrentWeapon()
    {
        return currentWeapon;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}