using UnityEngine;
using UnityEngine.InputSystem;

public class PlaneGun : MonoBehaviour
{
    [Header("Gun Settings")]
    public GameObject bulletPrefab;
    public Transform[] firePoints;
    public float fireRate = 10f;
    public float bulletSpeed = 100f;
    public float bulletDamage = 10f;
    public float bulletLifetime = 2f;

    [Header("Ammo")]
    public int maxAmmo = 200;
    public float reloadTime = 3f;

    private int currentAmmo;
    private float nextFireTime;
    private bool isReloading;
    private int currentFirePoint;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;
    public bool IsReloading => isReloading;

    void Start()
    {
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        bool fireInput = false;

        // F key to shoot
        if (Keyboard.current != null && Keyboard.current.fKey.isPressed)
            fireInput = true;

        if (fireInput && !isReloading)
            Fire();

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            StartCoroutine(Reload());
    }

    void Fire()
    {
        if (currentAmmo <= 0 || Time.time < nextFireTime) return;
        if (bulletPrefab == null || firePoints == null || firePoints.Length == 0) return;

        nextFireTime = Time.time + 1f / fireRate;
        currentAmmo--;

        Transform fp = firePoints[currentFirePoint % firePoints.Length];
        currentFirePoint++;

        GameObject bullet = Instantiate(bulletPrefab, fp.position, fp.rotation);
        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null)
        {
            b.damage = bulletDamage;
            b.speed = bulletSpeed;
            b.lifetime = bulletLifetime;
            b.isPlayerBullet = true;
        }

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            PlaneController pc = GetComponent<PlaneController>();
            float fwdSpeed = pc != null ? pc.CurrentSpeed : 0f;
            bulletRb.linearVelocity = fp.forward * bulletSpeed + Vector3.forward * fwdSpeed;
        }

        if (currentAmmo <= 0)
            StartCoroutine(Reload());
    }

    System.Collections.IEnumerator Reload()
    {
        if (isReloading) yield break;
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
