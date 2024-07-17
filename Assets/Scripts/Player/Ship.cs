using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using UnityEngine;

public enum ShipVariant
{
    None,
    Fighter,
    Bomber,
    Frigate,
    BattleCruiser,
}

public static class ShipVariantExtension
{
    public static string ToString(this ShipVariant shipVariant)
    {
        return shipVariant switch
        {
            ShipVariant.Fighter => "Fighter",
            ShipVariant.Bomber => "Bomber",
            ShipVariant.Frigate => "Frigate",
            ShipVariant.BattleCruiser => "BattleCruiser",
            _ => "None",
        };
    }
}

public class Ship : ShipStats
{
    public ShipVariant Variant { get; set; }
    public float ExperienceGatheringRadius { get; set; }

    public Ship()
        : base()
    {
        Variant = ShipVariant.None;
    }

    public Ship(DamageResistance kineticDamageResistance, DamageResistance energeticDamageResistance, 
                float maxHealth, float moveSpeed, float turnSpeed, float experienceGatheringRadius)
        : base(kineticDamageResistance, energeticDamageResistance, maxHealth, moveSpeed, turnSpeed)
    {
        Variant = ShipVariant.None;
        ExperienceGatheringRadius = experienceGatheringRadius;
    }
    
    public Ship(ShipVariant variant)
    {
        Variant = variant;
        KineticDamageResistance = Tables.ShipStats[variant].KineticDamageResistance;
        EnergeticDamageResistance = Tables.ShipStats[variant].EnergeticDamageResistance;
        CurrentHealth = Tables.ShipStats[variant].MaxHealth;
        MaxHealth = Tables.ShipStats[variant].MaxHealth;
        MoveSpeed = Tables.ShipStats[variant].MoveSpeed;
        TurnSpeed = Tables.ShipStats[variant].TurnSpeed;
        ExperienceGatheringRadius = Tables.ShipStats[variant].ExperienceGatheringRadius;
    }
}

internal class ShipStatsDeserializer
{
    public class Yaml
    {
        private static readonly IDeserializer s_deserializer = 
            new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

        public static Dictionary<ShipVariant, Ship> Deserialize(string yamlText)
        {
            var ships = s_deserializer.Deserialize<List<Ship>>(yamlText);
            var stats = ships.ToDictionary(s => s.Variant, s => s);

            return stats;
        }
    }
}