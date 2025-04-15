using UnityEngine;
using System.Collections;

public class StunGrenade : MonoBehaviour
{
    [Header("Settings")]
    public float delay = 2f;
    public float stunRadius = 8f;
    public float stunDuration = 3f;
    
    [Header("Optional Effects")]
    public Light flashLight;
    public AudioClip bangSound;
    public ParticleSystem explosionParticles;

    private float spawnTime;
    private bool hasDetonated;

    void Start()
    {
        spawnTime = Time.time;
        Debug.Log($"[StunGrenade] Spawned at {transform.position} | Detonation in {delay}s");

        if (flashLight != null)
        {
            flashLight.enabled = false;
            Debug.Log($"[StunGrenade] Flash light initialized");
        }

        Rigidbody rb = GetComponentInChildren<Rigidbody>();
        if (rb != null)
        {
            Vector3 force = transform.forward * 10f;
            Debug.Log($"[StunGrenade] Applying initial force: {force}");
            rb.AddForce(force, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError("[StunGrenade] No Rigidbody found in children!");
        }

        Invoke("Detonate", delay);
    }

    void Detonate()
    {
        if (hasDetonated) return;
        hasDetonated = true;

        Debug.Log($"[StunGrenade] Detonating after {Time.time - spawnTime:F2}s | Position: {transform.position}");
        Debug.Log($"[StunGrenade] Stun radius: {stunRadius} | Duration: {stunDuration}s");

        // Effects
        StartCoroutine(FlashEffect());
        
        if (bangSound != null)
        {
            Debug.Log("[StunGrenade] Playing bang sound");
            AudioSource.PlayClipAtPoint(bangSound, transform.position);
        }

        if (explosionParticles != null)
        {
            Debug.Log("[StunGrenade] Playing explosion particles");
            explosionParticles.Play();
        }

        // Stun detection
        Collider[] colliders = Physics.OverlapSphere(transform.position, stunRadius);
        Debug.Log($"[StunGrenade] Found {colliders.Length} objects in radius");

        int enemiesStunned = 0;
        foreach (Collider col in colliders)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log($"[StunGrenade] Stunning enemy: {col.name}");
                enemy.Stun(stunDuration);
                enemiesStunned++;
            }
        }

        Debug.Log($"[StunGrenade] Stunned {enemiesStunned} enemies");

        // Cleanup
        float destroyDelay = explosionParticles != null ? 2f : 0f;
        Debug.Log($"[StunGrenade] Destroying in {destroyDelay}s");
        Destroy(gameObject, destroyDelay);
    }

    IEnumerator FlashEffect()
    {
        if (flashLight == null)
        {
            Debug.Log("[StunGrenade] No flash light to activate");
            yield break;
        }

        Debug.Log("[StunGrenade] Starting flash effect");
        flashLight.enabled = true;
        float elapsed = 0f;
        float flashIntensity = flashLight.intensity;
        
        while (elapsed < 0.2f)
        {
            if (flashLight != null)
            {
                flashLight.intensity = Mathf.Lerp(flashIntensity, 0, elapsed / 0.2f);
                elapsed += Time.deltaTime;
            }
            else
            {
                Debug.LogWarning("[StunGrenade] Flash light destroyed during effect");
                yield break;
            }
            yield return null;
        }
        
        if (flashLight != null)
        {
            flashLight.enabled = false;
            Debug.Log("[StunGrenade] Flash effect completed");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, stunRadius);
    }
}