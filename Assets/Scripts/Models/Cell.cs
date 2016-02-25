using UnityEngine;
using System.Collections;

public class Cell
{
    public int X;
    public int Y;
    public Coord Location { get { return new Coord(X, Y); } }

    public bool isExit = false;
    public bool canWalk = true;
    public Character characterInTheCell { get; private set; }
    public bool hasCharacter { get { return characterInTheCell != null; } }
    public Cell(int x, int y)
    {
        this.X = x;
        this.Y = y;
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

    public bool IsAvailable()
    {
        return canWalk && !hasCharacter;
    }

    public void RemoveCharacter()
    {
        this.characterInTheCell = null;
    }
}
