using UnityEngine;

public class CameraRig : MonoBehaviour
{
    public static CameraRig Instance;
    
    [Header("Settings")]
    public Vector3[] levelOffsets;
    
    private Transform player;
    private Vector3 targetOffset;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(Transform playerTransform)
    {
        player = playerTransform;
    }

    public void SetLevelIndex(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelOffsets.Length)
        {
            targetOffset = levelOffsets[levelIndex];
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.position + targetOffset;
            transform.LookAt(player);
        }
    }
}