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
        var character = new Character();
        character.Location.Value = new Coord(0, 0);

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
        var characterTwo = new Character();
        // returns false when occupied by another character and true when its a same character
        Assert.IsTrue(map.CanWalk(0, 0, character));
        Assert.IsFalse(map.CanWalk(0, 0, characterTwo));
    }
}
