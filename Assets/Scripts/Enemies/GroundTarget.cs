using UnityEngine;

[DefaultExecutionOrder(10)]
public class GroundTarget : MonoBehaviour
{
    public enum TargetType { Tank, Truck, AAGun, Radar, Building, Warship, MissileSilo, AircraftCarrier }

    [Header("Target Settings")]
    public TargetType targetType = TargetType.Tank;
    public float maxHealth = 100f;
    public int scoreValue = 100;

    [Header("Combat")]
    public bool canShoot = false;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float fireRange = 100f;
    public float accuracy = 0.3f;
    public float bulletDamage = 5f;
    public float bulletSpeed = 60f;

    [Header("Effects")]
    public GameObject destroyedVersion;
    public GameObject explosionPrefab;
    public GameObject smokeEffect;

    private float currentHealth;
    private float nextFireTime;
    private Transform playerTarget;
    private bool isDestroyed;

    public bool IsDestroyed => isDestroyed;

    void Start()
    {
        currentHealth = maxHealth;
        GameManager.Instance?.RegisterTarget();

        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTarget = player.transform;
    }

    void Update()
    {
        if (isDestroyed || !canShoot || playerTarget == null) return;
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        float distToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        if (distToPlayer <= fireRange && Time.time >= nextFireTime)
        {
            ShootAtPlayer();
            nextFireTime = Time.time + 1f / fireRate;
        }

        // Rotate turret toward player (for AA guns)
        if (canShoot && firePoint != null)
        {
            Vector3 lookDir = (playerTarget.position - firePoint.position).normalized;
            firePoint.rotation = Quaternion.Slerp(firePoint.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);
        }
    }

    void ShootAtPlayer()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Calculate lead and apply accuracy spread
        Vector3 targetPos = playerTarget.position;
        Rigidbody playerRb = playerTarget.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            float dist = Vector3.Distance(firePoint.position, targetPos);
            float travelTime = dist / bulletSpeed;
            targetPos += playerRb.linearVelocity * travelTime * accuracy;
        }

        // Add inaccuracy
        float spread = (1f - accuracy) * 10f;
        targetPos += Random.insideUnitSphere * spread;

        Vector3 direction = (targetPos - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));

        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null)
        {
            b.damage = bulletDamage;
            b.speed = bulletSpeed;
            b.isPlayerBullet = false;
            b.lifetime = 3f;
        }

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = direction * bulletSpeed;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;

        if (currentHealth <= maxHealth * 0.5f && smokeEffect != null)
            smokeEffect.SetActive(true);

        if (currentHealth <= 0f)
            DestroyTarget();
    }

    void DestroyTarget()
    {
        isDestroyed = true;

        GameManager.Instance?.TargetDestroyed(scoreValue);

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        if (destroyedVersion != null)
        {
            destroyedVersion.SetActive(true);
            // Hide original mesh
            var renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var r in renderers)
            {
                if (r.gameObject != destroyedVersion && !r.transform.IsChildOf(destroyedVersion.transform))
                    r.enabled = false;
            }
        }
        else
        {
            Destroy(gameObject, 0.5f);
        }

        CameraShake.Instance?.Shake(0.2f, 0.3f);
    }
}
