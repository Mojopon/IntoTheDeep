using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

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
