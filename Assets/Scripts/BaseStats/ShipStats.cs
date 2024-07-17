using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DamageResistance
{
    public float Block { get; set; }
    public float Reduction { get; set; }

    public DamageResistance()
    {
        Block = 0.0f;
        Reduction = 1.0f;
    }

    public DamageResistance(float block, float reduction)
    {
        Block = block;
        Reduction = reduction;
    }

    public DamageResistance(IEnumerable<float> values)
    {
        if (values.Count() != 2)
        {
            throw new System.ArgumentException();
        }

        Block = values.ElementAt(0);
        Reduction = values.ElementAt(1);
    }

    public static implicit operator DamageResistance((float block, float reduction) values)
    {
        return new DamageResistance(values.block, values.reduction);
    }

    public static implicit operator DamageResistance(float[] values)
    {
        return new DamageResistance(values);
    }
}

public abstract class ShipStats
{
    public DamageResistance KineticDamageResistance { get; set; }
    public DamageResistance EnergeticDamageResistance { get; set; }
    public float CurrentHealth { get; set; }
    public float MaxHealth { get; set; }
    public float MoveSpeed { get; set; }
    public float TurnSpeed { get; set; }

    public ShipStats()
    {
        KineticDamageResistance = new DamageResistance(0.0f, 0.0f);
        EnergeticDamageResistance = new DamageResistance(0.0f, 0.0f);
        CurrentHealth = 0.0f;
        MaxHealth = 0.0f;
        MoveSpeed = 0.0f;
        TurnSpeed = 0.0f;
    }

    public ShipStats(DamageResistance kineticDamageResistance, DamageResistance energeticDamageResistance, 
                float maxHealth, float moveSpeed, float turnSpeed)
    {
        KineticDamageResistance = kineticDamageResistance;
        EnergeticDamageResistance = energeticDamageResistance;
        CurrentHealth = maxHealth;
        MaxHealth = maxHealth;
        MoveSpeed = moveSpeed;
        TurnSpeed = turnSpeed;
    }
}