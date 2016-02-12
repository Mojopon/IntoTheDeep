using UnityEngine;
using System.Collections;
using NUnit.Framework;

[TestFixture]
public class WorldCharactersTest
{
    WorldCharacters worldCharacters;
    [SetUp]
    public void Initialize()
    {
        worldCharacters = new WorldCharacters();
    }

    [Test]
    public void ShouldReturnTrueWhenAllEnemiesAreDead()
    {
        var enemyOne = new Character();
        var enemyTwo = new Character();
        Assert.IsTrue(worldCharacters.EnemyIsAnnihilated);

        worldCharacters.AddCharacterAsEnemy(enemyOne);
        worldCharacters.AddCharacterAsEnemy(enemyTwo);

        Assert.IsFalse(worldCharacters.EnemyIsAnnihilated);
        enemyOne.ApplyHealthChange(-10);
        enemyTwo.ApplyHealthChange(-10);

        Assert.IsTrue(worldCharacters.EnemyIsAnnihilated);
    }

    [Test]
    public void ShouldReturnNextCharacterAndSetItToBeMovePhase()
    {
        // returns null when no characters in the world
        Assert.IsNull(worldCharacters.GetNextCharacterToAction());

        var character = new Character();
        var characterTwo = new Character();

        worldCharacters.AddCharacter(character);
        worldCharacters.AddCharacter(characterTwo);

        Assert.AreEqual(Character.Phase.Idle, character.CurrentPhase.Value);
        Assert.AreEqual(Character.Phase.Idle, characterTwo.CurrentPhase.Value);

        Assert.AreEqual(character, worldCharacters.GetNextCharacterToAction());
        Assert.AreEqual(Character.Phase.Move, character.CurrentPhase.Value);
        Assert.AreEqual(Character.Phase.Idle, characterTwo.CurrentPhase.Value);
        character.SetPhase(Character.Phase.Idle);

        Assert.AreEqual(characterTwo, worldCharacters.GetNextCharacterToAction());
        Assert.AreEqual(Character.Phase.Idle, character.CurrentPhase.Value);
        Assert.AreEqual(Character.Phase.Move, characterTwo.CurrentPhase.Value);
        characterTwo.SetPhase(Character.Phase.Idle);

        Assert.AreEqual(character, worldCharacters.GetNextCharacterToAction());
        Assert.AreEqual(Character.Phase.Move, character.CurrentPhase.Value);
        Assert.AreEqual(Character.Phase.Idle, characterTwo.CurrentPhase.Value);
    }

    [Test]
    public void ApplyMovementToTheCharacter()
    {
        var character = new Character();
        worldCharacters.AddCharacter(character);

        Assert.AreEqual(0, character.X);
        Assert.AreEqual(0, character.Y);
    }
}
