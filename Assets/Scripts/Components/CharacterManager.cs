using UnityEngine;
using System.Collections;
using System;
using UniRx;
using System.Collections.Generic;

// class to spawn character transforms and observe World Events to send
// message the controller
public class CharacterManager : MonoBehaviour, IWorldEventSubscriber, IWorldUtilitiesUser, IMapInstanceUtilitiesUser
{
    public Transform playerPrefab;
    public Transform enemyPrefab;

    public HealthBar healthBarPrefab;

    public Func<Character,Coord, bool> MoveChecker { get; set; }
    public Func<int, int, Vector2> CoordToWorldPositionConverter { get; set; }
    public Func<Coord, Coord, Direction[]> Pathfinding { get; set; }

    public bool AllActionsDone { get; private set; }

    private World world;
    private Dictionary<Character, CharacterController> characters = new Dictionary<Character, CharacterController>();

    public void Initialize(MapInstance mapInstance, World world)
    {
        AllActionsDone = true;

        GetMapInstanceUtilities(mapInstance);

        Subscribe(world).AddTo(world.Disposables);
        GetWorldUtilities(world);
    }

    public void Spawn(Character character)
    {
        Transform characterObj;

        var positionToSpawn = CoordToWorldPositionConverter(character.X, character.Y);

        if (character.IsPlayer)
        {
            characterObj = Instantiate(playerPrefab,
                                       positionToSpawn,
                                       playerPrefab.rotation) as Transform;
        }
        else
        {
            characterObj = Instantiate(enemyPrefab,
                                       positionToSpawn,
                                       playerPrefab.rotation) as Transform;
        }
        characterObj.SetParent(transform);

        var characterController = characterObj.GetComponent<CharacterController>();
        characters.Add(character, characterController);

        character.Dead
                 .Where(x => x)
                 .Subscribe(x =>
                 {
                     Destroy(characterObj.gameObject);
                     characters.Remove(character);
                 }).AddTo(characterObj);

        var healthBar = Instantiate(healthBarPrefab, positionToSpawn, Quaternion.identity) as HealthBar;
        healthBar.transform.SetParent(characterObj);
        healthBar.ObserveOnCharacter(character);
    }

    void OnCharacterMove(CharacterMoveResult moveResult)
    {
        if (!characters.ContainsKey(moveResult.target)) return;

        var controller = characters[moveResult.target];
        controller.Move(CoordToWorldPositionConverter(moveResult.destination.x, moveResult.destination.y)).StartAsCoroutine();
    }

    void OnCharacterCombat(CharacterCombatResult combatResult)
    {
        StartCoroutine(SequenceCharacterCombatMotions(combatResult));
    }

    IEnumerator SequenceCharacterCombatMotions(CharacterCombatResult combatResult)
    {
        AllActionsDone = false;

        var userPosition = CoordToWorldPositionConverter(combatResult.user.X, combatResult.user.Y);

        foreach (var performance in combatResult.GetCombatLog())
        {
            var target = performance.target;
            if (!characters.ContainsKey(target)) continue;

            var targetPosition = CoordToWorldPositionConverter(target.X, target.Y);
            var direction = (targetPosition - userPosition).normalized / 2;

            ParticleSpawner.Instance.Spawn(ParticleType.AttackParticle, new Vector3(targetPosition.x, targetPosition.y, -1f));
            var targetTransformController = characters[target];
            targetTransformController.KnockBack(targetPosition + direction).StartAsCoroutine();

        }

        AllActionsDone = true;

        yield break;
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
                 .Subscribe(x => OnCharacterCombat(x));
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
