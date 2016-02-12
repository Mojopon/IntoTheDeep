using UnityEngine;
using System.Collections;
using System;

public interface IWorldUtilitiesUser
{
    Func<Character, Coord, bool> MoveChecker { get; set; }
    Func<Coord, Coord, Direction[]> Pathfinding { get; set; }
}
