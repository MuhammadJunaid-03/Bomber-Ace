using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    public int Coins { get; private set; }
    public int Gems { get; private set; }

    public event Action<int> OnCoinsChanged;
    public event Action<int> OnGemsChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Instance = this;
        }
        else
        {
            Instance = this;
        }
        LoadCurrency();
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        OnCoinsChanged?.Invoke(Coins);
        SaveCurrency();
    }

    public void AddGems(int amount)
    {
        Gems += amount;
        OnGemsChanged?.Invoke(Gems);
        SaveCurrency();
    }

    public bool SpendCoins(int amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            OnCoinsChanged?.Invoke(Coins);
            SaveCurrency();
            return true;
        }
        return false;
    }

    public bool SpendGems(int amount)
    {
        if (Gems >= amount)
        {
            Gems -= amount;
            OnGemsChanged?.Invoke(Gems);
            SaveCurrency();
            return true;
        }
        return false;
    }

    void SaveCurrency()
    {
        PlayerPrefs.SetInt("Coins", Coins);
        PlayerPrefs.SetInt("Gems", Gems);
        PlayerPrefs.Save();
    }

    void LoadCurrency()
    {
        Coins = PlayerPrefs.GetInt("Coins", 500);
        Gems = PlayerPrefs.GetInt("Gems", 10);
    }
}
