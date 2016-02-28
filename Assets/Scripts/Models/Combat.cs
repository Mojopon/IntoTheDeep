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
    private List<CombatLog> combatlogs = new List<CombatLog>();

    public CharacterCombatResult(Character user, Skill usedSkill)
    {
        this.user = user;
        this.usedSkill = usedSkill;
    }

    public void AddCombatLog(CombatLog log)
    {
        combatlogs.Add(log);
    }

    public void AddCombatLog(List<CombatLog> logs)
    {
        foreach (var log in logs) AddCombatLog(log);
    }

    public CombatLog[] GetCombatLog()
    {
        return combatlogs.ToArray();
    }
}

public class CombatLog
{
    public enum CombatType
    {
        Damage,
        Healing,
    }

    public CombatType combatType;
    public Character target;
    public int amount;

    public CombatLog() { }
}

public static class Combat
{
    public static CharacterCombatResult DoCombat(Character user, Skill skill, Func<Coord, Character> characterOnTheLocation, int randomSeed)
    {
        var combatResult = new CharacterCombatResult(user, skill);

        foreach(var effect in skill.effects)
        {
            var combatLogs = effect.Apply(user, characterOnTheLocation);
            combatResult.AddCombatLog(combatLogs);
        }

        return combatResult;
    }


    public static int CalculateMagnitude(ICharacterAttributes user, ICharacterAttributes target, EffectAttribute effectAttribute, float minMultiply, float maxMultiply)
    {
        int power = 0;
        float reduction = 0;

        switch(effectAttribute)
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

        var powerRange = UnityEngine.Random.Range(minMultiply, maxMultiply);
        float actualPower = power * powerRange;
        float reducedPower = actualPower - (actualPower * reduction);
        int magnitude = Mathf.RoundToInt(reducedPower);

        return magnitude;
    }
}
