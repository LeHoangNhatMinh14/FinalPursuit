using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    public float health = 100f;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float moveRange = 5f;

    private Vector3 randomTargetPosition;
    private Vector3 startingPosition;
    private bool isDead = false;

    void Start()
    {
        startingPosition = transform.position;
        StartCoroutine(MoveRandomly());
    }

    // Updated to accept hit location
    public void TakeDamage(float damage, string hitLocation)
    {
        if (isDead) return;

        switch (hitLocation.ToLower())
        {
            case "head":
                health -= damage;
                Debug.Log("HEADSHOT! Remaining HP: " + health);
                break;
            case "body":
                health -= damage;
                Debug.Log("Bodyshot! Remaining HP: " + health);
                break;
            // Limbs/other parts ignored (handled in PlayerShoot)
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        StopCoroutine(MoveRandomly());
        StartCoroutine(RespawnAfterDelay(3f));
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(delay);

        health = 100f;
        isDead = false;
        transform.position = startingPosition;

        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
        StartCoroutine(MoveRandomly());
    }

    private IEnumerator MoveRandomly()
    {
        while (!isDead)
        {
            randomTargetPosition = startingPosition + new Vector3(
                Random.Range(-moveRange, moveRange),
                0,
                Random.Range(-moveRange, moveRange)
            );

            while (Vector3.Distance(transform.position, randomTargetPosition) > 0.2f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    randomTargetPosition,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            yield return new WaitForSeconds(1f);
        }
    }
}