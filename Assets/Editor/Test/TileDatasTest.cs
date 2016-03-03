using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;
using System.Linq;

[TestFixture]
public class TileDatasTest
{
    TileDatas tileDatas;
    TileData tileOne;
    TileData tileTwo;

    [SetUp]
    public void Setup()
    {
        tileDatas = new TileDatas();
        tileOne = new TileData(0);
        tileTwo = new TileData(1);

        tileDatas.Add(tileOne);
        tileDatas.Add(tileTwo);
    }

    [Test]
    public void ShouldReturnTileData()
    {
        var tile = tileDatas.Get(0);
        Assert.AreEqual(tileOne, tile);

        tile = tileDatas.Get(1);
        Assert.AreEqual(tileTwo, tile);

        // should return null when it couldnt find a tile data
        tile = tileDatas.Get(2);
        Assert.IsNull(tile);
    }

    [Test]
    public void TileDataInitialParameters()
    {
        var tileData = new TileData(0);
        Assert.IsTrue(tileData.canWalk);
    }
}
