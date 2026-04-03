using UnityEngine;

public class Bomb : MonoBehaviour
{
    [HideInInspector] public float damage = 50f;
    [HideInInspector] public float explosionRadius = 8f;

    public GameObject explosionEffect;
    public LayerMask damageableLayers;
    public float lifetime = 10f;

    private bool hasExploded;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
        rb.mass = 5f;
        rb.linearDamping = 0.1f;

        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;
        Explode();
    }

    void Explode()
    {
        hasExploded = true;

        // Spawn explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Deal area damage
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hitColliders)
        {
            // Damage ground targets
            GroundTarget target = hit.GetComponentInParent<GroundTarget>();
            if (target != null)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float falloff = 1f - (distance / explosionRadius);
                target.TakeDamage(damage * Mathf.Max(0.2f, falloff));
            }

            // Damage enemy planes
            EnemyHealth enemyHealth = hit.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // Apply force to rigidbodies
            Rigidbody hitRb = hit.GetComponent<Rigidbody>();
            if (hitRb != null)
            {
                hitRb.AddExplosionForce(damage * 20f, transform.position, explosionRadius, 2f);
            }
        }

        // Camera shake
        CameraShake.Instance?.Shake(0.3f, 0.5f);

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
