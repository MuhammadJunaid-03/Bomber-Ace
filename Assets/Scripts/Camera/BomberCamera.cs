using UnityEngine;

public class BomberCamera : MonoBehaviour
{
    [Header("Position")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 25f, -18f);
    public float followSmooth = 8f;

    [Header("Look")]
    public float lookDownAngle = 50f;
    public float lookSmooth = 5f;

    [Header("Dynamic")]
    public float horizontalLag = 0.3f;

    private Vector3 velocity;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null) cam.fieldOfView = 60f;

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Quaternion.Euler(lookDownAngle, 0f, 0f);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Follow position with slight horizontal lag
        Vector3 desiredPos = target.position + offset;

        // Lag horizontally for cinematic feel
        desiredPos.x = Mathf.Lerp(transform.position.x, desiredPos.x, Time.deltaTime * followSmooth * horizontalLag);
        desiredPos.y = Mathf.Lerp(transform.position.y, desiredPos.y, Time.deltaTime * followSmooth);
        desiredPos.z = Mathf.Lerp(transform.position.z, desiredPos.z, Time.deltaTime * followSmooth);

        transform.position = desiredPos;

        // Look down at a fixed angle (Bomber Ace style)
        Quaternion targetRot = Quaternion.Euler(lookDownAngle, 0f, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * lookSmooth);
    }
}
