using UnityEngine;
using System.Collections;

// map instance utilities are providing things like that:
// Vector2 Position in the game scene.
// Character Transforms on the scene.

public interface IMapInstanceUtilitiesProvider
{
    void ProvideMapInstanceUtilities(IMapInstanceUtilitiesUser user);
}
