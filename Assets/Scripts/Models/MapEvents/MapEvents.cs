using UnityEngine;
using System.Collections;
using System;

public class MapEvents : IMapEvent
{
    public StartPositions startPositions;

    public MapEvents() { }

    public void Apply(Map targetMap)
    {
        startPositions.Apply(targetMap);
    }
}
