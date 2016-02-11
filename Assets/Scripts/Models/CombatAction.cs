using UnityEngine;
using System.Collections;

public class CombatActionResult
{
    public Character target { get; private set; }
    public int healthChange { get; private set; }
    public CombatActionResult(Character target, int healthChange)
    {
        this.target = target;
    }
}

public class CombatAction
{

}
