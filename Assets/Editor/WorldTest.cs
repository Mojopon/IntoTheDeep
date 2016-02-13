﻿using UnityEngine;
using System.Collections;
using NUnit.Framework;

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
    public void ShouldReturnNextCharacterAndSetItToBeMovePhase()
    {
        // returns null when no characters in the world
        Assert.IsNull(world.GetNextCharacterToAction());

        var character = new Character();
        var characterTwo = new Character();

        world.AddCharacter(character);
        world.AddCharacter(characterTwo);

        Assert.AreEqual(Character.Phase.Idle, character.CurrentPhase.Value);
        Assert.AreEqual(Character.Phase.Idle, characterTwo.CurrentPhase.Value);

        Assert.AreEqual(character, world.GetNextCharacterToAction());
        Assert.AreEqual(Character.Phase.Move, character.CurrentPhase.Value);
        Assert.AreEqual(Character.Phase.Idle, characterTwo.CurrentPhase.Value);
        character.SetPhase(Character.Phase.Idle);

        Assert.AreEqual(characterTwo, world.GetNextCharacterToAction());
        Assert.AreEqual(Character.Phase.Idle, character.CurrentPhase.Value);
        Assert.AreEqual(Character.Phase.Move, characterTwo.CurrentPhase.Value);
        characterTwo.SetPhase(Character.Phase.Idle);

        Assert.AreEqual(character, world.GetNextCharacterToAction());
        Assert.AreEqual(Character.Phase.Move, character.CurrentPhase.Value);
        Assert.AreEqual(Character.Phase.Idle, characterTwo.CurrentPhase.Value);
    }

    [Test]
    public void ApplyMovementToTheCharacter()
    {
        var character = new Character();
        world.AddCharacter(character);

        Assert.AreEqual(0, character.X);
        Assert.AreEqual(0, character.Y);
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

        world.AddCharacter(character);
        world.AddCharacter(characterTwo);

        world.AddCharacterAsEnemy(enemy);
        world.AddCharacterAsEnemy(enemyTwo);

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

        character.Location.Value = new Coord(1, 1);
        characterTwo.Location.Value = new Coord(5, 5);

        enemy.Location.Value = new Coord(4, 4);
        enemyTwo.Location.Value = new Coord(3, 3);

        world.AddCharacter(character);
        world.AddCharacter(characterTwo);

        //should return null when theres no hostiles
        Assert.IsNull(world.GetClosestHostile(character));
        Assert.IsNull(world.GetClosestHostile(characterTwo));

        world.AddCharacterAsEnemy(enemy);
        world.AddCharacterAsEnemy(enemyTwo);

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