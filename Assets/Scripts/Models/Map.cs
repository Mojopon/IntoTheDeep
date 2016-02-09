using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Map
{
    public int Width;
    public int Depth;

    private Cell[,] cells;

    public Map(int mapWidth, int mapDepth)
    {
        this.Width = mapWidth;
        this.Depth = mapDepth;

        cells = new Cell[mapWidth, mapDepth];
    }

    public Cell GetCell(int x, int y)
    {
        if (!IsOutOfRange(x, y)) return cells[x, y];
        else return null;
    }

    public void SetCell(int x, int y, Cell cell)
    {
        if (IsOutOfRange(x, y)) return;

        cells[x, y] = cell;
    }

    bool IsOutOfRange(int x, int y)
    {
        if(x < 0 || y < 0 || x >= Width || y >= Depth)
        {
            return false;
        }

        return true;
    }
}
