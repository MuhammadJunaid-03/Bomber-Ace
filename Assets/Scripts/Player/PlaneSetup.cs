using UnityEngine;

public class PlaneSetup : MonoBehaviour
{
    void Start()
    {
        ApplyUpgrades();
    }

    void ApplyUpgrades()
    {
        var planeData = UpgradeManager.Instance?.CurrentPlane;
        if (planeData == null) return;

        var controller = GetComponent<PlaneController>();
        if (controller != null)
        {
            controller.forwardSpeed = planeData.GetSpeed();
            controller.horizontalSpeed = 25f * planeData.GetManeuverability();
            controller.verticalSpeed = 15f * planeData.GetManeuverability();
        }

        var health = GetComponent<PlaneHealth>();
        if (health != null)
        {
            health.maxHealth = planeData.GetHealth();
            health.currentHealth = planeData.GetHealth();
            health.armor = planeData.GetArmor();
        }

        var bomber = GetComponent<BombDropper>();
        if (bomber != null)
        {
            bomber.bombDamage = planeData.GetBombDamage();
            bomber.maxBombs = planeData.GetBombCount();
        }

        var gun = GetComponent<PlaneGun>();
        if (gun != null)
        {
            gun.bulletDamage = planeData.GetGunDamage();
            gun.maxAmmo = planeData.GetAmmoCount();
        }
    }
}
