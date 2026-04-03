using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public Image damageOverlay;
    public float flashDuration = 0.3f;
    public Color damageColor = new Color(1f, 0f, 0f, 0.4f);

    private float flashTimer;
    private PlaneHealth playerHealth;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlaneHealth>();
            if (playerHealth != null)
                playerHealth.OnHealthChanged += OnHealthChanged;
        }

        if (damageOverlay != null)
        {
            damageOverlay.color = Color.clear;
        }
    }

    void OnHealthChanged(float current, float max)
    {
        flashTimer = flashDuration;

        // Persistent red tint at low health
        if (damageOverlay != null)
        {
            float healthPct = current / max;
            if (healthPct < 0.3f)
            {
                Color persistentColor = damageColor;
                persistentColor.a = (1f - healthPct / 0.3f) * 0.3f;
                damageOverlay.color = persistentColor;
            }
        }
    }

    void Update()
    {
        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            if (damageOverlay != null)
            {
                Color c = damageColor;
                c.a = (flashTimer / flashDuration) * damageColor.a;

                float healthPct = playerHealth != null ? playerHealth.HealthPercentage : 1f;
                if (healthPct < 0.3f)
                {
                    float persistentAlpha = (1f - healthPct / 0.3f) * 0.3f;
                    c.a = Mathf.Max(c.a, persistentAlpha);
                }

                damageOverlay.color = c;
            }
        }
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= OnHealthChanged;
    }
}
