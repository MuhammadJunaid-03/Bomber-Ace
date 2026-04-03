using UnityEngine;

[CreateAssetMenu(fileName = "NewPlane", menuName = "BomberAce/Plane Data")]
public class PlaneUpgradeData : ScriptableObject
{
    public string planeName = "P-51 Mustang";
    public string description = "A versatile fighter-bomber";
    public Sprite planeIcon;

    [Header("Unlock")]
    public int unlockCost = 0;
    public int unlockGemCost = 0;
    public bool isUnlockedByDefault = false;

    [Header("Base Stats")]
    public float baseSpeed = 30f;
    public float baseHealth = 100f;
    public float baseArmor = 0f;
    public float baseBombDamage = 50f;
    public float baseGunDamage = 10f;
    public int baseBombCount = 20;
    public int baseAmmoCount = 200;

    [Header("Upgrade Levels (0-5)")]
    public int engineLevel = 0;
    public int armorLevel = 0;
    public int wingLevel = 0;
    public int bombLevel = 0;
    public int gunLevel = 0;

    [Header("Upgrade Costs Per Level")]
    public int[] engineUpgradeCosts = { 100, 250, 500, 1000, 2000 };
    public int[] armorUpgradeCosts = { 100, 250, 500, 1000, 2000 };
    public int[] wingUpgradeCosts = { 80, 200, 400, 800, 1600 };
    public int[] bombUpgradeCosts = { 150, 300, 600, 1200, 2500 };
    public int[] gunUpgradeCosts = { 120, 280, 550, 1100, 2200 };

    public float GetSpeed()
    {
        return baseSpeed + engineLevel * 5f;
    }

    public float GetHealth()
    {
        return baseHealth + armorLevel * 25f;
    }

    public float GetArmor()
    {
        return baseArmor + armorLevel * 0.1f;
    }

    public float GetBombDamage()
    {
        return baseBombDamage + bombLevel * 12f;
    }

    public float GetGunDamage()
    {
        return baseGunDamage + gunLevel * 3f;
    }

    public float GetManeuverability()
    {
        return 1f + wingLevel * 0.15f;
    }

    public int GetBombCount()
    {
        return baseBombCount + bombLevel * 4;
    }

    public int GetAmmoCount()
    {
        return baseAmmoCount + gunLevel * 40;
    }
}
