using UnityEngine;
using System.Collections;
using System;

public enum EffectAttribute
{
    MeleePower,
    RangePower,
    SpellPower,
}

[Serializable]
public class Skill
{
    public string name;
    public SkillType skillType;
    public EffectType effectType;
    public EffectAttribute effectAttribute;

    public float minMultiply = 1f;
    public float maxMultiply = 1.1f;
    public Coord[] range;
}
