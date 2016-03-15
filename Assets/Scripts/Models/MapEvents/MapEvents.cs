using UnityEngine;
using System.Collections;
using System;

public class MapEvents : IMapEvent
{
    public StartPositionsMapEvent startPositions;

    public MapEvents() { }

    public void Apply(Map targetMap)
    {
        
    }
}
