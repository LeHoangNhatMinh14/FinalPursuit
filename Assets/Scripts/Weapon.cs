using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Damage")]
    public float bodyDamage = 30f;
    public float headDamage = 100f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
            {
                bool isHeadshot = hit.collider.CompareTag("Head");
                float damage = isHeadshot ? headDamage : bodyDamage;
                
                Enemy enemy = hit.transform.GetComponentInParent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage, isHeadshot);
                }
                else
                {
                    Debug.Log("Hit " + hit.collider.name + " (not an enemy)");
                }
            }
        }
    }

    // Visual debug
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * 100f);
    }
}