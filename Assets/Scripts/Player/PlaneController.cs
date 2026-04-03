using UnityEngine;
using UnityEngine.InputSystem;

public class PlaneController : MonoBehaviour
{
    [Header("Auto Flight")]
    public float forwardSpeed = 40f;
    public float speedBoostMax = 60f;

    [Header("Steering")]
    public float horizontalSpeed = 25f;
    public float verticalSpeed = 15f;
    public float smoothTime = 0.15f;

    [Header("Boundaries")]
    public float maxX = 60f;
    public float minY = 15f;
    public float maxY = 80f;

    [Header("Banking")]
    public float bankAngle = 35f;
    public float bankSmooth = 5f;

    [Header("Pitch Visual")]
    public float pitchAngle = 15f;

    [Header("References")]
    public Transform planeModel;

    private Vector2 inputDir;
    private Vector2 smoothVelocity;
    private Vector2 currentMoveSpeed;
    private float currentForwardSpeed;
    private Rigidbody rb;
    private bool isControllable = true;

    public float CurrentSpeed => currentForwardSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        currentForwardSpeed = forwardSpeed;
    }

    void Update()
    {
        if (!isControllable) return;
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        ReadInput();
    }

    void FixedUpdate()
    {
        if (!isControllable) return;
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        MovePlayer();
        UpdateVisuals();
    }

    void ReadInput()
    {
        float h = 0f, v = 0f;

        // Keyboard
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) h = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h = 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) v = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) v = -1f;
        }

        // Touch - drag anywhere on screen
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 delta = Touchscreen.current.primaryTouch.delta.ReadValue();
            h = Mathf.Clamp(delta.x * 0.02f, -1f, 1f);
            v = Mathf.Clamp(delta.y * 0.02f, -1f, 1f);
        }

        // Mouse drag (for testing in editor)
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            h = Mathf.Clamp(delta.x * 0.03f, -1f, 1f);
            v = Mathf.Clamp(delta.y * 0.03f, -1f, 1f);
        }

        inputDir = new Vector2(h, v);
    }

    void MovePlayer()
    {
        // Smooth the input
        Vector2 targetSpeed = new Vector2(
            inputDir.x * horizontalSpeed,
            inputDir.y * verticalSpeed
        );
        currentMoveSpeed = Vector2.SmoothDamp(currentMoveSpeed, targetSpeed, ref smoothVelocity, smoothTime);

        // Always fly forward
        currentForwardSpeed = forwardSpeed;

        // Calculate new position
        Vector3 pos = rb.position;
        pos.x += currentMoveSpeed.x * Time.fixedDeltaTime;
        pos.y += currentMoveSpeed.y * Time.fixedDeltaTime;
        pos.z += currentForwardSpeed * Time.fixedDeltaTime;

        // Clamp boundaries
        pos.x = Mathf.Clamp(pos.x, -maxX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        rb.MovePosition(pos);
    }

    void UpdateVisuals()
    {
        if (planeModel == null) return;

        // Bank when turning
        float targetBank = -inputDir.x * bankAngle;
        float targetPitch = -inputDir.y * pitchAngle;

        Vector3 currentEuler = planeModel.localEulerAngles;
        float currentBank = currentEuler.z > 180f ? currentEuler.z - 360f : currentEuler.z;
        float currentPitch = currentEuler.x > 180f ? currentEuler.x - 360f : currentEuler.x;

        float smoothBank = Mathf.Lerp(currentBank, targetBank, Time.fixedDeltaTime * bankSmooth);
        float smoothPitch = Mathf.Lerp(currentPitch, targetPitch, Time.fixedDeltaTime * bankSmooth);

        planeModel.localEulerAngles = new Vector3(smoothPitch, 0f, smoothBank);
    }

    public void SetControllable(bool controllable)
    {
        isControllable = controllable;
    }
}
