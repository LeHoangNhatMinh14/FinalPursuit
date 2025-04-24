using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SceneLoader : MonoBehaviour
{
    [Header("Scene Selection")]
    [Tooltip("Type the exact name of the scene you want to load")]
    public string targetSceneName;

    void Start()
    {
        // Automatically connect to the button
        GetComponent<Button>().onClick.AddListener(LoadTargetScene);
    }

    void LoadTargetScene()
    {
        // Disable button to prevent double-clicks
        GetComponent<Button>().interactable = false;
        
        // Load the specified scene
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("No scene name specified!", this);
        }
    }
}