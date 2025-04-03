using UnityEngine;

public static class GameManager
{
    public static bool isApplicationQuitting = false;

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        Application.quitting += () => isApplicationQuitting = true;
    }
}
