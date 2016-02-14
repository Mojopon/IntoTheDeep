using UnityEngine;
using System.Collections;

public class Cell
{
    public bool canWalk = true;
    public Character characterInTheCell = null;
    public bool hasCharacter { get { return characterInTheCell != null; } }
    public Cell()
    {

    }

    public bool CanWalk(Character character)
    {
        if (!canWalk) return false;

        if (hasCharacter && characterInTheCell != character) return false;

        return true;
    }
}
