using UnityEngine;
using System.Collections.Generic;
using System;
using UniRx;
using UniRx.Triggers;

[Serializable]
public class Map : IWorldEventSubscriber
{
    public int Width;
    public int Depth;

    public IObservable<Coord> CellChangeObservable;

    public Coord exitLocation;
    public Coord[] playerStartPositions = new Coord[4];

    [HideInInspector]
    public bool PlayerIsInExit = false;

    private Cell[,] cells;
    private Subject<Coord> onCellChangeSubject = new Subject<Coord>();

    public Map()
    {
        CellChangeObservable = onCellChangeSubject.AsObservable();
    }

    public Map(int[,] tilePattern) : this()
    {
        this.Width = tilePattern.GetLength(0);
        this.Depth = tilePattern.GetLength(1);

        cells = new Cell[Width, Depth];
        for (int y = 0; y < Depth; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                cells[x, y] = new Cell(x, y);
                SetTileID(x, y, tilePattern[x, y]);
            }
        }
    }

    public void IncreaseMapWidth()
    {
        var newCells = new Cell[++Width, Depth];
        InitializeCells(newCells);
        WriteOldMapPatternToNewCells(newCells, this.cells);
        this.cells = newCells;
    }

    public void DecreaseMapWidth()
    {
        if (Width < 2) return;

        var newCells = new Cell[--Width, Depth];
        InitializeCells(newCells);
        WriteOldMapPatternToNewCells(newCells, this.cells);
        this.cells = newCells;
    }

    public void IncreaseMapDepth()
    {
        var newCells = new Cell[Width, ++Depth];
        InitializeCells(newCells);
        WriteOldMapPatternToNewCells(newCells, this.cells);
        this.cells = newCells;
    }

    public void DecreaseMapDepth()
    {
        if (Depth < 2) return;

        var newCells = new Cell[Width, --Depth];
        InitializeCells(newCells);
        WriteOldMapPatternToNewCells(newCells, this.cells);
        this.cells = newCells;
    }

    private void InitializeCells(Cell[,] cells)
    {
        for (int y = 0; y < cells.GetLength(1); y++)
        {
            for (int x = 0; x < cells.GetLength(0); x++)
            {
                cells[x, y] = new Cell(x, y);
            }
        }
    }

    private void WriteOldMapPatternToNewCells(Cell[,] newCells, Cell[,] oldCells)
    {
        for(int y = 0; y < newCells.GetLength(1); y++)
        {
            for (int x = 0; x < newCells.GetLength(0); x++)
            {
                if (x >= oldCells.GetLength(0) || y >= oldCells.GetLength(1)) continue;

                newCells[x, y].SetTileID(oldCells[x, y].tileID);
            }
        }
    }

    public void ApplyTileData(TileDatas tileDatas)
    {
        for (int y = 0; y < Depth; y++)
        {
            for (int x = 0; x < Width; x++)
            {   
                cells[x, y].ApplyTileData(tileDatas.Get(cells[x, y].tileID));
            }
        }
    }

    public void Initialize()
    {
        if (cells == null)
        {
            cells = new Cell[Width, Depth];
            for (int y = 0; y < Depth; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    cells[x, y] = new Cell(x, y);
                }
            }
        }

        cells[exitLocation.x, exitLocation.y].isExit = true;
    }

    public void MoveCharacterToFrom(Character character, Coord source, Coord destination)
    {
        var sourceCell = GetCell(source);
        sourceCell.SetCharacter(null);

        var destinationCell = GetCell(destination);
        destinationCell.SetCharacter(character);
    }

    public bool CanWalk(int x, int y, Character character)
    {
        if (IsOutOfRange(x, y)) return false;

        return GetCell(x, y).CanWalk(character);
    }

    public Cell GetCell(Coord coord)
    {
        return GetCell(coord.x, coord.y);
    }

    public Cell GetCell(int x, int y)
    {
        if (IsOutOfRange(x, y)) return null;
        return cells[x, y];
    }

    public void SetTileID(int x, int y, int id)
    {
        if (IsOutOfRange(x, y)) return;

        cells[x, y].SetTileID(id);
        onCellChangeSubject.OnNext(new Coord(x, y));
    }

    public List<Cell> GetAvailableCells()
    {
        var availableCells = new List<Cell>();

        for(int y = 0; y < Depth; y++)
        {
            for(int x = 0; x < Width; x++)
            {
                if(cells[x, y].IsAvailable())
                {
                    availableCells.Add(cells[x, y]);
                }
            }
        }

        return availableCells;
    }

    public void SetCharacter(Character character)
    {
        cells[character.X, character.Y].SetCharacter(character);
    }

    public void RemoveCharacter(Character character, Coord destination)
    {
        if(cells[destination.x, destination.y].characterInTheCell == character)
        {
            cells[destination.x, destination.y].SetCharacter(null);
        }
    }

    public Character GetCharacter(Coord location)
    {
        if (IsOutOfRange(location) || !cells[location.x, location.y].hasCharacter)
        {
            return null;
        }

        return cells[location.x, location.y].characterInTheCell;
    }

    public int[,] GetTilePattern()
    {
        var pattern = new int[Width, Depth];
        for (int y = 0; y < Depth; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                pattern[x, y] = cells[x, y].tileID;
            }
        }

        return pattern;
    }

    bool IsOutOfRange(int x, int y)
    {
        if(x < 0 || y < 0 || x >= Width || y >= Depth)
        {
            return true;
        }

        return false;
    }

    bool IsOutOfRange(Coord location)
    {
        return IsOutOfRange(location.x, location.y);
    }

    #region IWorldEventSubscriber Method
    public IDisposable Subscribe(IWorldEventPublisher publisher)
    {
        var compositeDisposable = new CompositeDisposable();

        publisher.MoveResult
                 .Where(x => x != null)
                 .Subscribe(x => MoveCharacterToFrom(x.target, x.source, x.destination))
                 .AddTo(compositeDisposable); 

        publisher.AddedCharacter
                 .ObserveAdd()
                 .Select(x => x.Value)
                 .Where(x => x != null)
                 .Subscribe(x => SetCharacter(x))
                 .AddTo(compositeDisposable);

        return compositeDisposable;
    }
    #endregion
}
