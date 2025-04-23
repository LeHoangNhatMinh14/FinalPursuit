using UnityEngine;

public class ClimbablePillar : MonoBehaviour
{
    [Header("Climbing Settings")]
    public float interactionRange = 2f; // How close player needs to be
    public float climbHeightOffset = 1f; // How much above the pillar to teleport
    public KeyCode climbKey = KeyCode.E; // Key to press for climbing
    
    [Header("References")]
    public GameObject player; // Drag your player here
    public GameObject climbPrompt; // Optional UI prompt (set active/inactive)
    
    private bool playerInRange = false;

    void Update()
    {
        // Check if player is in range
        float distance = Vector3.Distance(transform.position, player.transform.position);
        playerInRange = distance <= interactionRange;
        
        // Show/hide climb prompt if available
        if (climbPrompt != null)
        {
            climbPrompt.SetActive(playerInRange);
        }
        
        // Check for climb input
        if (playerInRange && Input.GetKeyDown(climbKey))
        {
            ClimbPillar();
        }
    }
    
    void ClimbPillar()
    {
        // Calculate top position of the pillar
        Vector3 pillarTop = transform.position + Vector3.up * (GetComponent<Collider>().bounds.extents.y + climbHeightOffset);
        
        // Teleport player to the top
        player.transform.position = pillarTop;
        
        // Optional: Play sound effect
        // AudioManager.Instance.Play("ClimbSound");
        
        Debug.Log("Player climbed the pillar!");
    }
    
    // Visualize interaction range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}