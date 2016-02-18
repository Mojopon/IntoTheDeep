using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Skill
{
    public string name;
    public SkillType skillType;
    public EffectType effectType;
    public int power;

    public Coord[] range;
}
