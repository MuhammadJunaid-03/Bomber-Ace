using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float lifetime = 2f;
    public float maxScale = 5f;
    public float growSpeed = 8f;
    public Light explosionLight;
    public float lightIntensity = 5f;
    public float lightFadeSpeed = 3f;

    private float timer;

    void Start()
    {
        transform.localScale = Vector3.one * 0.1f;
        if (explosionLight != null)
            explosionLight.intensity = lightIntensity;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Grow
        float scale = Mathf.Lerp(0.1f, maxScale, 1f - Mathf.Exp(-growSpeed * timer));
        transform.localScale = Vector3.one * scale;

        // Fade light
        if (explosionLight != null)
        {
            explosionLight.intensity = Mathf.Lerp(lightIntensity, 0f, timer / lifetime);
        }
    }
}
