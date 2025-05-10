using UnityEngine;
using StarterAssets;

public class PlayerManager : MonoBehaviour
{
    private FirstPersonController playerController;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        playerController = GetComponent<FirstPersonController>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        if (GamePersistentData.Instance != null)
        {
            GamePersistentData.Instance.LoadPlayerStats(playerController, playerHealth);
        }
    }

    private void OnDestroy()
    {
        if (GamePersistentData.Instance != null)
        {
            GamePersistentData.Instance.SavePlayerStats(playerController, playerHealth);
        }
    }
}