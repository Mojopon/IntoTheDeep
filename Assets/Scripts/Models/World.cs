using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using System.Linq;
using System;

public class World : IWorldUtilitiesProvider
{
    public ReactiveProperty<Character> CurrentActor = new ReactiveProperty<Character>();

    private Map map;

    private List<Character> allCharacters = new List<Character>();
    private int currentActingCharacter = 0;

    private List<Character> enemies = new List<Character>();
    private int deadEnemies = 0;

    public World(Map map)
    {
        this.map = map;
    }

    // actor means the character who is moving or fighting in the turn
    public void GoNextCharacterPhase()
    {
        CurrentActor.Value =  GetNextCharacterToAction();
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

    public bool AddCharacter(Character character)
    {
        if (allCharacters.Contains(character)) return false;
        if (!map.CanWalk(character.X, character.Y, character)) return false;

        ProvideWorldUtilities(character);
        allCharacters.Add(character);
        map.SetCharacter(character);

        return true;
    }

    public bool AddCharacterAsEnemy(Character character)
    {
        if (!AddCharacter(character)) return false;

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
                 .AddTo(character.Disposables);

        return true;
    }

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
        if (!character.CanMove(direction)) return;

        character.Move(direction);
        map.MoveCharacterToFrom(character, character.Location.Value, character.Location.Value + direction.ToCoord());
    }

    public void ApplyUseSkill(Character character, Skill skill)
    {
        character.OnSkillUsed(skill);
    }

    public bool EnemyIsAnnihilated { get { return enemies.Count - deadEnemies == 0; } }

    bool CanMove(Character character, Coord coord)
    {
        int x = coord.x;
        int y = coord.y;
        if (x < 0 || y < 0 || x >= map.Width || y >= map.Depth) return false;
        if (!map.GetCell(x, y).canWalk) return false;

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

    public void ProvideWorldUtilities(IWorldUtilitiesUser user)
    {
        user.MoveChecker = new Func<Character, Coord, bool>((character, coord) => CanMove(character, coord));
        user.Pathfinding = new Func<Coord, Coord, Direction[]>((source, target) => GeneratePath(source, target));
    }
}
