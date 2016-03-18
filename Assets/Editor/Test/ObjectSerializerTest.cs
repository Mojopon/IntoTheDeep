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
    private static readonly string CHARACTER_DATA_FILE_FOR_TEST = "/characterdatas.txt";

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

        var mapEvent = new StartPositions();
        mapEvent.AddStartPosition(new Coord(1, 1));
        mapEvent.AddStartPosition(new Coord(2, 1));
        mapEvent.AddStartPosition(new Coord(3, 1));
        mapEvent.AddStartPosition(new Coord(4, 1));

        var path = Directory.GetCurrentDirectory() + STARTPOSITIONS_FILE_FOR_TEST;

        ObjectSerializer.SerializeObject(mapEvent, path);

        var loadedMapEvent = ObjectSerializer.DeSerializeObject<StartPositions>(path);
        Assert.IsNotNull(loadedMapEvent);

        for(int i = 0; i < mapEvent.positions.Count; i++)
        {
            Assert.AreEqual(mapEvent.positions[i], loadedMapEvent.positions[i]);
        }
    }

    [Test]
    public void ShouldSerializeAndDeserializeSerializableCharacterData()
    {
        var characterData = new SerializableCharacterData();
        characterData.name = "Player";
        characterData.level = 5;
        characterData.expToNextLevel = 250;

        var path = Directory.GetCurrentDirectory() + CHARACTER_DATA_FILE_FOR_TEST;
        ObjectSerializer.SerializeObject(characterData, path);

        var loadedCharacterData = ObjectSerializer.DeSerializeObject<SerializableCharacterData>(path);
        Assert.IsNotNull(loadedCharacterData);

        Assert.AreEqual(characterData.name, loadedCharacterData.name);
        Assert.AreEqual(characterData.level, loadedCharacterData.level);
        Assert.AreEqual(characterData.expToNextLevel, loadedCharacterData.expToNextLevel);
    }


    [TearDown]
    public void Destroy()
    {
        List<string> filesToDelete = new List<string>();
        filesToDelete.Add(Directory.GetCurrentDirectory() + TILEDATA_FILE_FOR_TEST);
        filesToDelete.Add(Directory.GetCurrentDirectory() + STARTPOSITIONS_FILE_FOR_TEST);
        filesToDelete.Add(Directory.GetCurrentDirectory() + CHARACTER_DATA_FILE_FOR_TEST);

        foreach (var path in filesToDelete) File.Delete(path);
    }
}
