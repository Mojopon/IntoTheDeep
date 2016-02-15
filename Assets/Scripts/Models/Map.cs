using UnityEngine;
using System.Collections;
using System;
using UniRx;

[Serializable]
public class Map : IWorldEventSubscriber
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

    public void MoveCharacterToFrom(Character character, Coord source, Coord destination)
    {
        var sourceCell = GetCell(source);
        sourceCell.characterInTheCell = null;

        var destinationCell = GetCell(destination);
        destinationCell.characterInTheCell = character;
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

    public void SetCell(int x, int y, Cell cell)
    {
        if (IsOutOfRange(x, y)) return;

        cells[x, y] = cell;
    }

    public void SetCharacter(Character character)
    {
        cells[character.X, character.Y].characterInTheCell = character;
    }

    bool IsOutOfRange(int x, int y)
    {
        if(x < 0 || y < 0 || x >= Width || y >= Depth)
        {
            return true;
        }

        return false;
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
                 .Where(x => x != null)
                 .Subscribe(x => SetCharacter(x))
                 .AddTo(compositeDisposable);

        return compositeDisposable;
    }
    #endregion
}
