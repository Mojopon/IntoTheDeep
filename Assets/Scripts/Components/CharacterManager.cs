using UnityEngine;
using System.Collections;
using System;
using UniRx;
using System.Collections.Generic;

// class to spawn character transforms and observe World Events to send
// message the controller
public class CharacterManager : MonoBehaviour, IWorldEventSubscriber, IWorldUtilitiesUser, IMapInstanceUtilitiesUser
{
    public Transform characterPrefab;

    public Func<Character,Coord, bool> MoveChecker { get; set; }
    public Func<int, int, Vector2> CoordToWorldPositionConverter { get; set; }
    public Func<Coord, Coord, Direction[]> Pathfinding { get; set; }

    public bool AllActionsDone { get { return true;  } }

    private World world;
    private Dictionary<Character, CharacterController> characters = new Dictionary<Character, CharacterController>();

    public void Initialize(MapInstance mapInstance, World world)
    {
        GetMapInstanceUtilities(mapInstance);

        Subscribe(world).AddTo(gameObject);
        GetWorldUtilities(world);
    }

    public void Spawn(Character character)
    {
        var characterObj = Instantiate(characterPrefab,
                                       CoordToWorldPositionConverter(character.X, character.Y),
                                       characterPrefab.rotation) as Transform;
        characterObj.SetParent(transform);

        var characterController = characterObj.GetComponent<CharacterController>();
        characters.Add(character, characterController);
    }

    void OnCharacterMove(CharacterMoveResult moveResult)
    {
        var controller = characters[moveResult.target];
        controller.Move(CoordToWorldPositionConverter(moveResult.destination.x, moveResult.destination.y));
    }

    public IDisposable Subscribe(IWorldEventPublisher publisher)
    {
        var disposables = new CompositeDisposable();

        publisher.AddedCharacter
                 .ToObservable()
                 .Where(x => x != null)
                 .Subscribe(x => Spawn(x))
                 .Dispose();

        publisher.AddedCharacter
                 .ObserveAdd()
                 .Select(x => x.Value)
                 .Where(x => x != null)
                 .Subscribe(x => Spawn(x))
                 .AddTo(disposables);

        publisher.MoveResult
                 .Where(x => x != null)
                 .Subscribe(x => OnCharacterMove(x));

        publisher.CombatResult
                 .Where(x => x != null)
                 .Subscribe(x =>
                 {
                     //Debug.Log(x.user + " used " + x.usedSkill.name);
                     foreach(var performance in x.GetPerformances())
                     {
                         //Debug.Log(performance.target + " received " + performance.receivedSkill);
                         //Debug.Log(performance.skillEffect);
                     }
                 });
        return disposables;
    }

    public void GetWorldUtilities(IWorldUtilitiesProvider provider)
    {
        MoveChecker = provider.MoveChecker;
        Pathfinding = provider.Pathfinding;
    }

    public void GetMapInstanceUtilities(IMapInstanceUtilitiesProvider provider)
    {
        CoordToWorldPositionConverter = provider.CoordToWorldPositionConverter;
    }
}
