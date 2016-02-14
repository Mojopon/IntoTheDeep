using UnityEngine;
using System.Collections;
using System;

public interface IWorldUtilitiesUser
{
    // should return true when the character can move to the coord;
    Func<Character, Coord, bool> MoveChecker { get; set; }
    Func<Coord, Coord, Direction[]> Pathfinding { get; set; }
}
