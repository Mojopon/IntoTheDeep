using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

// result represent the entire combat
public class CharacterCombatResult
{
    public Character user;
    public Skill usedSkill;
    private List<Performance> performances = new List<Performance>();

    public CharacterCombatResult(Character user, Skill usedSkill)
    {
        this.user = user;
        this.usedSkill = usedSkill;
    }

    public void AddPerformance(Performance performance)
    {
        performances.Add(performance);
    }

    public Performance[] GetPerformances()
    {
        return performances.ToArray();
    }

    public void Apply()
    {
        foreach(var performance in performances)
        {
            performance.Apply();
        }
    }
}

public class Performance
{
    public Character target;
    public Skill receivedSkill;
    public SkillEffect skillEffect;

    public Performance(Character target, Skill receivedSkill, SkillEffect skillEffect)
    {
        this.target = target;
        this.receivedSkill = receivedSkill;
        this.skillEffect = skillEffect;
    }

    public void Apply()
    {
        skillEffect.Apply();
    }
}

public static class Combat
{
    public static CharacterCombatResult GetCombatResult(Character user, Skill skill, Func<Coord, Character> characterOnTheLocation, int randomSeed)
    {
        var combatResult = new CharacterCombatResult(user, skill);

        var skillRanges = skill.range.Select(x => x + new Coord(user.X, user.Y));
        foreach(var range in skillRanges)
        {
            var target = characterOnTheLocation(range);
            if (target == null)
            {
                continue;
            }

            var performance = PerformSkill(user, target, skill);
            combatResult.AddPerformance(performance);
        }

        return combatResult;
    }


    private static Performance PerformSkill(Character user, Character target, Skill skill)
    {
        var magnitude = CalculateMagnitude(user, target, skill);

        SkillEffect effect = null;
        switch(skill.effectType)
        {
            case EffectType.Damage:
                effect = new DamageSkillEffect(target, magnitude);
                break;
        }

        return new Performance(target, skill, effect);
    }

    public static int CalculateMagnitude(ICharacterAttributes user, ICharacterAttributes target, Skill skill)
    {
        int power = 0;
        float reduction = 0;

        switch(skill.effectAttribute)
        {
            case EffectAttribute.MeleePower:
                {
                    power = user.meleePower;
                    reduction = 0.1f;
                }
                break;
            case EffectAttribute.RangePower:
                {
                    power = user.rangePower;
                    reduction = 0.1f;
                }
                break;
            case EffectAttribute.SpellPower:
                {
                    power = user.spellPower;
                }
                break;
        }

        var powerRange = UnityEngine.Random.Range(skill.minMultiply, skill.maxMultiply);
        float actualPower = power * powerRange;
        float reducedPower = actualPower - (actualPower * reduction);
        int magnitude = Mathf.RoundToInt(reducedPower);

        return magnitude;
    }
}
