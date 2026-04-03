using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameInitializer : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartMission();
        }
    }
}
