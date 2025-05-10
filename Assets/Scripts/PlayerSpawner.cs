using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;

public class PlayerSpawner : MonoBehaviour
{
    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!gameObject.scene.IsValid()) return;

        // Find or create spawn point
        var spawn = GameObject.FindWithTag("Spawnpoint") ?? new GameObject("TempSpawn");
        
        // Reset player
        var cc = GetComponent<CharacterController>();
        cc.enabled = false;
        transform.position = spawn.transform.position;
        transform.rotation = Quaternion.identity;
        cc.enabled = true;

        // Reset camera if using StarterAssets
        if (TryGetComponent<FirstPersonController>(out var fpc))
        {
            fpc.ResetCameraRotation();
        }
    }
}