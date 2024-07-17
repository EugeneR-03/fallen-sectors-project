using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public enum ProjectileVariant
{
    None,
    KineticProjectile1,
    KineticProjectile2,
    KineticProjectile3,
    KineticProjectile4,
    EnergeticProjectile1,
    EnergeticProjectile2,
    EnergeticProjectile3,
}

public class Projectile : ProjectileStats
{
    public ProjectileVariant Variant { get; set; }

    public Projectile()
        : base()
    {
        Variant = ProjectileVariant.None;
    }

    public Projectile(float kineticDamage, float energeticDamage, 
                    float moveSpeed, float turnSpeed, float lifeTime, 
                    int penetrationCount, float explosionRadius)
        : base(kineticDamage, energeticDamage, moveSpeed, turnSpeed, lifeTime, penetrationCount, explosionRadius)
    {
        Variant = ProjectileVariant.None;
    }
    
    public Projectile(ProjectileVariant variant)
    {
        Variant = variant;
        KineticDamage = Tables.ProjectileStats[variant].KineticDamage;
        EnergeticDamage = Tables.ProjectileStats[variant].EnergeticDamage;
        MoveSpeed = Tables.ProjectileStats[variant].MoveSpeed;
        TurnSpeed = Tables.ProjectileStats[variant].TurnSpeed;
        LifeTime = Tables.ProjectileStats[variant].LifeTime;
        PenetrationCount = Tables.ProjectileStats[variant].PenetrationCount;
        ExplosionRadius = Tables.ProjectileStats[variant].ExplosionRadius;
    }

    public Projectile(Projectile other)
    {
        Variant = other.Variant;
        KineticDamage = other.KineticDamage;
        EnergeticDamage = other.EnergeticDamage;
        MoveSpeed = other.MoveSpeed;
        TurnSpeed = other.TurnSpeed;
        LifeTime = other.LifeTime;
        PenetrationCount = other.PenetrationCount;
        ExplosionRadius = other.ExplosionRadius;
    }

    public Projectile Clone()
    {
        return new Projectile(this);
    }
}

internal class ProjectileStatsDeserializer
{
    public class Yaml
    {
        private static readonly IDeserializer s_deserializer =
            new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

        public static Dictionary<ProjectileVariant, Projectile> Deserialize(string yamlText)
        {
            var projectiles = s_deserializer.Deserialize<List<Projectile>>(yamlText);
            var stats = projectiles.ToDictionary(s => s.Variant, s => s);

            return stats;
        }
    }
}