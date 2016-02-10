using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class Enemies
{
    List<ICharacter> enemies = new List<ICharacter>();
    private int deadCharacters = 0;

    public Enemies() { }

    public void AddCharacterAsEnemy(Character character)
    {
        enemies.Add(character);
        character.SetIsPlayer(false);
        deadCharacters++;

        character.Dead
                 .DistinctUntilChanged()
                 .Subscribe(x =>
                 {
                     // the added character should return Dead as false for the first time 
                     // so we subtract when the character isnt dead and we add when the character dies
                     if (x) deadCharacters++;
                     else deadCharacters--;
                 })
                 .AddTo(character.Disposables);
    }

    public bool IsAnnihilated { get { return enemies.Count - deadCharacters == 0; } }
}
