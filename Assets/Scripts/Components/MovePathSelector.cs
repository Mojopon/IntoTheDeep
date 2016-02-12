using UnityEngine;
using System.Collections;
using System;
using UniRx;
using System.Collections.Generic;

public class MovePathSelector : MonoBehaviour, IMapInstanceUtilitiesUser
{
    public Transform marker;

    public Func<int, int, Vector2> CoordToWorldPositionConverter { get; set; }
    public Func<int, int, bool> MoveChecker { get; set; }
    public Func<Coord, Coord, Direction[]> Pathfinding { get; set; }

    private GameManager gameManager;
    private Character character;
    private WorldCharacters worldCharacters;
    public void Initialize(GameManager gameManager, MapInstance map, Character character, WorldCharacters worldCharacters)
    {
        map.RegisterUtilityUser(this);
        this.character = character;
        this.gameManager = gameManager;
        this.worldCharacters = worldCharacters;
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
                     });

        gameManager.PlayerInput
                   .Where(x => !gameManager.MenuIsOpened && x != PlayerCommand.None)
                   .Subscribe(x => OnInput(x))
                   .AddTo(markerObject.gameObject);
    }

    void NPCRouting()
    {
        var closestHostile = worldCharacters.GetClosestHostile(character);

        var routes = Pathfinding(character.Location.Value, closestHostile.Location.Value);

        foreach(var direction in routes)
        {
            movedDirections.Add(direction);
        }

        ApplyMove();
    }

    void OnInput(PlayerCommand command)
    {
        switch(command)
        {
            case PlayerCommand.Left:
                MoveMarker(Direction.Left);
                break;
            case PlayerCommand.Right:
                MoveMarker(Direction.Right);
                break;
            case PlayerCommand.Down:
                MoveMarker(Direction.Down);
                break;
            case PlayerCommand.Up:
                MoveMarker(Direction.Up);
                break;
            case PlayerCommand.Cancel:
                CancelMove();
                break;
            case PlayerCommand.Enter:
                ApplyMove();
                break;
        }
    }

    private List<Direction> movedDirections = new List<Direction>();
    void MoveMarker(Direction direction)
    {
        var destination = markerLocation.Value + direction.ToCoord();
        if (!MoveChecker(destination.x, destination.y)) return;

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
            worldCharacters.ApplyMove(character, direction);
            yield return new WaitForSeconds(0.1f);
        }

        moveDone.Value = true;
    }
}
