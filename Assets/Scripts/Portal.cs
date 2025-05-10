using UnityEngine;

public class Portal : MonoBehaviour
{
    private bool hasTriggeredPerkUI = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggeredPerkUI)
        {
            hasTriggeredPerkUI = true;

            Debug.Log("[Portal] Player entered. Showing perk UI...");

            var enemyCounter = FindObjectOfType<EnemyCounter>();
            if (enemyCounter != null)
            {
                enemyCounter.ShowPerksAfterPortal();
            }
            else
            {
                Debug.LogWarning("No EnemyCounter found in scene!");
            }

            gameObject.SetActive(false); // Optional: disable portal after use
        }
    }
}
