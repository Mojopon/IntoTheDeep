using UnityEngine;
using System.Collections;
using System;
using UniRx;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour, IWorldEventSubscriber, IWorldUtilitiesUser, IMapInstanceUtilitiesUser
{
    public Transform characterPrefab;

    public Func<Character,Coord, bool> MoveChecker { get; set; }
    public Func<int, int, Vector2> CoordToWorldPositionConverter { get; set; }
    public Func<Coord, Coord, Direction[]> Pathfinding { get; set; }

    private World world;
    private Dictionary<Character, CharacterController> characters = new Dictionary<Character, CharacterController>();

    public void Initialize(MapInstance mapInstance, World world)
    {
        mapInstance.ProvideMapInstanceUtilities(this);
        world.ProvideWorldUtilities(this);
        Subscribe(world).AddTo(gameObject);
    }

    public void Spawn(Character character)
    {
        var characterObj = Instantiate(characterPrefab,
                                       CoordToWorldPositionConverter(character.X, character.Y),
                                       characterPrefab.rotation) as Transform;
        characterObj.SetParent(transform);

        var characterController = characterObj.GetComponent<CharacterController>();
        characters.Add(character, characterController);

        character.Location
                 .Subscribe(coord => 
                 {
                     characterController.Move(CoordToWorldPositionConverter(coord.x, coord.y));
                 });
    }

    public IDisposable Subscribe(IWorldEventPublisher publisher)
    {
        var disposables = new CompositeDisposable();
        publisher.AddedCharacter
                 .Where(x => x != null)
                 .Subscribe(x => Spawn(x))
                 .AddTo(disposables);

        return disposables;
    }
}
