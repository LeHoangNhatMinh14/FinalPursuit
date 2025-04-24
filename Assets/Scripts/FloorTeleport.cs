using UnityEngine;

public class FloorTeleport : MonoBehaviour
{
    [Header("Teleport Settings")]
    [Tooltip("The transform the player will be teleported to.")]
    public Transform teleportDestination;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();

            // Temporarily disable CharacterController to prevent teleport issues
            if (controller != null)
                controller.enabled = false;

            other.transform.position = teleportDestination.position;

            if (controller != null)
                controller.enabled = true;

            Debug.Log("[Portal] Player teleported to " + teleportDestination.name);
        }
    }
}
