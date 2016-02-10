using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Map
{
    public int Width;
    public int Depth;

    private Cell[,] cells;

    public Map() { }

    public void Initialize()
    {
        cells = new Cell[Width, Depth];
        for (int y = 0; y < Depth; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                cells[x, y] = new Cell();
            }
        }
    }

    public Cell GetCell(int x, int y)
    {
        if (IsOutOfRange(x, y)) return null;
        return cells[x, y];
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
            return true;
        }

        return false;
    }
}
