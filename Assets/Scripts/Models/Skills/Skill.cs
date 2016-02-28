using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

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
    public Effect[] effects;
}
