using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class StartPositionsMapEvent : IMapEvent
{
    public List<Coord> startPositions;

    public StartPositionsMapEvent()
    {
    }

    public void EditStartPosition(int target, Coord newStartPosition)
    {
        startPositions[target] = newStartPosition;
    }

    public void AddStartPosition(Coord startPosition)
    {
        if (startPositions == null) startPositions = new List<Coord>();

        startPositions.Add(startPosition);
    }

    public void Apply(Map targetMap)
    {
        targetMap.playerStartPositions = startPositions.ToArray();
    }

    public static StartPositionsMapEvent Create()
    {
        var startPositionsMapEvent = new StartPositionsMapEvent();
        startPositionsMapEvent.AddStartPosition(new Coord(1, 1));
        startPositionsMapEvent.AddStartPosition(new Coord(2, 1));
        startPositionsMapEvent.AddStartPosition(new Coord(3, 1));
        startPositionsMapEvent.AddStartPosition(new Coord(4, 1));

        return startPositionsMapEvent;
    }
}
