using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public enum EnemyVariant
{
    None,
    Ordinary,
    Fast,
    Armored,
    Boss,
}

public static class EnemyVariantExtension
{
    public static string ToString(this EnemyVariant enemyVariant)
    {
        return enemyVariant switch
        {
            EnemyVariant.Ordinary => "Ordinary",
            EnemyVariant.Fast => "Fast",
            EnemyVariant.Armored => "Armored",
            EnemyVariant.Boss => "Boss",
            _ => "None",
        };
    }
}

public class Enemy : ShipStats
{
    public float KineticDamage { get; set; }
    public float EnergeticDamage { get; set; }
    public float ExperienceAmount { get; set; }
    public float CurrencyAmount { get; set; }
    public EnemyVariant Variant { get; set; }

    public Enemy()
        : base()
    {
        Variant = EnemyVariant.None;
        KineticDamage = 0;
        EnergeticDamage = 0;
        ExperienceAmount = 0;
        CurrencyAmount = 0;
    }

    public Enemy(DamageResistance kineticDamageResistance, DamageResistance energeticDamageResistance, 
                float kineticDamage, float energeticDamage, float maxHealth, 
                float moveSpeed, float turnSpeed, float experienceAmount, float currencyCount)
        : base(kineticDamageResistance, energeticDamageResistance, maxHealth, moveSpeed, turnSpeed)
    {
        Variant = EnemyVariant.None;
        KineticDamage = kineticDamage;
        EnergeticDamage = energeticDamage;
        ExperienceAmount = experienceAmount;
        CurrencyAmount = currencyCount;
    }
    
    public Enemy(EnemyVariant variant)
    {
        Variant = variant;
        KineticDamageResistance = Tables.EnemyStats[variant].KineticDamageResistance;
        EnergeticDamageResistance = Tables.EnemyStats[variant].EnergeticDamageResistance;
        KineticDamage = Tables.EnemyStats[variant].KineticDamage;
        EnergeticDamage = Tables.EnemyStats[variant].EnergeticDamage;
        CurrentHealth = Tables.EnemyStats[variant].MaxHealth;
        MaxHealth = Tables.EnemyStats[variant].MaxHealth;
        MoveSpeed = Tables.EnemyStats[variant].MoveSpeed;
        TurnSpeed = Tables.EnemyStats[variant].TurnSpeed;
        ExperienceAmount = Tables.EnemyStats[variant].ExperienceAmount;
        CurrencyAmount = Tables.EnemyStats[variant].CurrencyAmount;
    }
}

internal class EnemyStatsDeserializer
{
    public class Yaml
    {
        private static readonly IDeserializer s_deserializer = 
            new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

        public static Dictionary<EnemyVariant, Enemy> Deserialize(string yamlText)
        {
            var enemies = s_deserializer.Deserialize<List<Enemy>>(yamlText);
            var stats = enemies.ToDictionary(s => s.Variant, s => s);

            return stats;
        }
    }
}