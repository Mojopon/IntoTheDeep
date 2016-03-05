using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class StartPositionsMapEvent : IMapEvent
{
    public List<Coord> startPositions = null;

    public StartPositionsMapEvent() { }

    public void AddStartPosition(Coord startPosition)
    {
        if (startPositions == null) startPositions = new List<Coord>();

        startPositions.Add(startPosition);
    }

    public void Apply(Map targetMap)
    {
        targetMap.playerStartPositions = startPositions.ToArray();
    }
}
