using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public float damage = 10f;
    [HideInInspector] public float speed = 100f;
    [HideInInspector] public float lifetime = 2f;
    [HideInInspector] public bool isPlayerBullet = true;

    public GameObject hitEffect;
    public TrailRenderer trail;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        if (rb.linearVelocity.sqrMagnitude < 1f)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.gameObject, collision.contacts[0].point);
    }

    void HandleHit(GameObject hitObject, Vector3 hitPoint)
    {
        if (hitEffect != null)
        {
            Instantiate(hitEffect, hitPoint, Quaternion.identity);
        }

        if (isPlayerBullet)
        {
            // Damage enemies
            GroundTarget target = hitObject.GetComponentInParent<GroundTarget>();
            if (target != null) target.TakeDamage(damage);

            EnemyHealth enemyHealth = hitObject.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null) enemyHealth.TakeDamage(damage);
        }
        else
        {
            // Enemy bullet hits player
            PlaneHealth playerHealth = hitObject.GetComponentInParent<PlaneHealth>();
            if (playerHealth != null) playerHealth.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
