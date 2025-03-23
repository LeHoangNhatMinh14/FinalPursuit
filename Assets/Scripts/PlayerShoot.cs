using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Camera cam;
    public float damageBody = 34f;
    public float damageHead = 100f;
    public LayerMask enemyLayer; // Ensure this is set in Inspector

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
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100f, enemyLayer))
        {
            Enemy enemy = hit.collider.GetComponentInParent<Enemy>(); // Ensures it finds the main enemy script
            if (enemy != null)
            {
                if (hit.collider.CompareTag("Head"))
                {
                    enemy.TakeDamage(damageHead);
                    Debug.Log("Headshot! Enemy took " + damageHead + " damage.");
                }
                else
                {
                    enemy.TakeDamage(damageBody);
                    Debug.Log("Body shot! Enemy took " + damageBody + " damage.");
                }
            }
        }
        else
        {
            Debug.Log("Missed!");
            Debug.Log("Raycast hit: " + hit.collider.name + " | Layer: " + hit.collider.gameObject.layer);
        }
    }
}
