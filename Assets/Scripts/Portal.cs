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

            var enemyCounter = EnemyCounter.Instance;
            if (enemyCounter != null)
            {
                enemyCounter.ShowPerksAfterPortal();
            }

            gameObject.SetActive(false); // Optional: disable portal after use
        }
    }
}
