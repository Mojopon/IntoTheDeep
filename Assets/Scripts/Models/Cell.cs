using UnityEngine;
using System.Collections;

public class Cell
{
    public bool isExit = false;
    public bool canWalk = true;
    public Character characterInTheCell { get; private set; }
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

    public void SetCharacter(Character character)
    {
        if(character == null)
        {
            RemoveCharacter();
            return;
        }

        this.characterInTheCell = character;
    }

    public void RemoveCharacter()
    {
        this.characterInTheCell = null;
    }
}
