using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using System.Linq;
using System;

public class World : IWorldEventPublisher, IWorldUtilitiesProvider, IDisposable
{
    #region IWorldEvents Property Group
    public ReactiveProperty<Character> AddedCharacter { get; private set; }
    public ReactiveProperty<Character> CurrentActor { get; private set; }
    public ReactiveProperty<CharacterMoveResult> MoveResult { get; private set; }
    public ReactiveProperty<CharacterCombatResult> CombatResult { get; private set; }
    #endregion

    #region IWorldUtilities Property Group
    public Func<Character, Coord, bool> MoveChecker { get; private set; }
    public Func<Coord, Coord, Direction[]> Pathfinding { get; private set; }
    public Func<Coord, Character> CharacterOnTheLocation { get; private set; }
    #endregion

    private Map map;

    private List<Character> allCharacters = new List<Character>();
    private int currentActingCharacter = 0;

    private List<Character> enemies = new List<Character>();
    private int deadEnemies = 0;

    private CompositeDisposable disposables = new CompositeDisposable();
    public World(Map map)
    {
        AddedCharacter = new ReactiveProperty<Character>();
        CurrentActor = new ReactiveProperty<Character>();
        MoveResult = new ReactiveProperty<CharacterMoveResult>();
        CombatResult = new ReactiveProperty<CharacterCombatResult>();

        MoveChecker = new Func<Character, Coord, bool>((chara, coord) => CanMove(chara, coord));
        Pathfinding = new Func<Coord, Coord, Direction[]>((c1, c2) => GeneratePath(c1, c2));
        CharacterOnTheLocation = new Func<Coord, Character>((coord) => GetCharacter(coord));

        this.map = map;
        map.Subscribe(this)
           .AddTo(disposables);

        AddedCharacter.Where(x => x != null)
                      .Subscribe(x => allCharacters.Add(x))
                      .AddTo(disposables);
    }

    // actor means the character who is moving or fighting in the turn
    public void GoNextCharacterPhase()
    {
        if (CurrentActor.Value != null)
        {
            var currentCharacter = CurrentActor.Value;
            currentCharacter.SetPhase(Character.Phase.Idle);
        }

        var nextCharacter = GetNextCharacterToAction();
        CurrentActor.Value = nextCharacter;
    }

    Character GetNextCharacterToAction()
    {
        if (allCharacters.Count == 0) return null;

        var character = allCharacters[currentActingCharacter];
        currentActingCharacter++;
        if (currentActingCharacter >= allCharacters.Count) currentActingCharacter = 0;
        character.SetPhase(Character.Phase.Move);
        return character;
    }

    public Character GetCharacter(Coord location)
    {
        return map.GetCharacter(location);
    }

    public bool AddCharacter(Character character)
    {
        return AddCharacter(character, character.Location.Value);
    }

    public bool AddCharacter(Character character, int x, int y)
    {
        return AddCharacter(character, new Coord(x, y));
    }

    public bool AddCharacter(Character character, Coord destination)
    {
        if (allCharacters.Contains(character)) return false;
        if (!map.CanWalk(destination.x, destination.y, character)) return false;

        character.SetLocation(destination);
        // provide utilities for the character to act in the world
        character.GetWorldUtilities(this);
        character.Location
                 .DistinctUntilChanged()
                 .Scan((x, y) =>
                      {
                          this.MoveResult.Value = new CharacterMoveResult(character, x, y);
                          return y;
                      })
                 .Subscribe(x => { })
                 .AddTo(disposables);

        character.UsedSkill
                 .Where(x => x != null)
                 .Subscribe(x =>
                 {
                     var result = Combat.GetCombatResult(character, x, CharacterOnTheLocation, UnityEngine.Random.Range(0, 10000));
                     result.Apply();
                     this.CombatResult.Value = result;
                 })
                 .AddTo(disposables);

        AddedCharacter.Value = character;

        return true;
    }

    public bool AddCharacterAsEnemy(Character character)
    {
        return AddCharacterAsEnemy(character, character.Location.Value);
    }

    public bool AddCharacterAsEnemy(Character character, int x, int y)
    {
        return AddCharacterAsEnemy(character, new Coord(x, y));
    }

    public bool AddCharacterAsEnemy(Character character, Coord destination)
    {
        if (!AddCharacter(character, destination)) return false;

        enemies.Add(character);
        character.SetIsPlayer(false);
        character.SetAlliance(Alliance.Enemy);
        deadEnemies++;

        character.Dead
                 .DistinctUntilChanged()
                 .Subscribe(x =>
                 {
                     // the added character should return Dead as false for the first time 
                     // so we subtract one to keep zero when the character isnt dead and we add one when the character dies
                     if (x) deadEnemies++;
                     else deadEnemies--;
                 })
                 .AddTo(disposables);

        return true;
    }

    public bool EnemyIsAnnihilated { get { return enemies.Count - deadEnemies == 0; } }

    public List<Character> GetAllHostiles(Character character)
    {
        return allCharacters.Where(x => x.Alliance != character.Alliance).ToList();
    }

    public Character GetClosestHostile(Character character)
    {
        var hostiles = GetAllHostiles(character);
        return hostiles.OrderBy(x => Coord.Distance(x.Location.Value, character.Location.Value))
                       .DefaultIfEmpty(null)
                       .First();
    }

    public void ApplyMove(Character character, Direction direction)
    {
        if (!character.CanMoveTo(direction)) return;

        character.Move(direction);
    }

    public void ApplyTransfer(Character character, Coord destination)
    {
        if (!character.CanTransferTo(destination)) return;

        character.Transfer(destination);
    }

    public void ApplyUseSkill(Character character, Skill skill)
    {
        character.UseSkill(skill);
    }

    bool CanMove(Character character, Coord coord)
    {
        int x = coord.x;
        int y = coord.y;
        if (x < 0 || y < 0 || x >= map.Width || y >= map.Depth) return false;
        if (!map.CanWalk(coord.x, coord.y ,character)) return false;

        return true;
    }

    Direction[] GeneratePath(Coord source, Coord target)
    {
        List<Direction> directions = new List<Direction>();
        var currentPosition = source;
        for (int i = 0; i < 10; i++)
        {
            var rand = UnityEngine.Random.Range(0, 5);
            var direction = Direction.None;
            switch (rand)
            {
                case 0:
                    direction = Direction.None;
                    break;
                case 1:
                    direction = Direction.Up;
                    break;
                case 2:
                    direction = Direction.Right;
                    break;
                case 3:
                    direction = Direction.Down;
                    break;
                case 4:
                    direction = Direction.Left;
                    break;
            }

            var destination = currentPosition + direction.ToCoord();
            if (CanMove(null, destination))
            {
                currentPosition += direction.ToCoord();
                directions.Add(direction);
            }
        }

        return directions.ToArray();
    }

    #region IDisposables Method
    public void Dispose()
    {
        disposables.Dispose();
    }
    #endregion
}
