using UnityEngine;

public class PlayerGrenadeThrow : MonoBehaviour
{
    public GameObject stunGrenadePrefab;
    public Transform throwPoint;
    public float throwForce = 20f;
    public float throwUpwardForce = 5f;
    public Transform playerCameraRoot;
    public float cooldown = 3f;
    
    private float lastThrowTime;
    private int grenadeCount = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (Time.time > lastThrowTime + cooldown)
            {
                Debug.Log($"[GrenadeThrow] G key pressed. Throwing grenade #{++grenadeCount}");
                ThrowGrenade();
                lastThrowTime = Time.time;
            }
            else
            {
                Debug.Log($"[GrenadeThrow] Cooldown active. Remaining: {lastThrowTime + cooldown - Time.time:F1}s");
            }
        }
    }

    void ThrowGrenade()
    {
        if (stunGrenadePrefab == null)
        {
            Debug.LogError("[GrenadeThrow] stunGrenadePrefab not assigned!");
            return;
        }

        if (throwPoint == null)
        {
            Debug.LogError("[GrenadeThrow] throwPoint not assigned!");
            return;
        }

        if (playerCameraRoot == null)
        {
            Debug.LogError("[GrenadeThrow] playerCameraRoot not assigned!");
            return;
        }

        Debug.Log($"[GrenadeThrow] Instantiating grenade at {throwPoint.position}");
        GameObject grenade = Instantiate(stunGrenadePrefab, throwPoint.position, Quaternion.identity);
        grenade.name = $"Grenade_{grenadeCount}";

        Rigidbody rb = grenade.GetComponentInChildren<Rigidbody>();
        if (rb != null)
        {
            // Use forward direction of the camera root
            Vector3 throwDirection = playerCameraRoot.forward;

            // Optionally add upward arc
            Vector3 force = throwDirection * throwForce + Vector3.up * throwUpwardForce;

            Debug.Log($"[GrenadeThrow] Applying force: {force}");
            rb.AddForce(force, ForceMode.Impulse);

            // Optional: make grenade face forward
            grenade.transform.rotation = Quaternion.LookRotation(throwDirection);
        }
        else
        {
            Debug.LogError("[GrenadeThrow] No Rigidbody found in grenade hierarchy");
        }
    }
}