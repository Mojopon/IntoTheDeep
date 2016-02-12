using UnityEngine;
using System.Collections;
using System;

public interface IMapInstanceUtilitiesUser
{
    Func<int, int, bool> MoveChecker { get; set; }
    Func<int, int, Vector2> CoordToWorldPositionConverter { get; set; }
    Func<Coord, Coord, Direction[]> Pathfinding { get; set; }
}
