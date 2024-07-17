using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public enum TemporaryUpgradeColor
{
    None,
    White,
    Blue,
    Violet,
    Orange,
}

public enum TemporaryUpgradeVariant
{
    None,
    Unique,
    HealthUpgrade,
    KineticDamageUpgrade,
    KineticDamageBlockUpgrade,
    KineticDamageReductionUpgrade,
    EnergeticDamageUpgrade,
    EnergeticDamageBlockUpgrade,
    EnergeticDamageReductionUpgrade,
    MoveSpeedUpgrade,
    TurnSpeedUpgrade,
    ExperienceGatheringRadiusUpgrade,
    AttackSpeedUpgrade,
    AttackRangeUpgrade,
    PenetrationCountUpgrade,
    ExplosionRadiusUpgrade,
}

public class TemporaryUpgrade
{
    public TemporaryUpgradeVariant Variant { get; set; }
    public int Tier { get; set; }
    public int Level { get; set; }
    public float Value { get; set; }

    public TemporaryUpgrade()
    {
        Variant = TemporaryUpgradeVariant.None;
        Tier = 0;
        Level = 0;
        Value = 0.0f;
    }

    public TemporaryUpgrade(int tier, int level, float value)
    {
        Tier = tier;
        Level = level;
        Value = value;
    }

    public TemporaryUpgrade(TemporaryUpgradeVariant variant, int tier, int level, float value)
    {
        Variant = variant;
        Tier = tier;
        Level = level;
        Value = value;
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

    public static TemporaryUpgradeVariant GetUpgradeVariant(int variantNumber)
    {
        return variantNumber switch
        {
            0 => TemporaryUpgradeVariant.None,
            1 => TemporaryUpgradeVariant.Unique,
            2 => TemporaryUpgradeVariant.HealthUpgrade,
            3 => TemporaryUpgradeVariant.KineticDamageUpgrade,
            4 => TemporaryUpgradeVariant.KineticDamageBlockUpgrade,
            5 => TemporaryUpgradeVariant.KineticDamageReductionUpgrade,
            6 => TemporaryUpgradeVariant.EnergeticDamageUpgrade,
            7 => TemporaryUpgradeVariant.EnergeticDamageBlockUpgrade,
            8 => TemporaryUpgradeVariant.EnergeticDamageReductionUpgrade,
            9 => TemporaryUpgradeVariant.MoveSpeedUpgrade,
            10 => TemporaryUpgradeVariant.TurnSpeedUpgrade,
            11 => TemporaryUpgradeVariant.AttackSpeedUpgrade,
            12 => TemporaryUpgradeVariant.AttackRangeUpgrade,
            13 => TemporaryUpgradeVariant.PenetrationCountUpgrade,
            14 => TemporaryUpgradeVariant.ExplosionRadiusUpgrade,
            _ => TemporaryUpgradeVariant.None,
        };
    }

    public static TemporaryUpgradeColor GetUpgradeColor(int level)
    {
        return level switch
        {
            1 => TemporaryUpgradeColor.White,
            2 => TemporaryUpgradeColor.Blue,
            3 => TemporaryUpgradeColor.Violet,
            4 => TemporaryUpgradeColor.Orange,
            _ => TemporaryUpgradeColor.None,
        };
    }

    public static string GetUpgradeName(TemporaryUpgradeVariant variant)
    {
        return variant switch
        {
            TemporaryUpgradeVariant.Unique => "Уникальное улучшение",
            TemporaryUpgradeVariant.HealthUpgrade => "Улучшение здоровья",
            TemporaryUpgradeVariant.KineticDamageUpgrade => "Улучшение кинетического урона",
            TemporaryUpgradeVariant.KineticDamageBlockUpgrade => "Улучшение блока кинетического урона",
            TemporaryUpgradeVariant.KineticDamageReductionUpgrade => "Улучшение сопротивления кинетическому урону",
            TemporaryUpgradeVariant.EnergeticDamageUpgrade => "Улучшение энергетического урона",
            TemporaryUpgradeVariant.EnergeticDamageBlockUpgrade => "Улучшение блока энергетического урона",
            TemporaryUpgradeVariant.EnergeticDamageReductionUpgrade => "Улучшение сопротивления энергетическому урону",
            TemporaryUpgradeVariant.MoveSpeedUpgrade => "Улучшение скорости передвижения",
            TemporaryUpgradeVariant.TurnSpeedUpgrade => "Улучшение скорости поворота",
            TemporaryUpgradeVariant.ExperienceGatheringRadiusUpgrade => "Улучшение радиуса сбора опыта",
            TemporaryUpgradeVariant.AttackSpeedUpgrade => "Улучшение скорости атаки",
            TemporaryUpgradeVariant.AttackRangeUpgrade => "Улучшение дальности атаки",
            TemporaryUpgradeVariant.PenetrationCountUpgrade => "Улучшение количества попаданий",
            TemporaryUpgradeVariant.ExplosionRadiusUpgrade => "Улучшение радиуса взрыва",
            _ => "Неизвестное улучшение",
        };
    }

    public static string GetUpgradeDescription(TemporaryUpgradeVariant variant)
    {
        return variant switch
        {
            TemporaryUpgradeVariant.Unique => "Уникальное улучшение",
            TemporaryUpgradeVariant.HealthUpgrade => "Увеличивает максимальный запас здоровья",
            TemporaryUpgradeVariant.KineticDamageUpgrade => "Увеличивает наносимый кинетический урон",
            TemporaryUpgradeVariant.KineticDamageBlockUpgrade => "Увеличивает показатель блока кинетического урона",
            TemporaryUpgradeVariant.KineticDamageReductionUpgrade => "Увеличивает показатель сопротивления кинетическому урону",
            TemporaryUpgradeVariant.EnergeticDamageUpgrade => "Увеличивает наносимый энергетический урон",
            TemporaryUpgradeVariant.EnergeticDamageBlockUpgrade => "Увеличивает показатель блока энергетического урона",
            TemporaryUpgradeVariant.EnergeticDamageReductionUpgrade => "Увеличивает показатель сопротивления энергетическому урону",
            TemporaryUpgradeVariant.MoveSpeedUpgrade => "Увеличивает скорость передвижения",
            TemporaryUpgradeVariant.TurnSpeedUpgrade => "Увеличивает скорость поворота",
            TemporaryUpgradeVariant.ExperienceGatheringRadiusUpgrade => "Увеличивает радиус сбора опыта",
            TemporaryUpgradeVariant.AttackSpeedUpgrade => "Увеличивает скорость атаки",
            TemporaryUpgradeVariant.AttackRangeUpgrade => "Увеличивает дальность атаки",
            TemporaryUpgradeVariant.PenetrationCountUpgrade => "Увеличивает пробивную способность снарядов (только для кинетического оружия)",
            TemporaryUpgradeVariant.ExplosionRadiusUpgrade => "Увеличивает радиус взрыва снарядов (только для энергетического оружия)",
            _ => "Улучшение",
        };
    }

    public static TemporaryUpgrade GetUpgrade()
    {
        System.Random rnd = new();

        int tier = 0;
        int tierProbability = rnd.Next(0, 20);
        // 0, 1, 2, 3, 5, 6, 7, 8, 9, 10
        if (tierProbability >= 0 && tierProbability <= 10)
        {
            tier = 1;
        }
        // 11, 12, 13, 14, 15
        else if (tierProbability >= 11 && tierProbability <= 15)
        {
            tier = 2;
        }
        // 16, 17, 18, 19
        else if (tierProbability >= 16 && tierProbability <= 19)
        {
            tier = 3;
        }
        // none
        else
        {
            tier = 4;
        }

        int level = 0;
        int levelProbability = rnd.Next(0, 20);
        // 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10
        if (levelProbability >= 0 && levelProbability <= 10)
        {
            level = 1;
        }
        // 11, 12, 13, 14, 15
        else if (levelProbability >= 11 && levelProbability <= 15)
        {
            level = 2;
        }
        // 16, 17, 18
        else if (levelProbability >= 16 && levelProbability <= 18)
        {
            level = 3;
        }
        // 19
        else
        {
            level = 4;
        }

        var upgrades = Tables.UpgradeStats[new Tuple<int, int>(tier, level)];
        int upgradeVariantProbability = rnd.Next(0, upgrades.Count);
        TemporaryUpgrade upgrade = upgrades.ElementAt(upgradeVariantProbability).Value;
        return upgrade;
    }

    public static TemporaryUpgrade GetUpgradeWithoutUpgradesForEnergeticWeapons()
    {
        TemporaryUpgrade upgrade = GetUpgrade();
        while (upgrade.Variant == TemporaryUpgradeVariant.EnergeticDamageUpgrade
            || upgrade.Variant == TemporaryUpgradeVariant.ExplosionRadiusUpgrade)
        {
            upgrade = GetUpgrade();
        }

        return upgrade;
    }

    public static TemporaryUpgrade GetUpgradeWithoutUpgradesForKineticWeapons()
    {
        TemporaryUpgrade upgrade = GetUpgrade();
        while (upgrade.Variant == TemporaryUpgradeVariant.KineticDamageUpgrade
            || upgrade.Variant == TemporaryUpgradeVariant.PenetrationCountUpgrade)
        {
            upgrade = GetUpgrade();
        }

        return upgrade;
    } 
}

internal class TemporaryUpgradeStatsDeserializer
{
    public class Yaml
    {
        private static readonly IDeserializer s_deserializer = 
            new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

        public static Dictionary<Tuple<int, int>, Dictionary<TemporaryUpgradeVariant, TemporaryUpgrade>> Deserialize(string yamlText)
        {
            // получаем список всех улучшений
            var upgrades = s_deserializer.Deserialize<List<TemporaryUpgrade>>(yamlText);

            // создаём представление с группировкой по тиру и уровню
            var stats = new Dictionary<Tuple<int, int>, Dictionary<TemporaryUpgradeVariant, TemporaryUpgrade>>();

            // заполняем представление
            foreach (var upgrade in upgrades)
            {
                var upgradeKey = new Tuple<int, int>(upgrade.Tier, upgrade.Level);
                if (!stats.ContainsKey(upgradeKey))
                {
                    stats[upgradeKey] = new Dictionary<TemporaryUpgradeVariant, TemporaryUpgrade>();
                }
                stats[upgradeKey][upgrade.Variant] = upgrade;
            }

            return stats;
        }
    }
}