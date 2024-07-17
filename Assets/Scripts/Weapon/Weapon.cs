using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public enum WeaponVariant
{
    None,
    MachineGun,
    HeavyMachineGun,
    LargeCaliberGun,
    RailGun,
    RocketCannon,
    LaserCannon,
    PlasmaCannon,
}

public static class WeaponVariantExtension
{
    public static string ToString(this WeaponVariant weaponVariant)
    {
        return weaponVariant switch
        {
            WeaponVariant.None => "None",
            WeaponVariant.MachineGun => "MachineGun",
            WeaponVariant.HeavyMachineGun => "HeavyMachineGun",
            WeaponVariant.LargeCaliberGun => "LargeCaliberGun",
            WeaponVariant.RailGun => "RailGun",
            WeaponVariant.RocketCannon => "RocketCannon",
            WeaponVariant.LaserCannon => "LaserCannon",
            WeaponVariant.PlasmaCannon => "PlasmaCannon",
            _ => "None",
        };
    }
}

public class Weapon
{
    public WeaponVariant Variant { get; set; }
    public float FireRate { get; set; }
    public int Cost { get; private set; }

    public Weapon()
    {
        Variant = WeaponVariant.None;
        FireRate = 0;
        Cost = 0;
    }

    public Weapon(float fireRate)
    {
        Variant = WeaponVariant.None;
        FireRate = fireRate;
        Cost = 0;
    }

    public Weapon(WeaponVariant variant, float fireRate)
    {
        Variant = variant;
        FireRate = fireRate;
        Cost = 0;
    }

    public Weapon(WeaponVariant variant)
    {
        if (variant == WeaponVariant.None)
        {
            return;
        }

        Variant = variant;
        FireRate = Tables.WeaponStats[variant].FireRate;
        Cost = Tables.WeaponStats[variant].Cost;
    }
}

internal class WeaponStatsDeserializer
{
    public class Yaml
    {
        private static readonly IDeserializer s_deserializer = 
            new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
        
        public static Dictionary<WeaponVariant, Weapon> Deserialize(string yamlText)
        {
            var weapons = s_deserializer.Deserialize<List<Weapon>>(yamlText);
            var stats = weapons.ToDictionary(s => s.Variant, s => s);

            return stats;
        }
    }
}