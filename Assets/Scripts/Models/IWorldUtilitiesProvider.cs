using UnityEngine;
using System.Collections;
using System;

public interface IWorldUtilitiesProvider
{
    Func<Character, Coord, bool> MoveChecker { get; }
    Func<Coord, Coord, Direction[]> Pathfinding { get; }
    Func<Coord, Character> CharacterOnTheLocation { get; }
}
