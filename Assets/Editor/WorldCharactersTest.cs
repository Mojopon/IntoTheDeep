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

    [Test]
    public void ShouldSetToBeEnemyAllianceWhenAddedAsEnemy()
    {
        var enemy = new Character();
        //should be created as player on default
        Assert.AreEqual(Alliance.Player, enemy.Alliance);
        worldCharacters.AddCharacterAsEnemy(enemy);
        Assert.AreEqual(Alliance.Enemy, enemy.Alliance);
    }

    [Test]
    public void ShouldReturnHostiles()
    {
        var character = new Character();
        var characterTwo = new Character();

        var enemy = new Character();
        var enemyTwo = new Character();

        worldCharacters.AddCharacter(character);
        worldCharacters.AddCharacter(characterTwo);

        worldCharacters.AddCharacterAsEnemy(enemy);
        worldCharacters.AddCharacterAsEnemy(enemyTwo);

        var playersHostiles = worldCharacters.GetAllHostiles(character);

        Assert.IsTrue(playersHostiles.Contains(enemy));
        Assert.IsTrue(playersHostiles.Contains(enemyTwo));
        Assert.IsFalse(playersHostiles.Contains(character));
        Assert.IsFalse(playersHostiles.Contains(characterTwo));

        playersHostiles = worldCharacters.GetAllHostiles(characterTwo);

        Assert.IsTrue(playersHostiles.Contains(enemy));
        Assert.IsTrue(playersHostiles.Contains(enemyTwo));
        Assert.IsFalse(playersHostiles.Contains(character));
        Assert.IsFalse(playersHostiles.Contains(characterTwo));

        var enemysHostiles = worldCharacters.GetAllHostiles(enemy);
        Assert.IsFalse(enemysHostiles.Contains(enemy));
        Assert.IsFalse(enemysHostiles.Contains(enemyTwo));
        Assert.IsTrue(enemysHostiles.Contains(character));
        Assert.IsTrue(enemysHostiles.Contains(characterTwo));

        enemysHostiles = worldCharacters.GetAllHostiles(enemyTwo);
        Assert.IsFalse(enemysHostiles.Contains(enemy));
        Assert.IsFalse(enemysHostiles.Contains(enemyTwo));
        Assert.IsTrue(enemysHostiles.Contains(character));
        Assert.IsTrue(enemysHostiles.Contains(characterTwo));
    }

    [Test]
    public void ShouldReturnClosestHostile()
    {
        var character = new Character();
        var characterTwo = new Character();

        var enemy = new Character();
        var enemyTwo = new Character();

        character.Location.Value = new Coord(1, 1);
        characterTwo.Location.Value = new Coord(5, 5);

        enemy.Location.Value = new Coord(4, 4);
        enemyTwo.Location.Value = new Coord(3, 3);

        worldCharacters.AddCharacter(character);
        worldCharacters.AddCharacter(characterTwo);

        //should return null when theres no hostiles
        Assert.IsNull(worldCharacters.GetClosestHostile(character));
        Assert.IsNull(worldCharacters.GetClosestHostile(characterTwo));

        worldCharacters.AddCharacterAsEnemy(enemy);
        worldCharacters.AddCharacterAsEnemy(enemyTwo);

        var closestHostile = worldCharacters.GetClosestHostile(character);
        Assert.AreEqual(enemyTwo, closestHostile);

        closestHostile = worldCharacters.GetClosestHostile(characterTwo);
        Assert.AreEqual(enemy, closestHostile);

        closestHostile = worldCharacters.GetClosestHostile(enemy);
        Assert.AreEqual(characterTwo, closestHostile);

        closestHostile = worldCharacters.GetClosestHostile(enemyTwo);
        Assert.AreEqual(character, closestHostile);
    }
}
