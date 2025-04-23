using UnityEngine;
using System.Collections.Generic;
using StarterAssets;

public class GamePersistentData : MonoBehaviour
{
    public static GamePersistentData Instance;
    
    [Header("Weapon Data")]
    public GameObject currentWeaponPrefab;
    public GameObject defaultWeaponPrefab; // Set this in inspector
    public List<GameObject> unlockedWeapons = new List<GameObject>();

    [Header("Player Stats")]
    public float playerMoveSpeed;
    public float playerMaxHealth;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDefaults();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDefaults()
    {
        // Add default weapon to unlocked weapons
        if (defaultWeaponPrefab != null && !unlockedWeapons.Contains(defaultWeaponPrefab))
        {
            unlockedWeapons.Add(defaultWeaponPrefab);
        }
        
        // Set current weapon if none exists
        if (currentWeaponPrefab == null)
        {
            currentWeaponPrefab = defaultWeaponPrefab;
        }
    }

    public void SavePlayerStats(FirstPersonController player, PlayerHealth health)
    {
        if (player != null) playerMoveSpeed = player.MoveSpeed;
        if (health != null) playerMaxHealth = health.maxHealth;
    }

    public void LoadPlayerStats(FirstPersonController player, PlayerHealth health)
    {
        if (player != null) player.MoveSpeed = playerMoveSpeed;
        if (health != null)
        {
            health.maxHealth = playerMaxHealth;
            health.currentHealth = Mathf.Min(health.currentHealth, playerMaxHealth);
        }
    }
}