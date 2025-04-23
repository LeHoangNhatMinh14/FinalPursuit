using UnityEngine;

[RequireComponent(typeof(BoxCollider))] // Ensures collider exists
public class TeleportPlatform : MonoBehaviour
{
    [Header("Teleport Settings")]
    public float teleportHeight = 2f;
    public KeyCode teleportKey = KeyCode.E;
    public LayerMask playerLayer; // Set this to your player's layer

    [Header("Effects")]
    public GameObject teleportEffectPrefab;
    public AudioClip teleportSound;
    public float soundVolume = 0.5f;

    private GameObject player;
    private bool canTeleport;
    private BoxCollider triggerCollider;

    void Start()
    {
        // Set up trigger collider
        triggerCollider = GetComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        
        // Make sure we have a player tag defined
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            Debug.LogError("No GameObject with 'Player' tag found in scene!");
        }
    }

    void Update()
    {
        if (canTeleport && Input.GetKeyDown(teleportKey))
        {
            TeleportPlayer();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            canTeleport = true;
            Debug.Log("Player in range - Press E to teleport", this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTeleport = false;
            player = null;
        }
    }

    void TeleportPlayer()
    {
        if (player == null) return;

        // Calculate teleport position
        Vector3 teleportPos = transform.position;
        teleportPos.y += teleportHeight;

        // Check if destination is clear
        if (!IsPositionClear(teleportPos))
        {
            Debug.Log("Teleport blocked - space above is occupied", this);
            return;
        }

        // Play effects
        PlayTeleportEffects(player.transform.position, teleportPos);

        // Move player
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false; // Must disable CC before teleporting
            player.transform.position = teleportPos;
            cc.enabled = true;
        }
        else
        {
            player.transform.position = teleportPos;
        }

        Debug.Log("Player teleported successfully!", this);
    }

    bool IsPositionClear(Vector3 position)
    {
        // Check if there's enough space for the player
        return !Physics.CheckSphere(position, 0.5f, ~playerLayer);
    }

    void PlayTeleportEffects(Vector3 startPos, Vector3 endPos)
    {
        if (teleportEffectPrefab != null)
        {
            Instantiate(teleportEffectPrefab, startPos, Quaternion.identity);
            Instantiate(teleportEffectPrefab, endPos, Quaternion.identity);
        }

        if (teleportSound != null)
        {
            AudioSource.PlayClipAtPoint(teleportSound, transform.position, soundVolume);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw teleport destination marker
        Gizmos.color = Color.green;
        Vector3 destPos = transform.position + Vector3.up * teleportHeight;
        Gizmos.DrawWireSphere(destPos, 0.5f);
        Gizmos.DrawLine(transform.position, destPos);
    }
}