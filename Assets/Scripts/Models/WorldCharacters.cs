using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;

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

    public bool EnemyIsAnnihilated { get { return enemies.Count - deadEnemies == 0; } }
}
