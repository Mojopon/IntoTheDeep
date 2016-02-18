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

    }
}

public class Performance
{
    public Character target;
    public Attributes changes;
    public Skill receivedSkill;

    public Performance(Character target, Attributes changes, Skill receivedSkill)
    {
        this.target = target;
        this.changes = changes;
        this.receivedSkill = receivedSkill;
    }
}

public static class Combat
{
    public static CharacterCombatResult GetCombatResult(Character user, Skill skill, Func<Coord, Character> characterOnTheLocation, int randomSeed)
    {
        var combatResult = new CharacterCombatResult(user, skill);

        var skillRanges = skill.range.Select(x => x + user.Location.Value);
        foreach(var range in skillRanges)
        {
            var target = characterOnTheLocation(range);
            if (target == null) continue;

            var performance = PerformSkill(user, target, skill);
            combatResult.AddPerformance(performance);
        }

        return combatResult;
    }

    private static Performance PerformSkill(Character user, Character target, Skill skill)
    {
        Attributes attributeChanges = null;
        switch(skill.effectType)
        {
            case EffectType.Damage:
                {
                    attributeChanges = new Attributes()
                    {
                        health = -user.CurrentStrength.Value,
                    };
                }
                break;
        }

        return new Performance(target, attributeChanges, skill);
    }
}
