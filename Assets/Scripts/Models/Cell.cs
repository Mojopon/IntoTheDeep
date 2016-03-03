using UnityEngine;
using System.Collections;

public class Cell
{
    public int tileID { get; private set; }
    public int x;
    public int y;
    public Coord Location { get { return new Coord(x, y); } }

    public bool isExit = false;
    public bool canWalk = true;
    public Character characterInTheCell { get; private set; }
    public bool hasCharacter { get { return characterInTheCell != null; } }
    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
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

    public void SetTileID(int id)
    {
        this.tileID = id;
    }

    public void ApplyTileData(TileData tileData)
    {
        if (tileData.id != this.tileID) return;

        this.canWalk = tileData.canWalk;
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
