using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private float shakeDuration;
    private float shakeMagnitude;
    private Vector3 originalLocalPos;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (shakeDuration > 0f)
        {
            transform.localPosition = originalLocalPos + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime;

            if (shakeDuration <= 0f)
            {
                transform.localPosition = originalLocalPos;
            }
        }
    }

    public void Shake(float duration, float magnitude)
    {
        if (shakeDuration <= 0f)
            originalLocalPos = transform.localPosition;

        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
