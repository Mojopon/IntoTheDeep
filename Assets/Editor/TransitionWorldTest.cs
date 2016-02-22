﻿using UnityEngine;
using System.Collections;
using NUnit.Framework;

[TestFixture]
public class TransitionWorldTest
{
    private TransitionWorld transition;
    private Map[] maps;
    private Character playerOne;

    [SetUp]
    public void Initialize()
    {
        this.maps = new Map[3];
        maps[0] = new Map()
        {
            Width = 3,
            Depth = 3,
            exitLocation = new Coord(2, 2),
            playerStartPosition = new Coord(0, 0),
        };

        maps[1] = new Map()
        {
            Width = 4,
            Depth = 4,
            exitLocation = new Coord(3, 3),
            playerStartPosition = new Coord(1, 1),
        };

        maps[2] = new Map()
        {
            Width = 5,
            Depth = 5,
            exitLocation = new Coord(4, 4),
            playerStartPosition = new Coord(1, 2),
        };

        foreach(var map in maps)
        {
            map.Initialize();
        }

        playerOne = Character.Create();
        playerOne.GodMode = true;
        playerOne.SetPhase(Character.Phase.Move);

        this.transition = new TransitionWorld(maps);
        transition.AddPlayer(playerOne);
    }
    
    [Test]
    public void ShouldAddCharacterTotheWorldInThePlayerStartPosition()
    {
        var nextWorld = transition.GoNext();
        var currentMap = maps[0];
        Assert.AreEqual(currentMap.playerStartPosition, playerOne.Location.Value);
        Assert.AreEqual(currentMap.GetCharacter(playerOne.Location.Value), playerOne);
    }

    [Test]
    public void ShouldTransitionToNextMap()
    {
        var nextWorld = transition.GoNext();
        var currentMap = maps[0];

        Assert.IsFalse(playerOne.CanMoveTo(Direction.Left));
        Assert.IsFalse(playerOne.CanMoveTo(Direction.Down));
        Assert.IsTrue(playerOne.CanMoveTo(Direction.Up));
        Assert.IsTrue(playerOne.CanMoveTo(Direction.Right));

        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Right);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Right);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Up);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Up);

        Assert.IsTrue(playerOne.IsOnExit);

        nextWorld = transition.GoNext();
        currentMap = maps[1];

        Assert.AreEqual(currentMap.playerStartPosition, playerOne.Location.Value);
        Assert.AreEqual(currentMap.GetCharacter(playerOne.Location.Value), playerOne);

        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Right);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Right);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Up);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Up);
        Assert.IsTrue(playerOne.IsOnExit);

        nextWorld = transition.GoNext();
        currentMap = maps[2];

        Assert.AreEqual(currentMap.playerStartPosition, playerOne.Location.Value);
        Assert.AreEqual(currentMap.GetCharacter(playerOne.Location.Value), playerOne);

        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Up);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Up);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Right);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Right);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Right);
        Assert.IsTrue(playerOne.IsOnExit);
    }

    void CheckIfMoveAppliedToTheMap(Map map, Character character, Direction direction)
    {
        Assert.AreEqual(map.GetCharacter(playerOne.Location.Value), playerOne);
        Assert.IsTrue(character.Move(direction));
        Assert.AreEqual(map.GetCharacter(playerOne.Location.Value), playerOne);
        var previousLocation = character.Location.Value + direction.GetOpposide().ToCoord();
        Assert.IsNull(map.GetCharacter(previousLocation));
    }
}