using UnityEngine;

public class EnemyHelicopter : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 15f;
    public float hoverHeight = 40f;
    public float turnSpeed = 2f;
    public float strafeSpeed = 8f;

    [Header("Combat")]
    public GameObject bulletPrefab;
    public GameObject missilePrefab;
    public Transform gunPoint;
    public Transform missilePoint;
    public float gunFireRate = 3f;
    public float missileFireRate = 0.15f;
    public float attackRange = 100f;
    public float bulletDamage = 6f;

    [Header("Visuals")]
    public Transform rotorBlade;
    public float rotorSpeed = 1500f;

    private Transform player;
    private float nextGunFire;
    private float nextMissileFire;
    private Rigidbody rb;
    private Vector3 strafeDir;
    private float strafeTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        strafeDir = Random.insideUnitSphere;
        strafeDir.y = 0;
        strafeDir.Normalize();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        // Rotate blade
        if (rotorBlade != null)
            rotorBlade.Rotate(Vector3.up, rotorSpeed * Time.deltaTime, Space.Self);

        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Face player
        Vector3 lookDir = (player.position - transform.position);
        lookDir.y = 0;
        if (lookDir.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }

        // Strafe periodically
        strafeTimer -= Time.deltaTime;
        if (strafeTimer <= 0f)
        {
            strafeDir = Quaternion.Euler(0, Random.Range(60f, 120f), 0) * strafeDir;
            strafeTimer = Random.Range(2f, 4f);
        }

        // Combat
        if (dist < attackRange)
        {
            if (Time.time >= nextGunFire)
            {
                FireGun();
                nextGunFire = Time.time + 1f / gunFireRate;
            }

            if (missilePrefab != null && Time.time >= nextMissileFire)
            {
                FireMissile();
                nextMissileFire = Time.time + 1f / missileFireRate;
            }
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Maintain hover height and strafe
        Vector3 targetPos = transform.position;
        targetPos.y = Mathf.Lerp(targetPos.y, hoverHeight, Time.fixedDeltaTime);

        float dist = Vector3.Distance(transform.position, player.position);
        Vector3 velocity = Vector3.zero;

        if (dist > attackRange)
        {
            velocity = (player.position - transform.position).normalized * speed;
        }
        else if (dist < attackRange * 0.3f)
        {
            velocity = (transform.position - player.position).normalized * speed;
        }
        else
        {
            velocity = strafeDir * strafeSpeed;
        }

        velocity.y = (targetPos.y - transform.position.y) * 3f;
        rb.linearVelocity = velocity;
    }

    void FireGun()
    {
        if (bulletPrefab == null || gunPoint == null || player == null) return;

        Vector3 dir = (player.position - gunPoint.position).normalized;
        dir += Random.insideUnitSphere * 0.1f;
        dir.Normalize();

        GameObject bullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.LookRotation(dir));
        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null)
        {
            b.damage = bulletDamage;
            b.speed = 70f;
            b.isPlayerBullet = false;
        }
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null) bulletRb.linearVelocity = dir * 70f;
    }

    void FireMissile()
    {
        if (missilePrefab == null || missilePoint == null) return;

        Vector3 dir = (player.position - missilePoint.position).normalized;
        Instantiate(missilePrefab, missilePoint.position, Quaternion.LookRotation(dir));
    }
}
