using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    public float health = 100f;
    private Renderer enemyRenderer;

    void Start()
    {
        enemyRenderer = GetComponentInChildren<Renderer>();
        Debug.Log("Enemy ready (HP: " + health + ")");
    }

    public void TakeDamage(float damage, bool isHeadshot)
    {
        if (health <= 0) return;

        string hitType = isHeadshot ? "HEADSHOT" : "bodyshot";
        health -= damage;
        
        Debug.Log(hitType + "! " + damage + " damage | Remaining HP: " + health);
        
        if (health <= 0) Die();
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}