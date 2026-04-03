using UnityEngine;

// Legacy HUD - replaced by HUDController which builds Canvas UI programmatically
// Keep this file to avoid missing script references but it does nothing
public class HUDManager : MonoBehaviour
{
    void Start()
    {
        // Disabled - HUDController handles all UI now
        enabled = false;
    }
}
