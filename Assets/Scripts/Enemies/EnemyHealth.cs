using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 80f;
    public int scoreValue = 200;
    public GameObject explosionPrefab;
    public GameObject smokeTrailPrefab;

    private float currentHealth;
    private bool isDead;

    public event Action OnDeath;
    public float HealthPercentage => currentHealth / maxHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= maxHealth * 0.3f && smokeTrailPrefab != null)
        {
            var smoke = Instantiate(smokeTrailPrefab, transform);
            smoke.transform.localPosition = Vector3.zero;
        }

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        isDead = true;
        GameManager.Instance?.AddScore(scoreValue);

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        OnDeath?.Invoke();
        CameraShake.Instance?.Shake(0.15f, 0.2f);
        Destroy(gameObject, 0.1f);
    }
}
