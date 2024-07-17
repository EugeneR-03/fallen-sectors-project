using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public enum ConstantUpgradeVariant
{
    None,
    HealthUpgrade,
    KineticDamageUpgrade,
    KineticDamageBlockUpgrade,
    EnergeticDamageUpgrade,
    EnergeticDamageBlockUpgrade,
    MoveSpeedUpgrade,
    TurnSpeedUpgrade,
    AttackSpeedUpgrade,
}

[Serializable]
public class ConstantUpgrade : IEquatable<ConstantUpgrade>
{
    public ConstantUpgradeVariant Variant { get; set; }
    public int Level { get; set; }
    public float Value { get; set; }
    public int Cost { get; set; }

    public ConstantUpgrade()
    {
        Variant = ConstantUpgradeVariant.None;
        Level = 0;
        Value = 0.0f;
        Cost = 0;
    }

    public ConstantUpgrade(int level, float value, int cost)
    {
        Level = level;
        Value = value;
        Cost = cost;
    }

    public ConstantUpgrade(ConstantUpgradeVariant variant, int level, float value, int cost)
    {
        Variant = variant;
        Level = level;
        Value = value;
        Cost = cost;
    }

    public ConstantUpgrade(ConstantUpgradeVariant variant, int level)
    {
        Variant = variant;
        Level = level;
        Value = Tables.ConstantUpgradeStats[Variant][level].Value;
        Cost = Tables.ConstantUpgradeStats[Variant][level].Cost;
    }

    public static int Count
    {
        get
        {
            int sum = 0;
            foreach (var kvp in Tables.UpgradeStats)
            {
                sum += kvp.Value.Count;
            }

            return sum;
        }
    }

    public bool Equals(ConstantUpgrade other)
    {
        if (other is null) return false;
        if (this is null) return true;
        return Variant == other.Variant && Level == other.Level;
    }

    public static ConstantUpgradeVariant GetUpgradeVariant(int variantNumber)
    {
        return variantNumber switch
        {
            0 => ConstantUpgradeVariant.None,
            1 => ConstantUpgradeVariant.HealthUpgrade,
            2 => ConstantUpgradeVariant.KineticDamageUpgrade,
            3 => ConstantUpgradeVariant.EnergeticDamageUpgrade,
            4 => ConstantUpgradeVariant.MoveSpeedUpgrade,
            5 => ConstantUpgradeVariant.TurnSpeedUpgrade,
            6 => ConstantUpgradeVariant.AttackSpeedUpgrade,
            _ => ConstantUpgradeVariant.None,
        };
    }
}

internal class ConstantUpgradeStatsDeserializer
{
    public class Yaml
    {
        private static readonly IDeserializer s_deserializer = 
            new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

        public static Dictionary<ConstantUpgradeVariant, Dictionary<int, ConstantUpgrade>> Deserialize(string yamlText)
        {
            var upgrades = s_deserializer.Deserialize<List<ConstantUpgrade>>(yamlText);
            var stats = new Dictionary<ConstantUpgradeVariant, Dictionary<int, ConstantUpgrade>>();

            foreach (var upgrade in upgrades)
            {
                if (!stats.ContainsKey(upgrade.Variant))
                {
                    stats[upgrade.Variant] = new Dictionary<int, ConstantUpgrade>();
                }
                stats[upgrade.Variant][upgrade.Level] = upgrade;
            }

            return stats;
        }
    }
}