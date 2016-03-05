using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.IO;
using System.Collections.Generic;

[TestFixture]
public class ObjectSerializerTest
{
    private static readonly string TILEDATA_FILE_FOR_TEST = "/tiledata.txt"; 
    private static readonly string STARTPOSITIONS_FILE_FOR_TEST = "/startpositions.txt";

    [Test]
    public void ShouldSerializeAndDeserializeBackTileData()
    {
        var tileData = new TileData(1);
        tileData.fileName = "tileSprite";
        tileData.canWalk = false;
        tileData.isExit = true;
        Assert.AreEqual(1, tileData.id);

        var path = Directory.GetCurrentDirectory() + TILEDATA_FILE_FOR_TEST;

        ObjectSerializer.SerializeObject(tileData, path);

        var loadedTileData = ObjectSerializer.DeSerializeObject<TileData>(path);
        Assert.IsNotNull(loadedTileData);

        Assert.AreEqual(tileData.canWalk, loadedTileData.canWalk);
        Assert.AreEqual(tileData.fileName, loadedTileData.fileName);
        Assert.AreEqual(tileData.isExit, loadedTileData.isExit);
        // id will not be serialized
        Assert.AreEqual(0, loadedTileData.id);

    }

    [Test]
    public void ShouldSerializeAndDeserializeBackStartPositionsMapEvent()
    {

        var mapEvent = new StartPositionsMapEvent();
        mapEvent.AddStartPosition(new Coord(1, 1));
        mapEvent.AddStartPosition(new Coord(2, 1));
        mapEvent.AddStartPosition(new Coord(3, 1));
        mapEvent.AddStartPosition(new Coord(4, 1));

        var path = Directory.GetCurrentDirectory() + STARTPOSITIONS_FILE_FOR_TEST;

        ObjectSerializer.SerializeObject(mapEvent, path);

        var loadedMapEvent = ObjectSerializer.DeSerializeObject<StartPositionsMapEvent>(path);
        Assert.IsNotNull(loadedMapEvent);

        for(int i = 0; i < mapEvent.startPositions.Count; i++)
        {
            Assert.AreEqual(mapEvent.startPositions[i], loadedMapEvent.startPositions[i]);
        }
    }

    [TearDown]
    public void Destroy()
    {
        List<string> filesToDelete = new List<string>();
        filesToDelete.Add(Directory.GetCurrentDirectory() + TILEDATA_FILE_FOR_TEST);
        filesToDelete.Add(Directory.GetCurrentDirectory() + STARTPOSITIONS_FILE_FOR_TEST);

        foreach (var path in filesToDelete) File.Delete(path);
    }
}
