using UnityEngine;
using UnityEngine.InputSystem;

public class BombDropper : MonoBehaviour
{
    [Header("Bomb Settings")]
    public GameObject bombPrefab;
    public Transform bombDropPoint;
    public float bombCooldown = 0.3f;
    public int maxBombs = 20;
    public float bombDamage = 50f;
    public float bombRadius = 10f;

    private int currentBombs;
    private float lastBombTime;

    public int CurrentBombs => currentBombs;
    public int MaxBombs => maxBombs;

    void Start()
    {
        currentBombs = maxBombs;
        if (bombDropPoint == null)
            bombDropPoint = transform;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        bool dropInput = false;

        // Space bar
        if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed)
            dropInput = true;

        // Right mouse button
        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
            dropInput = true;

        // Touch - second finger
        if (Touchscreen.current != null)
        {
            var touches = Touchscreen.current.touches;
            for (int i = 0; i < touches.Count; i++)
            {
                if (touches[i].press.isPressed && i >= 1)
                {
                    dropInput = true;
                    break;
                }
            }
        }

        if (dropInput) DropBomb();
    }

    public void DropBomb()
    {
        if (currentBombs <= 0 || Time.time - lastBombTime < bombCooldown) return;
        if (bombPrefab == null) return;

        lastBombTime = Time.time;
        currentBombs--;

        GameObject bomb = Instantiate(bombPrefab, bombDropPoint.position, Quaternion.identity);
        Bomb bombScript = bomb.GetComponent<Bomb>();
        if (bombScript != null)
        {
            bombScript.damage = bombDamage;
            bombScript.explosionRadius = bombRadius;
        }

        // Inherit forward velocity
        Rigidbody bombRb = bomb.GetComponent<Rigidbody>();
        if (bombRb != null)
        {
            PlaneController pc = GetComponent<PlaneController>();
            if (pc != null)
                bombRb.linearVelocity = Vector3.forward * pc.CurrentSpeed;
        }

        if (currentBombs <= 0)
        {
            Invoke(nameof(ReloadBombs), 4f);
        }
    }

    void ReloadBombs()
    {
        currentBombs = maxBombs;
    }
}
