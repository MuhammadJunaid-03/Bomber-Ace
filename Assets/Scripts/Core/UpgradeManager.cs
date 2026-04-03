using UnityEngine;
using System;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    public PlaneUpgradeData[] availablePlanes;
    public int currentPlaneIndex = 0;

    public event Action OnUpgradeChanged;

    public PlaneUpgradeData CurrentPlane =>
        availablePlanes != null && currentPlaneIndex < availablePlanes.Length
            ? availablePlanes[currentPlaneIndex]
            : null;

    void Awake()
    {
        Instance = this;
        LoadUpgrades();
    }

    public bool UpgradeEngine()
    {
        var plane = CurrentPlane;
        if (plane == null || plane.engineLevel >= 5) return false;

        int cost = plane.engineUpgradeCosts[plane.engineLevel];
        if (CurrencyManager.Instance.SpendCoins(cost))
        {
            plane.engineLevel++;
            SaveUpgrades();
            OnUpgradeChanged?.Invoke();
            return true;
        }
        return false;
    }

    public bool UpgradeArmor()
    {
        var plane = CurrentPlane;
        if (plane == null || plane.armorLevel >= 5) return false;

        int cost = plane.armorUpgradeCosts[plane.armorLevel];
        if (CurrencyManager.Instance.SpendCoins(cost))
        {
            plane.armorLevel++;
            SaveUpgrades();
            OnUpgradeChanged?.Invoke();
            return true;
        }
        return false;
    }

    public bool UpgradeWings()
    {
        var plane = CurrentPlane;
        if (plane == null || plane.wingLevel >= 5) return false;

        int cost = plane.wingUpgradeCosts[plane.wingLevel];
        if (CurrencyManager.Instance.SpendCoins(cost))
        {
            plane.wingLevel++;
            SaveUpgrades();
            OnUpgradeChanged?.Invoke();
            return true;
        }
        return false;
    }

    public bool UpgradeBombs()
    {
        var plane = CurrentPlane;
        if (plane == null || plane.bombLevel >= 5) return false;

        int cost = plane.bombUpgradeCosts[plane.bombLevel];
        if (CurrencyManager.Instance.SpendCoins(cost))
        {
            plane.bombLevel++;
            SaveUpgrades();
            OnUpgradeChanged?.Invoke();
            return true;
        }
        return false;
    }

    public bool UpgradeGuns()
    {
        var plane = CurrentPlane;
        if (plane == null || plane.gunLevel >= 5) return false;

        int cost = plane.gunUpgradeCosts[plane.gunLevel];
        if (CurrencyManager.Instance.SpendCoins(cost))
        {
            plane.gunLevel++;
            SaveUpgrades();
            OnUpgradeChanged?.Invoke();
            return true;
        }
        return false;
    }

    public bool UnlockPlane(int index)
    {
        if (index < 0 || index >= availablePlanes.Length) return false;
        var plane = availablePlanes[index];
        if (plane.isUnlockedByDefault) return false;

        bool canAfford = true;
        if (plane.unlockCost > 0 && CurrencyManager.Instance.Coins < plane.unlockCost) canAfford = false;
        if (plane.unlockGemCost > 0 && CurrencyManager.Instance.Gems < plane.unlockGemCost) canAfford = false;

        if (canAfford)
        {
            if (plane.unlockCost > 0) CurrencyManager.Instance.SpendCoins(plane.unlockCost);
            if (plane.unlockGemCost > 0) CurrencyManager.Instance.SpendGems(plane.unlockGemCost);
            plane.isUnlockedByDefault = true;
            SaveUpgrades();
            OnUpgradeChanged?.Invoke();
            return true;
        }
        return false;
    }

    public void SelectPlane(int index)
    {
        if (index >= 0 && index < availablePlanes.Length)
        {
            currentPlaneIndex = index;
            PlayerPrefs.SetInt("SelectedPlane", currentPlaneIndex);
            OnUpgradeChanged?.Invoke();
        }
    }

    void SaveUpgrades()
    {
        for (int i = 0; i < availablePlanes.Length; i++)
        {
            var p = availablePlanes[i];
            string key = $"Plane_{i}_";
            PlayerPrefs.SetInt(key + "Engine", p.engineLevel);
            PlayerPrefs.SetInt(key + "Armor", p.armorLevel);
            PlayerPrefs.SetInt(key + "Wing", p.wingLevel);
            PlayerPrefs.SetInt(key + "Bomb", p.bombLevel);
            PlayerPrefs.SetInt(key + "Gun", p.gunLevel);
            PlayerPrefs.SetInt(key + "Unlocked", p.isUnlockedByDefault ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    void LoadUpgrades()
    {
        currentPlaneIndex = PlayerPrefs.GetInt("SelectedPlane", 0);

        for (int i = 0; i < availablePlanes.Length; i++)
        {
            var p = availablePlanes[i];
            string key = $"Plane_{i}_";
            p.engineLevel = PlayerPrefs.GetInt(key + "Engine", 0);
            p.armorLevel = PlayerPrefs.GetInt(key + "Armor", 0);
            p.wingLevel = PlayerPrefs.GetInt(key + "Wing", 0);
            p.bombLevel = PlayerPrefs.GetInt(key + "Bomb", 0);
            p.gunLevel = PlayerPrefs.GetInt(key + "Gun", 0);

            if (!p.isUnlockedByDefault)
                p.isUnlockedByDefault = PlayerPrefs.GetInt(key + "Unlocked", 0) == 1;
        }
    }
}
