using UnityEngine;
using System.Collections;
using System;

// map instance utilities are providing things like that:
// Vector2 Position in the game scene.
// Character Transforms on the scene.

public interface IMapInstanceUtilitiesProvider
{
    Func<int, int, Vector2> CoordToWorldPositionConverter { get; }
}
