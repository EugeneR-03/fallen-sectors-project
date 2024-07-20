using System;
using UnityEngine;

public class UserDataController : MonoBehaviour
{
    private static UserData _userData;
    public static UserDataController Instance { get; private set; }
    public event Action OnDataChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetUserData(UserData userData)
    {
        if (userData == null || userData.Equals(_userData))
        {
            return;
        }

        _userData = userData;
        OnDataChanged?.Invoke();
    }

    public UserData GetUserData()
    {
        return _userData;
    }

    public void SetUserDataCurrencyForConstantUpgrades(int value)
    {
        if (value == _userData.CurrencyForConstantUpgrades)
        {
            return;
        }

        _userData.CurrencyForConstantUpgrades = value;
        OnDataChanged?.Invoke();
    }

    public void AddCurrencyForConstantUpgrades(int value)
    {
        _userData.CurrencyForConstantUpgrades += value;
        OnDataChanged?.Invoke();
    }

    public void SetUserDataSpecificConstantUpgrade(ConstantUpgrade constantUpgrade)
    {
        if (_userData.ConstantUpgrades[constantUpgrade.Variant].Equals(constantUpgrade))
        {
            return;
        }

        _userData.ConstantUpgrades[constantUpgrade.Variant] = constantUpgrade;
        OnDataChanged?.Invoke();
    }

    private void BuyWeapon(WeaponVariant weaponVariant)
    {
        _userData.CurrencyForConstantUpgrades -= Tables.WeaponStats[weaponVariant].Cost;
        _userData.WeaponsUnlocked[weaponVariant] = true;
        OnDataChanged?.Invoke();
    }

    public void TryBuyWeapon(WeaponVariant weaponVariant)
    {
        if (_userData.WeaponsUnlocked[weaponVariant] || 
            Tables.WeaponStats[weaponVariant].Cost > _userData.CurrencyForConstantUpgrades)
        {
            return;
        }

        BuyWeapon(weaponVariant);
    }
}