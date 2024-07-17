using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class ProjectileStats
{
    public float KineticDamage { get; set; }
    public float EnergeticDamage { get; set; }
    public float MoveSpeed { get; set; }
    public float TurnSpeed { get; set; }
    public float LifeTime { get; set; }
    public int PenetrationCount { get; set; }
    public float ExplosionRadius { get; set; }

    public ProjectileStats()
    {
        KineticDamage = 0.0f;
        EnergeticDamage = 0.0f;
        MoveSpeed = 0.0f;
        TurnSpeed = 0.0f;
        LifeTime = 0.0f;
        PenetrationCount = 0;
        ExplosionRadius = 0.0f;
    }

    public ProjectileStats(float kineticDamage, float energeticDamage, 
                            float moveSpeed, float turnSpeed, float lifeTime, 
                            int penetrationCount, float explosionRadius)
    {
        KineticDamage = kineticDamage;
        EnergeticDamage = energeticDamage;
        MoveSpeed = moveSpeed;
        TurnSpeed = turnSpeed;
        LifeTime = lifeTime;
        PenetrationCount = penetrationCount;
        ExplosionRadius = explosionRadius;
    }
}