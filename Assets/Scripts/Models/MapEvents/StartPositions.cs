using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class StartPositions : IMapEvent
{
    public List<Coord> positions;

    public StartPositions()
    {
    }

    public void EditStartPosition(int target, Coord newStartPosition)
    {
        positions[target] = newStartPosition;
    }

    public void AddStartPosition(Coord startPosition)
    {
        if (positions == null) positions = new List<Coord>();

        positions.Add(startPosition);
    }

    public void Apply(Map targetMap)
    {
        targetMap.playerStartPositions = positions.ToArray();
    }

    public static StartPositions Create()
    {
        var startPositionsMapEvent = new StartPositions();
        startPositionsMapEvent.AddStartPosition(new Coord(1, 1));
        startPositionsMapEvent.AddStartPosition(new Coord(2, 1));
        startPositionsMapEvent.AddStartPosition(new Coord(3, 1));
        startPositionsMapEvent.AddStartPosition(new Coord(4, 1));

        return startPositionsMapEvent;
    }
}
