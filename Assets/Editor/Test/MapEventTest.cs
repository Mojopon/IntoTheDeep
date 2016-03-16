using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;
using System.Linq;
using System.Collections.Generic;

[TestFixture]
public class MapEventTest
{
    private Map map;
    private List<Coord> positions;

    [SetUp]
    public void Initialize()
    {
        positions = new List<Coord>()
        {
            new Coord(1, 2),
            new Coord(2, 2),
            new Coord(3, 2),
            new Coord(4, 2),
        };

        map = MapFixtureFactory.Create(5, 5);
    }

    [Test]
    public void ShouldApplyAllMapEventsToTheMap()
    {
        var mapEvents = new MapEvents();
        mapEvents.startPositions = CreateStartPositionsFromPositionList(positions);

        mapEvents.Apply(map);

        int i = 0;
        foreach (var startPosition in map.playerStartPositions)
        {
            Assert.AreEqual(mapEvents.startPositions.positions[i], startPosition);
            i++;
        }
    }

    [Test]
    public void SetPlayerStartPositions()
    {
        var startPositions = CreateStartPositionsFromPositionList(positions);
       
        startPositions.Apply(map);
        int i = 0;
        foreach(var startPosition in map.playerStartPositions)
        {
            Assert.AreEqual(startPositions.positions[i], startPosition);
            i++;
        }
    }

    private StartPositions CreateStartPositionsFromPositionList(List<Coord> positions)
    {
        var startPositions = new StartPositions();

        foreach (var position in positions) startPositions.AddStartPosition(position);

        return startPositions;
    }
}
