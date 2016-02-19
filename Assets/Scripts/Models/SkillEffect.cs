using UnityEngine;
using System.Collections;
using System;

public abstract class SkillEffect
{
    public abstract EffectType EffectType { get; }
    public bool IsApplied = false;

    protected Character target;
    public SkillEffect(Character target)
    {
        this.target = target;
    }

    public virtual void Apply()
    {
        IsApplied = true;
    }
}

public class DamageSkillEffect : SkillEffect
{
    public override EffectType EffectType { get { return EffectType.Damage; } }

    private int damage;
    public DamageSkillEffect(Character target, int damage) : base(target)
    {
        this.damage = damage;
    }

    public override void Apply()
    {
        if (IsApplied) return;
        base.Apply();

        target.ApplyHealthChange(-damage);
    }

    public override string ToString()
    {
        return damage + " damage to the " + target;
    }
}
