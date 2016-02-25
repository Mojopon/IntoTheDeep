using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;
using System;

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
            playerStartPositions = new Coord[]
                                   {
                                       new Coord (0, 0),
                                       new Coord (1, 0),
                                       new Coord (2, 0),
                                       new Coord (2, 1),
                                   }
        };

        maps[1] = new Map()
        {
            Width = 4,
            Depth = 4,
            exitLocation = new Coord(3, 3),
            playerStartPositions = new Coord[]
                                   {
                                       new Coord (1, 1),
                                       new Coord (1, 0),
                                       new Coord (1, 2),
                                       new Coord (2, 2),
                                   }
        };

        maps[2] = new Map()
        {
            Width = 5,
            Depth = 5,
            exitLocation = new Coord(4, 4),
            playerStartPositions = new Coord[]
                                   {
                                       new Coord (1, 2),
                                       new Coord (1, 1),
                                       new Coord (0, 2),
                                       new Coord (0, 1),
                                   }
        };

        foreach(var map in maps)
        {
            map.Initialize();
        }

        var characterData = new CharacterDataTable();

        this.transition = new TransitionWorld(maps);
        transition.AddPlayer(characterData);
    }
    
    [Test]
    public void ShouldAddCharacterTotheWorldInThePlayerStartPosition()
    {
        var nextWorld = transition.GoNext();
        var currentMap = maps[0];

        nextWorld.CurrentActor.Subscribe(x => playerOne = x);
        nextWorld.GoNextCharacterPhase();
        Assert.AreEqual(currentMap.playerStartPositions[0], playerOne.Location.Value);
        Assert.AreEqual(currentMap.GetCharacter(playerOne.Location.Value), playerOne);
    }

    [Test]
    public void ShouldTransitionToNextMap()
    {
        var nextWorld = transition.GoNext();
        var currentMap = maps[0];

        nextWorld.CurrentActor.Subscribe(x => playerOne = x).AddTo(nextWorld.Disposables);
        nextWorld.GoNextCharacterPhase();

        Assert.IsFalse(playerOne.CanMoveTo(Direction.Left));
        Assert.IsFalse(playerOne.CanMoveTo(Direction.Down));
        Assert.IsTrue(playerOne.CanMoveTo(Direction.Up));
        Assert.IsTrue(playerOne.CanMoveTo(Direction.Right));

        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Right);
        Assert.IsFalse(playerOne.IsOnExit);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Right);
        Assert.IsFalse(playerOne.IsOnExit);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Up);
        Assert.IsFalse(playerOne.IsOnExit);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Up);
        Assert.IsTrue(playerOne.IsOnExit);

        nextWorld = transition.GoNext();
        currentMap = maps[1];

        var playerInThePreviousMap = playerOne;
        nextWorld.CurrentActor.Subscribe(x => playerOne = x).AddTo(nextWorld.Disposables);
        nextWorld.GoNextCharacterPhase();
        Assert.AreNotEqual(playerOne, playerInThePreviousMap);
        Assert.IsFalse(playerOne.IsOnExit);

        Assert.AreEqual(currentMap.playerStartPositions[0], playerOne.Location.Value);
        Assert.AreEqual(currentMap.GetCharacter(playerOne.Location.Value), playerOne);

        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Right);
        Assert.IsFalse(playerOne.IsOnExit);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Right);
        Assert.IsFalse(playerOne.IsOnExit);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Up);
        Assert.IsFalse(playerOne.IsOnExit);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Up);
        Assert.IsTrue(playerOne.IsOnExit);

        nextWorld = transition.GoNext();
        currentMap = maps[2];
        nextWorld.CurrentActor.Subscribe(x => playerOne = x).AddTo(nextWorld.Disposables);
        nextWorld.GoNextCharacterPhase();
        Assert.AreNotEqual(playerOne, playerInThePreviousMap);
        Assert.IsFalse(playerOne.IsOnExit);

        Assert.AreEqual(currentMap.playerStartPositions[0], playerOne.Location.Value);
        Assert.AreEqual(currentMap.GetCharacter(playerOne.Location.Value), playerOne);

        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Up);
        Assert.IsFalse(playerOne.IsOnExit);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Up);
        Assert.IsFalse(playerOne.IsOnExit);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Right);
        Assert.IsFalse(playerOne.IsOnExit);
        CheckIfMoveAppliedToTheMap(currentMap, playerOne, Direction.Right);
        Assert.IsFalse(playerOne.IsOnExit);
        // player moved 4 times so we need to roll the character phase to keep moving
        nextWorld.GoNextCharacterPhase();
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
