using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;

[TestFixture]
public class MapTest
{
    Map map;

    [SetUp]
    public void Initialize()
    {
        map = new Map();
        map.Width = 3;
        map.Depth = 5;
        map.Initialize();
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
        character.SetLocation(0, 0);
        Assert.IsFalse(map.GetCell(0, 0).hasCharacter);
        Assert.IsNull(map.GetCell(0, 0).characterInTheCell);

        world.AddCharacter(character);
        Assert.IsTrue(map.GetCell(0, 0).hasCharacter);
        Assert.AreEqual(character, map.GetCell(0, 0).characterInTheCell);
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
}
