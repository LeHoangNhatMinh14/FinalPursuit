using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Camera cam;
    public float damageBody = 34f;
    public float damageHead = 100f;
    public LayerMask enemyLayer; // Assign "Enemy" layer in Inspector

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left Click to shoot
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, enemyLayer))
        {
            // Check if we hit a head or body collider (tag-based)
            if (hit.collider.CompareTag("Head") || hit.collider.CompareTag("Body"))
            {
                Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
                if (enemy != null)
                {
                    if (hit.collider.CompareTag("Head"))
                    {
                        enemy.TakeDamage(damageHead, "head");
                        Debug.Log("HEADSHOT! Damage: " + damageHead);
                    }
                    else if (hit.collider.CompareTag("Body"))
                    {
                        enemy.TakeDamage(damageBody, "body");
                        Debug.Log("Bodyshot! Damage: " + damageBody);
                    }
                }
            }
            else
            {
                Debug.Log("Hit a limb - no damage!");
            }
        }
        else
        {
            Debug.Log("Missed!");
        }
    }
}