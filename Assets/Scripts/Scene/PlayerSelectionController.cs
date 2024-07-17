using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// [InitializeOnLoad]
public class PlayerSelectionController : MonoBehaviour
{
    private static int s_currentShipNumber;
    private static Dictionary<int, ShipVariant> s_ships;
    private static int s_activeWeaponNumber;
    private static Dictionary<int, WeaponVariant> s_weapons;

    static PlayerSelectionController()
    {
        Init();
    }

    private static void Init()
    {
        s_currentShipNumber = 1;
        s_activeWeaponNumber = 1;
        s_ships = new()
        {
            { 1, ShipVariant.Fighter },
            { 2, ShipVariant.Frigate },
        };
        s_weapons = new()
        {
            { 1, WeaponVariant.MachineGun },
            { 2, WeaponVariant.MachineGun },
            { 3, WeaponVariant.MachineGun },
            { 4, WeaponVariant.MachineGun },
            { 5, WeaponVariant.MachineGun },
            { 6, WeaponVariant.MachineGun },
        };
    }

    public static void SetShipNumber(int shipNumber)
    {
        if (shipNumber < 1 || shipNumber > 2)
        {
            return;
        }
        s_currentShipNumber = shipNumber;
    }

    public static void SetShipType(ShipVariant shipVariant)
    {
        s_ships[s_currentShipNumber] = shipVariant;
    }

    public static void SetShipType(string shipName)
    {
        switch (shipName)
        {
            case "Fighter":
                s_ships[s_currentShipNumber] = ShipVariant.Fighter;
                break;
            case "Frigate":
                s_ships[s_currentShipNumber] = ShipVariant.Frigate;
                break;
        }
    }

    public static ShipVariant GetShipType()
    {
        return s_ships[s_currentShipNumber];
    }

    public static void SetActiveWeaponNumber(int weaponNumber)
    {
        if (weaponNumber < 1 || weaponNumber > 6)
        {
            return;
        }
        s_activeWeaponNumber = weaponNumber;
        
    }

    public static int GetActiveWeaponNumber()
    {
        return s_activeWeaponNumber;
    }

    public static void SetActiveWeaponVariant(WeaponVariant weaponVariant)
    {
        s_weapons[s_activeWeaponNumber] = weaponVariant;
    }

    public static void SetActiveWeaponVariant(string weaponName)
    {
        UserData userData = UserDataController.Instance.GetUserData();
        WeaponVariant weaponVariant = WeaponVariant.None;

        switch (weaponName)
        {
            case "MachineGun":
                weaponVariant = WeaponVariant.MachineGun;
                break;
            case "HeavyMachineGun":
                weaponVariant = WeaponVariant.HeavyMachineGun;
                break;
            case "LargeCaliberGun":
                weaponVariant = WeaponVariant.LargeCaliberGun;
                break;
            case "RailGun":
                weaponVariant = WeaponVariant.RailGun;
                break;
            case "RocketCannon":
                weaponVariant = WeaponVariant.RocketCannon;
                break;
            case "LaserCannon":
                weaponVariant = WeaponVariant.LaserCannon;
                break;
            case "PlasmaCannon":
                weaponVariant = WeaponVariant.PlasmaCannon;
                break;
        }

        if (!userData.WeaponsUnlocked[weaponVariant])
        {
            return;
        }

        s_weapons[s_activeWeaponNumber] = weaponVariant;
    }

    public static void SetWeaponVariant(int weaponNumber, WeaponVariant weaponVariant)
    {
        if (weaponNumber < 1 || weaponNumber > 6)
        {
            return;
        }
        s_weapons[weaponNumber] = weaponVariant;
    }

    public static WeaponVariant GetWeaponVariant(int weaponNumber)
    {
        if (weaponNumber < 1 || weaponNumber > 6)
        {
            return WeaponVariant.None;
        }
        return s_weapons[weaponNumber];
    }

    public static WeaponVariant GetActiveWeaponVariant()
    {
        return s_weapons[s_activeWeaponNumber];
    }

    public static void Reset()
    {
        Init();
    }
}