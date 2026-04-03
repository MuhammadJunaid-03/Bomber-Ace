using UnityEngine;
public class PlaneHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float armor = 0f; // Damage reduction percentage (0-0.8)

    [Header("Effects")]
    public GameObject smokeEffect;
    public GameObject fireEffect;
    public GameObject explosionPrefab;

    public event System.Action<float, float> OnHealthChanged; // current, max
    public event System.Action OnDeath;

    public float HealthPercentage => currentHealth / maxHealth;
    public bool IsAlive => currentHealth > 0f;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;

        float actualDamage = damage * (1f - Mathf.Clamp(armor, 0f, 0.8f));
        currentHealth = Mathf.Max(0f, currentHealth - actualDamage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Visual feedback
        if (HealthPercentage < 0.5f && smokeEffect != null)
            smokeEffect.SetActive(true);
        if (HealthPercentage < 0.25f && fireEffect != null)
            fireEffect.SetActive(true);

        if (currentHealth <= 0f)
            Die();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (HealthPercentage >= 0.5f && smokeEffect != null)
            smokeEffect.SetActive(false);
        if (HealthPercentage >= 0.25f && fireEffect != null)
            fireEffect.SetActive(false);
    }

    void Die()
    {
        OnDeath?.Invoke();

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        GameManager.Instance?.PlayerDied();

        // Disable plane controls and let it fall
        var controller = GetComponent<PlaneController>();
        if (controller != null) controller.SetControllable(false);

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddTorque(Random.insideUnitSphere * 50f);
        }

        Destroy(gameObject, 3f);
    }
}
