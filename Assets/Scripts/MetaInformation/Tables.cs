using System;
using System.Collections.Generic;
using UnityEngine;

public static class Tables
{
    // словарь следующего формата: tier, level, variant
    public static Dictionary<Tuple<int, int>, Dictionary<TemporaryUpgradeVariant, TemporaryUpgrade>> UpgradeStats { get; private set; }

    // словарь следующего формата: variant, level -> upgrade
    public static Dictionary<ConstantUpgradeVariant, Dictionary<int, ConstantUpgrade>> ConstantUpgradeStats { get; private set; }

    public static Dictionary<ShipVariant, Ship> ShipStats { get; private set; }
    
    public static Dictionary<WeaponVariant, Weapon> WeaponStats { get; private set; }

    public static Dictionary<ProjectileVariant, Projectile> ProjectileStats { get; private set; }

    public static Dictionary<EnemyVariant, Enemy> EnemyStats { get; private set; }

    static Tables()
    {
        var upgradeTableFile = Resources.Load<TextAsset>("Tables/Upgrades").ToString();
        UpgradeStats = TemporaryUpgradeStatsDeserializer.Yaml.Deserialize(upgradeTableFile);

        var constantUpgradeTableFile = Resources.Load<TextAsset>("Tables/ConstantUpgrades").ToString();
        ConstantUpgradeStats = ConstantUpgradeStatsDeserializer.Yaml.Deserialize(constantUpgradeTableFile);

        var shipTableFile = Resources.Load<TextAsset>("Tables/Ships").ToString();
        ShipStats = ShipStatsDeserializer.Yaml.Deserialize(shipTableFile);

        var weaponTableFile = Resources.Load<TextAsset>("Tables/Weapons").ToString();
        WeaponStats = WeaponStatsDeserializer.Yaml.Deserialize(weaponTableFile);

        var projectileTableFile = Resources.Load<TextAsset>("Tables/Projectiles").ToString();
        ProjectileStats = ProjectileStatsDeserializer.Yaml.Deserialize(projectileTableFile);

        var enemyTableFile = Resources.Load<TextAsset>("Tables/Enemies").ToString();
        EnemyStats = EnemyStatsDeserializer.Yaml.Deserialize(enemyTableFile);
    }
}