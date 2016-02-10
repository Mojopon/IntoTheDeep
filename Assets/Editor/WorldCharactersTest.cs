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
    public void ShouldReturnNextCharacterToMoveAndAction()
    {
        // returns null when no characters in the world
        Assert.IsNull(worldCharacters.GetNextCharacterToAction());

        var character = new Character();
        var characterTwo = new Character();

        worldCharacters.AddCharacter(character);
        worldCharacters.AddCharacter(characterTwo);

        Assert.AreEqual(character, worldCharacters.GetNextCharacterToAction());
        Assert.AreEqual(characterTwo, worldCharacters.GetNextCharacterToAction());
        Assert.AreEqual(character, worldCharacters.GetNextCharacterToAction());
    }
}
