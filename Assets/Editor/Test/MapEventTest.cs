using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;
using System.Linq;
using System.Collections.Generic;

[TestFixture]
public class MapEventTest
{
    Map map;
    [SetUp]
    public void Initialize()
    {
        var mapPattern = new int[5, 5];
        map = new Map(mapPattern);
    } 

    [Test]
    public void SetPlayerStartPositions()
    {
        var mapEvent = new StartPositionsMapEvent();
        var positions = new List<Coord>()
        {
            new Coord(1, 2),
            new Coord(2, 2),
            new Coord(3, 2),
            new Coord(4, 2),
        };

        foreach (var position in positions) mapEvent.AddStartPosition(position);

        mapEvent.Apply(map);
        int i = 0;
        foreach(var startPosition in map.playerStartPositions)
        {
            Assert.AreEqual(mapEvent.startPositions[i], startPosition);
            i++;
        }
    }
}
