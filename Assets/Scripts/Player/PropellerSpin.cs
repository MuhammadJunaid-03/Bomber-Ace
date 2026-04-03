using UnityEngine;

public class PropellerSpin : MonoBehaviour
{
    public float spinSpeed = 2000f;

    void Update()
    {
        transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime, Space.Self);
    }
}
