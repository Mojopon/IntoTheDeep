﻿using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;
using System.Linq;

[TestFixture]
public class MapTest
{
    Map map;

    [SetUp]
    public void Initialize()
    {
        map = MapFixtureFactory.Create(3, 5);
    }

    [Test]
    public void CanCreateFromMapPattern()
    {
        
        var mapPatternRaw = new int[,]
        {
            { 1,1,1,1, },
            { 0,0,0,1, },
            { 0,0,0,1, },
        };

        var mapPattern = MapFixtureFactory.GetRotatedMapPattern(mapPatternRaw);

        var map = MapFixtureFactory.CreateFromPattern(mapPattern);
        for (int y = 0; y < map.Depth; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                Assert.AreEqual(mapPattern[x, y], map.GetCell(x, y).tileID);
            }
        }
    }

    [Test]
    public void ShouldApplyTileDatas()
    {
        TileDatas tileDatas = new TileDatas();
        var tileOne = new TileData(0);
        var tileTwo = new TileData(1) { canWalk = false, };
        var tileThree = new TileData(2) { isExit = true, };
        tileDatas.Add(tileOne, tileTwo, tileThree);

        var mapPatternRaw = new int[,]
        {
            { 1,1,2,1, },
            { 0,0,0,1, },
            { 0,0,0,1, },
        };

        var mapPattern = MapFixtureFactory.GetRotatedMapPattern(mapPatternRaw);

        map = MapFixtureFactory.CreateFromPattern(mapPattern);
        for (int y = 0; y < mapPattern.GetLength(1); y++)
        {
            for (int x = 0; x < mapPattern.GetLength(0); x++)
            {
                Assert.IsTrue(map.GetCell(x, y).canWalk);
            }
        }

        map.ApplyTileData(tileDatas);

        for (int y = 0; y < mapPattern.GetLength(1); y++)
        {
            for (int x = 0; x < mapPattern.GetLength(0); x++)
            {
                if(map.GetCell(x, y).tileID == 0)
                {
                    Assert.IsTrue(map.GetCell(x, y).canWalk);
                    Assert.IsFalse(map.GetCell(x, y).isExit);

                }
                else if (map.GetCell(x, y).tileID == 1)
                {
                    Assert.IsFalse(map.GetCell(x, y).canWalk);
                    Assert.IsFalse(map.GetCell(x, y).isExit);
                }
                else if (map.GetCell(x, y).tileID == 2)
                {
                    Assert.IsTrue(map.GetCell(x, y).canWalk);
                    Assert.IsTrue(map.GetCell(x, y).isExit);
                }
                else
                {
                    Assert.Fail();
                }
            }
        }
    }

    [Test]
    public void ShouldIncreaseAndDecreaseSize()
    {
        int previousWidth = map.Width;
        int previousDepth = map.Depth;

        map.SetTileID(2, 2, 3);

        var oldTilePattern = map.GetTilePattern();
        map.IncreaseMapWidth();
        Assert.AreEqual(previousWidth + 1, map.Width);
        Assert.AreEqual(previousDepth, map.Depth);

        for (int y = 0; y < map.Depth; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                var cell = map.GetCell(x, y);

                if (x >= oldTilePattern.GetLength(0) || y >= oldTilePattern.GetLength(1)) continue;
                Assert.AreEqual(cell.tileID, oldTilePattern[x, y]);
            }
        }

        previousWidth = map.Width;
        oldTilePattern = map.GetTilePattern();
        map.DecreaseMapWidth();
        Assert.AreEqual(previousWidth - 1, map.Width);
        Assert.AreEqual(previousDepth, map.Depth);
        for (int y = 0; y < map.Depth; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                var cell = map.GetCell(x, y);

                if (x >= oldTilePattern.GetLength(0) || y >= oldTilePattern.GetLength(1)) continue;
                Assert.AreEqual(cell.tileID, oldTilePattern[x, y]);
            }
        }

        previousWidth = map.Width;
        oldTilePattern = map.GetTilePattern();
        map.IncreaseMapDepth();
        Assert.AreEqual(previousWidth, map.Width);
        Assert.AreEqual(previousDepth + 1, map.Depth);

        for (int y = 0; y < map.Depth; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                var cell = map.GetCell(x, y);

                if (x >= oldTilePattern.GetLength(0) || y >= oldTilePattern.GetLength(1)) continue;
                Assert.AreEqual(cell.tileID, oldTilePattern[x, y]);
            }
        }

        previousDepth = map.Depth;
        oldTilePattern = map.GetTilePattern();
        map.DecreaseMapDepth();
        Assert.AreEqual(previousWidth, map.Width);
        Assert.AreEqual(previousDepth - 1, map.Depth);

        for (int y = 0; y < map.Depth; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                var cell = map.GetCell(x, y);

                if (x >= oldTilePattern.GetLength(0) || y >= oldTilePattern.GetLength(1)) continue;
                Assert.AreEqual(cell.tileID, oldTilePattern[x, y]);
            }
        }
    }


    [Test]
    public void CheckIfTheCharacterCanMoveToTheCell()
    {
        var character = Character.Create();
        character.SetLocation(0, 0);

        for(int y = 0; y < map.Depth; y++)
        {
            for(int x = 0; x < map.Width; x++)
            {
                Assert.IsTrue(map.CanWalk(x, y, character));
            }
        }

        map.GetCell(1, 1).canWalk = false;
        Assert.IsFalse(map.CanWalk(1, 1, character));

        map.SetCharacter(character);
        var characterTwo = Character.Create();
        // returns false when occupied by another character and true when its a same character
        Assert.IsTrue(map.CanWalk(0, 0, character));
        Assert.IsFalse(map.CanWalk(0, 0, characterTwo));
    }

    [Test]
    public void ShouldAddCharacterToTheCell()
    {
        var character = Character.Create();
        character.SetLocation(0, 0);
        Assert.IsFalse(map.GetCell(0, 0).hasCharacter);
        Assert.IsNull(map.GetCell(0, 0).characterInTheCell);

        map.SetCharacter(character);
        Assert.IsTrue(map.GetCell(0, 0).hasCharacter);
        Assert.AreEqual(character, map.GetCell(0, 0).characterInTheCell);

        map.MoveCharacterToFrom(character, character.Location.Value, new Coord(1, 0));
        Assert.IsFalse(map.GetCell(0, 0).hasCharacter);
        Assert.IsNull(map.GetCell(0, 0).characterInTheCell);
        Assert.IsTrue(map.GetCell(1, 0).hasCharacter);
        Assert.AreEqual(character, map.GetCell(1, 0).characterInTheCell);
    }
    
    [Test]
    public void ShouldSetAddedCharacterToTheWorld()
    {
        var world = new World(map);
        var character = Character.Create();
        Assert.IsFalse(map.GetCell(0, 0).hasCharacter);
        Assert.IsNull(map.GetCell(0, 0).characterInTheCell);

        world.AddCharacter(character, 0, 0);
        Assert.IsTrue(map.GetCell(0, 0).hasCharacter);
        Assert.AreEqual(character, map.GetCell(0, 0).characterInTheCell);
    }

    [Test]
    public void ShouldBeIsOnExitTrueWhenCharacterIsOnExit()
    {
        map.GetCell(2, 4).isExit = true;
        var world = new World(map);
        var character = Character.Create();

        world.AddCharacter(character, 2, 2);
        character.SetPhase(Character.Phase.Move);
        Assert.IsFalse(character.IsOnExit);
        Assert.IsTrue(character.Move(Direction.Up));
        Assert.IsFalse(character.IsOnExit);
        Assert.IsTrue(character.Move(Direction.Up));
        Assert.IsTrue(character.IsOnExit);
        Assert.IsTrue(character.Move(Direction.Down));
        Assert.IsFalse(character.IsOnExit);
    }

    [Test]
    public void ShouldGetAvailableCells()
    {
        var world = new World(map);
        var character = Character.Create();

        world.AddCharacter(character, 2, 2);
        for(int y = 0; y < map.Depth; y++)
        {
            for(int x = 0; x < map.Width; x++)
            {
                if (x == 2 && y == 2) continue;
                Assert.IsTrue(map.GetCell(x, y).IsAvailable());
            }
        }

        Assert.IsFalse(map.GetCell(2, 2).IsAvailable());

        var availableCells = map.GetAvailableCells();

        for (int y = 0; y < map.Depth; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                if (x == 2 && y == 2) continue;
                Assert.IsTrue(availableCells.Contains(map.GetCell(x, y)));
            }
        }

        Assert.IsFalse(availableCells.Contains(map.GetCell(2, 2)));
    }

    [Test]
    public void ShouldGetCharacterInTheCell()
    {
        var character = Character.Create();
        character.SetLocation(1, 0);
        map.SetCharacter(character);

        Assert.IsFalse(map.GetCell(0, 0).hasCharacter);
        Assert.IsNull(map.GetCell(0, 0).characterInTheCell);
        Assert.IsTrue(map.GetCell(1, 0).hasCharacter);
        Assert.AreEqual(character, map.GetCell(1, 0).characterInTheCell);

        // should return null when no character found in the cell
        Assert.IsNull(map.GetCharacter(new Coord(0, 0)));
        Assert.AreEqual(character, map.GetCharacter(new Coord(1, 0)));
    }

    [Test]
    public void ShouldRemoveCharacter()
    {
        var character = Character.Create();
        Assert.IsFalse(map.GetCell(0, 0).hasCharacter);
        Assert.IsNull(map.GetCell(0, 0).characterInTheCell);

        map.SetCharacter(character);
        Assert.IsTrue(map.GetCell(0, 0).hasCharacter);
        Assert.AreEqual(character, map.GetCell(0, 0).characterInTheCell);

        var characterTwo = Character.Create();
        //ignore if a wrong character is given
        map.RemoveCharacter(characterTwo, new Coord(0, 0));
        Assert.IsTrue(map.GetCell(0, 0).hasCharacter);
        Assert.AreEqual(character, map.GetCell(0, 0).characterInTheCell);

        map.RemoveCharacter(character, new Coord(0, 0));
        Assert.IsFalse(map.GetCell(0, 0).hasCharacter);
        Assert.IsNull(map.GetCell(0, 0).characterInTheCell);
    }

    [Test]
    public void ShouldNotifyCellChange()
    {
        Coord changedCellCoord = new Coord(-1, -1);

        map.CellChangeObservable.Subscribe(x => changedCellCoord = x);
        map.SetTileID(2, 3, 10);

        Assert.AreEqual(new Coord(2, 3), changedCellCoord);
    }

    [Test]
    public void ShouldSetLocationForEveryCells()
    {
        for(int y = 0; y < map.Depth; y++)
        {
            for(int x = 0; x < map.Width; x++)
            {
                Assert.AreEqual(map.GetCell(x, y).x, x);
                Assert.AreEqual(map.GetCell(x, y).y, y);
            }
        }
    }

    [Test]
    public void ShouldReturnTilePattern()
    {
        map.SetTileID(2, 2, 3);
        var tilePattern = map.GetTilePattern();

        for(int y = 0; y < map.Depth; y++)
        {
            for(int x = 0; x < map.Width; x++)
            {
                if (x == 2 && y == 2) continue;
                Assert.AreEqual(0, tilePattern[x, y]);
            }
        }

        Assert.AreEqual(3, tilePattern[2, 2]);
    }
}
