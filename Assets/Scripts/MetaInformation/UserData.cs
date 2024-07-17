using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

[Serializable]
public class UserData : IEquatable<UserData>
{
    public int CurrencyForConstantUpgrades { get; set; }
    public Dictionary<ConstantUpgradeVariant, ConstantUpgrade> ConstantUpgrades { get; set; }
    public Dictionary<WeaponVariant, bool> WeaponsUnlocked { get; set; }

    public UserData()
    {
        CurrencyForConstantUpgrades = 0;
        ConstantUpgrades = new Dictionary<ConstantUpgradeVariant, ConstantUpgrade>();
    }

    public bool Equals(UserData other)
    {
        return other != null
            && CurrencyForConstantUpgrades == other.CurrencyForConstantUpgrades
            && ConstantUpgrades.SequenceEqual(other.ConstantUpgrades)
            && WeaponsUnlocked.SequenceEqual(other.WeaponsUnlocked);
    }
}