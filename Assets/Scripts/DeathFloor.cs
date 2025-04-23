using UnityEngine;

public class InstantDeathFloor : MonoBehaviour
{
    [Tooltip("Tag of the player object")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has the player tag
        if (other.CompareTag(playerTag))
        {
            DestroyPlayer(other.gameObject);
        }
    }

    private void DestroyPlayer(GameObject player)
    {
        // Simply destroy the player object
        Destroy(player);
        
        // Or if you have a game manager handling death:
        // GameManager.Instance.PlayerDied();
    }
}