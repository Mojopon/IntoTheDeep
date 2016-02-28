using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class Effect
{
    public Coord[] ranges;

    public Effect() { }

    public abstract List<CombatLog> Apply(Character user, Func<Coord, Character> characterOnTheLocation);
}

public class DamageEffect : Effect
{
    public EffectAttribute effectAttribute = EffectAttribute.MeleePower;
    public float minMultiply = 1f;
    public float maxMultiply = 1f;

    public DamageEffect() { }

    public DamageEffect(Coord[] ranges, EffectAttribute effectAttribute, float minMultiply, float maxMultiply)
    {
        this.effectAttribute = effectAttribute;
        this.minMultiply = minMultiply;
        this.maxMultiply = maxMultiply;
    }

    public override List<CombatLog> Apply(Character user, Func<Coord, Character> characterOnTheLocation)
    {
        var logs = new List<CombatLog>();

        var userLocation = user.Location.Value;

        foreach(var range in ranges)
        {
            var targetLocation = userLocation + range;
            var target = characterOnTheLocation(targetLocation);

            if (target != null) logs.Add(AffectTarget(user, target));
        }

        return logs;
    }

    public CombatLog AffectTarget(Character user, Character target)
    {
        var magnitude = Combat.CalculateMagnitude(user, target, effectAttribute, minMultiply, maxMultiply);

        target.ApplyHealthChange(-magnitude);

        return new CombatLog()
        {
            target = target,
            amount = magnitude,
            combatType = CombatLog.CombatType.Damage
        };
    }
}


