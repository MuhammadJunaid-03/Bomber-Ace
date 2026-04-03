using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public float speed = 25f;
    public float turnSpeed = 3f;
    public float damage = 40f;
    public float lifetime = 8f;
    public float armingDistance = 20f;
    public GameObject explosionEffect;
    public TrailRenderer trail;

    private Transform target;
    private Rigidbody rb;
    private bool isArmed;
    private Vector3 launchPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        launchPos = transform.position;

        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) target = player.transform;

        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (!isArmed)
        {
            float dist = Vector3.Distance(transform.position, launchPos);
            if (dist > armingDistance) isArmed = true;
        }

        if (target != null && isArmed)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction, turnSpeed * Time.fixedDeltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }

        rb.linearVelocity = transform.forward * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isArmed) return;

        PlaneHealth playerHealth = collision.gameObject.GetComponentInParent<PlaneHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        CameraShake.Instance?.Shake(0.2f, 0.3f);
        Destroy(gameObject);
    }
}
