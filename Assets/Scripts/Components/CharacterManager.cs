using UnityEngine;
using System.Collections;
using System;

public class CharacterManager : MonoBehaviour, IMapInstanceUtilitiesUser
{
    public Transform characterPrefab;

    public Func<int, int, bool> MoveChecker { get; set; }
    public Func<int, int, Vector2> CoordToWorldPositionConverter { get; set; }

    private Transform characterObj;

    public void Spawn(Character character)
    {
        var newCharacter = Instantiate(characterPrefab,
                                       CoordToWorldPositionConverter(character.X, character.Y),
                                       characterPrefab.rotation) as Transform;
        newCharacter.SetParent(transform);
        characterObj = newCharacter;
    }

    public void Move(Vector2 destination)
    {

    }
}
