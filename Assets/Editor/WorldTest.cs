using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;

[TestFixture]
public class WorldTest
{
    Map map;
    World world;
    [SetUp]
    public void Initialize()
    {
        map = new Map();
        map.Width = 10;
        map.Depth = 10;
        map.Initialize();
        world = new World(map);
    }

    [Test]
    public void ShouldReturnTrueWhenAllEnemiesAreDead()
    {
        var enemyOne = new Character();
        var enemyTwo = new Character();
        Assert.IsTrue(world.EnemyIsAnnihilated);

        world.AddCharacterAsEnemy(enemyOne);
        world.AddCharacterAsEnemy(enemyTwo);

        Assert.IsFalse(world.EnemyIsAnnihilated);
        enemyOne.ApplyHealthChange(-10);
        enemyTwo.ApplyHealthChange(-10);

        Assert.IsTrue(world.EnemyIsAnnihilated);
    }

    [Test]
    public void CantAddToTheCellCantMoveTo()
    {
        var character = new Character();
        Assert.IsTrue(world.AddCharacter(character, 0, 0));

        var characterTwo = new Character();
        // false meand we couldnt add the character to the world(in the place)
        Assert.False(world.AddCharacter(characterTwo, 0, 0));
    }

    [Test]
    public void CantMoveToTheObstacleAfterAddedToTheWorld()
    {
        var character = new Character();
        character.SetPhase(Character.Phase.Move);
        Assert.IsTrue(character.CanMove(Direction.Right));
        Assert.AreEqual(0, character.X);
        Assert.AreEqual(0, character.Y);

        map = new Map();
        map.Width = 5;
        map.Depth = 5;
        map.Initialize();
        map.GetCell(1, 0).canWalk = false;
        world = new World(map);

        world.AddCharacter(character);
        Assert.IsFalse(character.CanMove(Direction.Right));
        character.Move(Direction.Right);
        Assert.AreEqual(0, character.X);
        Assert.AreEqual(0, character.Y);
    }

    [Test]
    public void CantMoveToTheOccupiedPlace()
    {
        var character = new Character();
        character.SetPhase(Character.Phase.Move);
        var characterTwo = new Character();
        characterTwo.SetPhase(Character.Phase.Move);
        var enemy = new Character();

        Assert.IsTrue(world.AddCharacter(character, 0, 1));
        Assert.IsTrue(world.AddCharacter(characterTwo, 1, 1));
        Assert.IsTrue(world.AddCharacter(enemy, 0, 2));

        Assert.IsTrue(character.CanMove(Direction.Down));
        Assert.IsFalse(character.CanMove(Direction.Right));
        Assert.IsFalse(character.CanMove(Direction.Up));

        Assert.IsTrue(characterTwo.CanMove(Direction.Right));

        characterTwo.SetPhase(Character.Phase.Move);
        world.ApplyMove(characterTwo, Direction.Right);
        Assert.AreEqual(2, characterTwo.X);
        Assert.AreEqual(1, characterTwo.Y);

        Assert.IsTrue(character.CanMove(Direction.Down));
        Assert.IsTrue(character.CanMove(Direction.Right));
        Assert.IsFalse(character.CanMove(Direction.Up));
    }

    [Test]
    public void ShouldReturnNextCharacterAndSetItToBeMovePhase()
    {
        // returns null when no characters in the world
        Assert.IsNull(world.CurrentActor.Value);

        Character currentActor = null;
        world.CurrentActor.Subscribe(x => currentActor = x);

        var character = new Character();
        var characterTwo = new Character();

        Assert.IsTrue(world.AddCharacter(character, 0, 0));
        Assert.IsTrue(world.AddCharacter(characterTwo, 1, 1));

        Assert.AreEqual(Character.Phase.Idle, character.CurrentPhase.Value);
        Assert.AreEqual(Character.Phase.Idle, characterTwo.CurrentPhase.Value);

        world.GoNextCharacterPhase();
        Assert.AreEqual(character, currentActor);
        Assert.AreEqual(Character.Phase.Move, character.CurrentPhase.Value);
        Assert.AreEqual(Character.Phase.Idle, characterTwo.CurrentPhase.Value);
        character.SetPhase(Character.Phase.Idle);

        world.GoNextCharacterPhase();
        Assert.AreEqual(characterTwo, currentActor);
        Assert.AreEqual(Character.Phase.Idle, character.CurrentPhase.Value);
        Assert.AreEqual(Character.Phase.Move, characterTwo.CurrentPhase.Value);
        characterTwo.SetPhase(Character.Phase.Idle);

        world.GoNextCharacterPhase();
        Assert.AreEqual(character, currentActor);
        Assert.AreEqual(Character.Phase.Move, character.CurrentPhase.Value);
        Assert.AreEqual(Character.Phase.Idle, characterTwo.CurrentPhase.Value);
    }

    [Test]
    public void ApplyMovementTotheCharacterAndtheMap()
    {
        var character = new Character();
        world.AddCharacter(character);

        Assert.AreEqual(0, character.X);
        Assert.AreEqual(0, character.Y);

        Assert.IsTrue(map.GetCell(0, 0).hasCharacter);
        Assert.AreEqual(character, map.GetCell(0, 0).characterInTheCell);

        world.GoNextCharacterPhase();
        Assert.AreEqual(character, world.CurrentActor.Value);

        Assert.IsTrue(character.CanMove(Direction.Right));
        world.ApplyMove(character, Direction.Right);
        Assert.AreEqual(1, character.X);
        Assert.AreEqual(0, character.Y);
        Assert.IsFalse(map.GetCell(0, 0).hasCharacter);
        Assert.IsNull(map.GetCell(0, 0).characterInTheCell);

        Assert.IsTrue(map.GetCell(1, 0).hasCharacter);
        Assert.AreEqual(character, map.GetCell(1, 0).characterInTheCell);
    }

    [Test]
    public void ShouldSetToBeEnemyAllianceWhenAddedAsEnemy()
    {
        var enemy = new Character();
        //should be created as player on default
        Assert.AreEqual(Alliance.Player, enemy.Alliance);
        world.AddCharacterAsEnemy(enemy);
        Assert.AreEqual(Alliance.Enemy, enemy.Alliance);
    }

    [Test]
    public void ShouldReturnHostiles()
    {
        var character = new Character();
        var characterTwo = new Character();

        var enemy = new Character();
        var enemyTwo = new Character();

        
        Assert.IsTrue(world.AddCharacter(character, 0, 0));
        Assert.IsTrue(world.AddCharacter(characterTwo, 1, 1));

        Assert.IsTrue(world.AddCharacterAsEnemy(enemy, 2, 2));
        Assert.IsTrue(world.AddCharacterAsEnemy(enemyTwo, 3, 3));

        var playersHostiles = world.GetAllHostiles(character);

        Assert.IsTrue(playersHostiles.Contains(enemy));
        Assert.IsTrue(playersHostiles.Contains(enemyTwo));
        Assert.IsFalse(playersHostiles.Contains(character));
        Assert.IsFalse(playersHostiles.Contains(characterTwo));

        playersHostiles = world.GetAllHostiles(characterTwo);

        Assert.IsTrue(playersHostiles.Contains(enemy));
        Assert.IsTrue(playersHostiles.Contains(enemyTwo));
        Assert.IsFalse(playersHostiles.Contains(character));
        Assert.IsFalse(playersHostiles.Contains(characterTwo));

        var enemysHostiles = world.GetAllHostiles(enemy);
        Assert.IsFalse(enemysHostiles.Contains(enemy));
        Assert.IsFalse(enemysHostiles.Contains(enemyTwo));
        Assert.IsTrue(enemysHostiles.Contains(character));
        Assert.IsTrue(enemysHostiles.Contains(characterTwo));

        enemysHostiles = world.GetAllHostiles(enemyTwo);
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

        world.AddCharacter(character, 1, 1);
        world.AddCharacter(characterTwo, 5, 5);

        //should return null when theres no hostiles
        Assert.IsNull(world.GetClosestHostile(character));
        Assert.IsNull(world.GetClosestHostile(characterTwo));

        world.AddCharacterAsEnemy(enemy, 4, 4);
        world.AddCharacterAsEnemy(enemyTwo, 3, 3);

        var closestHostile = world.GetClosestHostile(character);
        Assert.AreEqual(enemyTwo, closestHostile);

        closestHostile = world.GetClosestHostile(characterTwo);
        Assert.AreEqual(enemy, closestHostile);

        closestHostile = world.GetClosestHostile(enemy);
        Assert.AreEqual(characterTwo, closestHostile);

        closestHostile = world.GetClosestHostile(enemyTwo);
        Assert.AreEqual(character, closestHostile);
    }
}
