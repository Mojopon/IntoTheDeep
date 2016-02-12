using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using System.Linq;

public class WorldCharacters
{
    private List<Character> allCharacters = new List<Character>();
    private int currentActingCharacter = 0;

    private List<Character> enemies = new List<Character>();
    private int deadEnemies = 0;

    public Character GetNextCharacterToAction()
    {
        if (allCharacters.Count == 0) return null;

        var character = allCharacters[currentActingCharacter];
        currentActingCharacter++;
        if (currentActingCharacter >= allCharacters.Count) currentActingCharacter = 0;
        character.SetPhase(Character.Phase.Move);
        return character;
    }

    public WorldCharacters() { }

    public void AddCharacter(Character character)
    {
        allCharacters.Add(character);
    }

    public void AddCharacterAsEnemy(Character character)
    {
        AddCharacter(character);

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
        character.Move(direction);
    }

    public bool EnemyIsAnnihilated { get { return enemies.Count - deadEnemies == 0; } }
}
