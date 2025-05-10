using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 3f;
    public float damage = 10f;
    public GameObject hitEffect;
    public LayerMask hitLayers;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & hitLayers) != 0)
        {
            if (other.TryGetComponent<PlayerHealth>(out PlayerHealth player))
            {
                player.TakeDamage(damage);
            }

            if (hitEffect)
            {
                Instantiate(hitEffect, transform.position, Quaternion.LookRotation(-transform.forward));
            }

            Destroy(gameObject);
        }
    }
}
