using UnityEngine;
using System.Collections.Generic;

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