using UnityEngine;
using System.Collections;
using NUnit.Framework;

[TestFixture]
public class MapHelperTest
{
    Map map;

    [SetUp]
    public void Initialize()
    {
        map = MapFixtureFactory.Create(3, 5);
    }

    [Test]
    public void CanConvertToTextAndConvertBack()
    {
        var mapText = MapHelper.TilePatternToText(map.GetTilePattern());
        Assert.AreEqual("0,0,0\n0,0,0\n0,0,0\n0,0,0\n0,0,0", mapText);

        var mapPattern = MapHelper.TextToTilePattern(mapText);

        for(int y = 0; y < map.Depth; y++)
        {
            for(int x = 0; x < map.Width; x++)
            {
                Assert.AreEqual(map.GetCell(x, y).tileID, mapPattern[x, y]);
            }
        }
    }
}
