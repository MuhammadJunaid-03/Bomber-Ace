using UnityEngine;

public class EnemyPlane : MonoBehaviour
{
    [Header("Flight")]
    public float speed = 25f;
    public float turnSpeed = 2f;
    public float attackRange = 80f;
    public float disengageRange = 200f;

    [Header("Combat")]
    public GameObject bulletPrefab;
    public Transform[] firePoints;
    public float fireRate = 2f;
    public float bulletDamage = 8f;
    public float bulletSpeed = 80f;
    public float accuracy = 0.4f;

    [Header("Behavior")]
    public float patrolRadius = 100f;
    public float waypointThreshold = 15f;

    private enum State { Patrol, Chase, Attack, Disengage }
    private State currentState = State.Patrol;
    private Transform player;
    private Vector3 patrolCenter;
    private Vector3 currentWaypoint;
    private float nextFireTime;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        patrolCenter = transform.position;
        SetNewWaypoint();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        switch (currentState)
        {
            case State.Patrol: UpdatePatrol(); break;
            case State.Chase: UpdateChase(); break;
            case State.Attack: UpdateAttack(); break;
            case State.Disengage: UpdateDisengage(); break;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = transform.forward * speed;
    }

    void UpdatePatrol()
    {
        FlyToward(currentWaypoint);

        if (Vector3.Distance(transform.position, currentWaypoint) < waypointThreshold)
            SetNewWaypoint();

        if (player != null && Vector3.Distance(transform.position, player.position) < attackRange * 1.5f)
            currentState = State.Chase;
    }

    void UpdateChase()
    {
        if (player == null) { currentState = State.Patrol; return; }

        FlyToward(player.position);

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist < attackRange)
            currentState = State.Attack;
        else if (dist > disengageRange)
            currentState = State.Patrol;
    }

    void UpdateAttack()
    {
        if (player == null) { currentState = State.Patrol; return; }

        FlyToward(player.position);

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackRange * 1.2f)
        {
            currentState = State.Chase;
            return;
        }

        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }

        // Avoid crashing into player
        if (dist < 15f)
            currentState = State.Disengage;
    }

    void UpdateDisengage()
    {
        // Fly away then circle back
        Vector3 awayDir = (transform.position - (player != null ? player.position : patrolCenter)).normalized;
        Vector3 escapePoint = transform.position + awayDir * 80f + Vector3.up * 20f;
        FlyToward(escapePoint);

        if (player == null || Vector3.Distance(transform.position, player.position) > attackRange)
            currentState = State.Chase;
    }

    void FlyToward(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
    }

    void Fire()
    {
        if (bulletPrefab == null || firePoints == null || firePoints.Length == 0) return;
        if (player == null) return;

        foreach (var fp in firePoints)
        {
            Vector3 targetPos = player.position;

            // Lead the target
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                float dist = Vector3.Distance(fp.position, targetPos);
                float travelTime = dist / bulletSpeed;
                targetPos += playerRb.linearVelocity * travelTime * accuracy;
            }

            // Add spread
            float spread = (1f - accuracy) * 8f;
            targetPos += Random.insideUnitSphere * spread;

            Vector3 dir = (targetPos - fp.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, fp.position, Quaternion.LookRotation(dir));

            Bullet b = bullet.GetComponent<Bullet>();
            if (b != null)
            {
                b.damage = bulletDamage;
                b.speed = bulletSpeed;
                b.isPlayerBullet = false;
            }

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
                bulletRb.linearVelocity = dir * bulletSpeed;
        }
    }

    void SetNewWaypoint()
    {
        Vector3 randomOffset = Random.insideUnitSphere * patrolRadius;
        randomOffset.y = Mathf.Abs(randomOffset.y) * 0.3f + 30f;
        currentWaypoint = patrolCenter + randomOffset;
    }
}
