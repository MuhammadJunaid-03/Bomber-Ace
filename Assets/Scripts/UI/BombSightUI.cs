using UnityEngine;

public class BombSightUI : MonoBehaviour
{
    public Transform player;
    public float projectionRadius = 1f;

    private MeshRenderer meshRenderer;

    void Start()
    {
        // Create a flat circle indicator on the ground
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = new Vector3(projectionRadius * 2f, 0.05f, projectionRadius * 2f);

        meshRenderer = go.GetComponent<MeshRenderer>();
        var col = go.GetComponent<Collider>();
        if (col != null) Destroy(col);

        // Create transparent red material
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetFloat("_Surface", 1); // Transparent
        mat.SetFloat("_Blend", 0);
        mat.color = new Color(1f, 0f, 0f, 0.3f);
        meshRenderer.material = mat;

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb == null) return;

        float altitude = player.position.y;
        if (altitude <= 0.1f) { meshRenderer.enabled = false; return; }

        float fallTime = Mathf.Sqrt(2f * altitude / 9.81f);
        Vector3 velocity = rb.linearVelocity;
        Vector3 landingPos = player.position + new Vector3(velocity.x, 0, velocity.z) * fallTime;

        // Raycast down to find ground
        RaycastHit hit;
        if (Physics.Raycast(landingPos + Vector3.up * 50f, Vector3.down, out hit, 200f))
        {
            landingPos.y = hit.point.y + 0.1f;
        }
        else
        {
            landingPos.y = 0.1f;
        }

        transform.position = landingPos;
        meshRenderer.enabled = true;
    }
}
