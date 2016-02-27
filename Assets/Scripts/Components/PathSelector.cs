using UnityEngine;
using System.Collections;
using System;
using UniRx;
using System.Collections.Generic;

public class PathSelector : MonoBehaviour, IWorldUtilitiesUser, IMapInstanceUtilitiesUser
{
    public Transform marker;

    public Func<int, int, Vector2> CoordToWorldPositionConverter { get; set; }
    public Func<Character, Coord, bool> MoveChecker { get; set; }
    public Func<Coord, Coord, Direction[]> Pathfinding { get; set; }

    private GameManager gameManager;
    private Character character;
    private World world;
    public void Initialize(GameManager gameManager, MapInstance mapInstance, Character character, World world)
    {
        this.character = character;
        this.gameManager = gameManager;
        this.world = world;

        GetWorldUtilities(world);

        GetMapInstanceUtilities(mapInstance);
    }

    private ReactiveProperty<bool> moveDone = new ReactiveProperty<bool>();
    public IEnumerator SequenceRouting()
    {
        moveDone.Value = false;
        StartRouting(character.MaxMove);

        while (!moveDone.Value)
        {
            yield return null;
        }

        // destroy selecter when routing is completed
        yield return null;
        Destroy(gameObject);
        yield break;
    }

    private int canMoveTime = 0;
    private int movedTime = 0;
    private ReactiveProperty<bool> moveSubmitted = new ReactiveProperty<bool>(false);
    private ReactiveProperty<Coord> markerLocation = new ReactiveProperty<Coord>();
    public void StartRouting(int canMoveTime)
    {
        this.canMoveTime = canMoveTime;

        if(character.IsPlayer)
        {
            PlayerRouting();
        }
        else if(!character.IsPlayer)
        {
            NPCRouting();
        }
    }

    private ReactiveProperty<PlayerCommand> PlayerInput = new ReactiveProperty<PlayerCommand>(PlayerCommand.None);
    void PlayerRouting()
    {
        var markerObject = Instantiate(marker, Vector3.zero, Quaternion.identity) as Transform;
        markerLocation.Value = new Coord(character.X, character.Y);

        markerLocation.Subscribe(coord => markerObject.position = CoordToWorldPositionConverter(coord.x, coord.y))
                      .AddTo(markerObject.gameObject);

        moveSubmitted.Where(x => x)
                     .Subscribe(x =>
                     {
                         Destroy(markerObject.gameObject);
                     })
                     .AddTo(markerObject.gameObject);

        SubscribePlayerInput(InputManager.Root).AddTo(markerObject.gameObject);
    }

    IDisposable SubscribePlayerInput(IPlayerInput inputs)
    {
        var compositeDisposable = new CompositeDisposable();

        inputs.MoveDirectionObservable
              .Skip(1)
              .Where(x => x != Direction.None)
              .Subscribe(x => MoveMarker(x))
              .AddTo(compositeDisposable);

        inputs.OnEnterButtonObservable
              .Skip(1)
              .Where(x => x)
              .Subscribe(x => ApplyMove())
              .AddTo(compositeDisposable);

        inputs.OnCancelButtonObservable
              .Skip(1)
              .Where(x => x)
              .Subscribe(x => CancelMove())
              .AddTo(compositeDisposable);

        return compositeDisposable;
    }

    void NPCRouting()
    {
        var closestHostile = world.GetClosestHostile(character);

        var routes = Pathfinding(character.Location.Value, closestHostile.Location.Value);

        foreach(var direction in routes)
        {
            movedDirections.Add(direction);
        }

        ApplyMove();
    }

    private List<Direction> movedDirections = new List<Direction>();
    void MoveMarker(Direction direction)
    {
        var destination = markerLocation.Value + direction.ToCoord();
        if (!MoveChecker(character, destination)) return;

        markerLocation.Value += direction.ToCoord();
        movedTime++;
        movedDirections.Add(direction);

        if (movedTime == canMoveTime) ApplyMove();
    }

    void CancelMove()
    {
        if (movedDirections.Count <= 0) return;

        var previousMove = movedDirections[--movedTime];
        markerLocation.Value += previousMove.GetOpposide().ToCoord();
        movedDirections.RemoveAt(movedDirections.Count - 1);
    }

    void ApplyMove()
    {
        moveSubmitted.Value = true;

        StartCoroutine(SequenceApplyMove());
    }

    IEnumerator SequenceApplyMove()
    {
        foreach (var direction in movedDirections)
        {
            world.ApplyMove(character, direction);
            yield return new WaitForSeconds(GlobalSettings.Instance.CharacterMoveSpeed);
        }

        while(character.CanMove)
        {
            world.ApplyMove(character, Direction.None);
        }

        moveDone.Value = true;
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
