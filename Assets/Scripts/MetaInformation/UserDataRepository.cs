using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public static class UserDataRepository
{
    private static UserData _userData = new();

    private static UserData CreateNewData()
    {
        UserData newData = new()
        {
            CurrencyForConstantUpgrades = 0,
            ConstantUpgrades = new()
            {
                { ConstantUpgradeVariant.HealthUpgrade, new ConstantUpgrade(ConstantUpgradeVariant.HealthUpgrade, 0) },
                { ConstantUpgradeVariant.KineticDamageBlockUpgrade, new ConstantUpgrade(ConstantUpgradeVariant.KineticDamageBlockUpgrade, 0) },
                { ConstantUpgradeVariant.EnergeticDamageBlockUpgrade, new ConstantUpgrade(ConstantUpgradeVariant.EnergeticDamageBlockUpgrade, 0) },
                { ConstantUpgradeVariant.MoveSpeedUpgrade, new ConstantUpgrade(ConstantUpgradeVariant.MoveSpeedUpgrade, 0) },
                { ConstantUpgradeVariant.KineticDamageUpgrade, new ConstantUpgrade(ConstantUpgradeVariant.KineticDamageUpgrade, 0) },
                { ConstantUpgradeVariant.EnergeticDamageUpgrade, new ConstantUpgrade(ConstantUpgradeVariant.EnergeticDamageUpgrade, 0) },
                { ConstantUpgradeVariant.AttackSpeedUpgrade, new ConstantUpgrade(ConstantUpgradeVariant.AttackSpeedUpgrade, 0) },
                { ConstantUpgradeVariant.TurnSpeedUpgrade, new ConstantUpgrade(ConstantUpgradeVariant.TurnSpeedUpgrade, 0) },
            },
            WeaponsUnlocked = new()
            {
                { WeaponVariant.MachineGun, true },
                { WeaponVariant.HeavyMachineGun, true },
                { WeaponVariant.LargeCaliberGun, true },
                { WeaponVariant.RailGun, false },
                { WeaponVariant.RocketCannon, false },
                { WeaponVariant.LaserCannon, false },
                { WeaponVariant.PlasmaCannon, false },
            }
        };

        return newData;
    }

    private static UserData CreateNewDataWithAllProgress()
    {
        UserData newData = new()
        {
            CurrencyForConstantUpgrades = 100000,
            ConstantUpgrades = new()
            {
                { ConstantUpgradeVariant.HealthUpgrade, new ConstantUpgrade(ConstantUpgradeVariant.HealthUpgrade, 0) },
                { ConstantUpgradeVariant.KineticDamageBlockUpgrade, new ConstantUpgrade(ConstantUpgradeVariant.KineticDamageBlockUpgrade, 0) },
                { ConstantUpgradeVariant.EnergeticDamageBlockUpgrade, new ConstantUpgrade(ConstantUpgradeVariant.EnergeticDamageBlockUpgrade, 0) },
                { ConstantUpgradeVariant.MoveSpeedUpgrade, new ConstantUpgrade(ConstantUpgradeVariant.MoveSpeedUpgrade, 0) },
                { ConstantUpgradeVariant.KineticDamageUpgrade, new ConstantUpgrade(ConstantUpgradeVariant.KineticDamageUpgrade, 0) },
                { ConstantUpgradeVariant.EnergeticDamageUpgrade, new ConstantUpgrade(ConstantUpgradeVariant.EnergeticDamageUpgrade, 0) },
                { ConstantUpgradeVariant.AttackSpeedUpgrade, new ConstantUpgrade(ConstantUpgradeVariant.AttackSpeedUpgrade, 0) },
                { ConstantUpgradeVariant.TurnSpeedUpgrade, new ConstantUpgrade(ConstantUpgradeVariant.TurnSpeedUpgrade, 0) },
            },
            WeaponsUnlocked = new()
            {
                { WeaponVariant.MachineGun, true },
                { WeaponVariant.HeavyMachineGun, true },
                { WeaponVariant.LargeCaliberGun, true },
                { WeaponVariant.RailGun, true },
                { WeaponVariant.RocketCannon, true },
                { WeaponVariant.LaserCannon, true },
                { WeaponVariant.PlasmaCannon, true },
            }
        };

        return newData;
    }

    public static void LoadDataWithAllProgress()
    {
        string fileName = Application.persistentDataPath + "/Saves/PlayerInfoDev.dat";
        if (!File.Exists(fileName))
        {
            Debug.Log("File with game state not found; creating new");
            _userData = CreateNewDataWithAllProgress();
            Debug.Log("New data: " + _userData.CurrencyForConstantUpgrades + " " + _userData.ConstantUpgrades.Count);
            return;
        }

        BinaryFormatter bf = new();
        FileStream file = File.Open(fileName, FileMode.Open);
        UserData data = (UserData)bf.Deserialize(file);
        
        file.Close();
        Debug.Log("Game data loaded");

        _userData = data;
    }

    public static void LoadData()
    {
        string fileName = Application.persistentDataPath + "/Saves/PlayerInfo.dat";
        if (!File.Exists(fileName))
        {
            Debug.Log("File with game state not found; creating new");
            _userData = CreateNewData();
            Debug.Log("New data: " + _userData.CurrencyForConstantUpgrades + " " + _userData.ConstantUpgrades.Count);
            return;
        }

        BinaryFormatter bf = new();
        FileStream file = File.Open(fileName, FileMode.Open);
        UserData data = (UserData)bf.Deserialize(file);
        
        file.Close();
        Debug.Log("Game data loaded");

        _userData = data;
    }

    public static void SaveData()
    {
        string fileName = "PlayerInfo.dat";
        string fileDir = Application.persistentDataPath + "/Saves/";
        string filePath = fileDir + fileName;
        if (!Directory.Exists(fileDir))
        {
            Directory.CreateDirectory(fileDir);
        }
        FileStream file = File.Create(filePath);

        BinaryFormatter bf = new();
        bf.Serialize(file, _userData);
        file.Close();
        Debug.Log("Game data saved");
    }

    public static UserData GetData()
    {
        return _userData;
    }

    public static void SetData(UserData value)
    {
        _userData = value;
    }

    public static bool TryGetData(out UserData value)
    {
        if (_userData == null)
        {
            value = null;
            return false;
        }
        else
        {
            value = _userData;
            return true;
        }
    }
}