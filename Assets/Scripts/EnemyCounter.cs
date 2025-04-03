using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;
using UnityEditor;


public class EnemyCounter : MonoBehaviour
{
    public static EnemyCounter Instance;
    
    [Header("Scene Transition")]
    #if UNITY_EDITOR
    [SerializeField] private SceneAsset nextSceneAsset; // Editor-only reference
    #endif
    [SerializeField] private string nextSceneName; // Runtime name

    private int currentEnemies = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            #if UNITY_EDITOR
            if (nextSceneAsset != null)
            {
                nextSceneName = nextSceneAsset.name;
            }
            #endif
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddEnemy(GameObject enemy)
    {
        currentEnemies++;
        Debug.Log($"Enemy added: {enemy.name}. Total: {currentEnemies}");
    }

    public void RemoveEnemy(GameObject enemy)
    {
        currentEnemies--;
        Debug.Log($"Enemy removed: {enemy.name}. Remaining: {currentEnemies}");

        if (currentEnemies <= 0)
        {
            LevelComplete();
        }
    }

    void LevelComplete()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            // Find player without PersistentPlayer
            var player = FindObjectOfType<FirstPersonController>();
            if (player != null)
            {
                player.GetComponent<CharacterController>().enabled = false;
                player.enabled = false;
            }
            
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("No next scene assigned!");
        }
    }
}