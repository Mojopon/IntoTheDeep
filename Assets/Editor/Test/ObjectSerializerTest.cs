using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.IO;

[TestFixture]
public class ObjectSerializerTest
{
    private static readonly string TILEDATA_FILE_FOR_TEST = "tiledata.txt"; 

	[Test]
    public void ShouldSerializeAndDeserializeBackTileData()
    {
        var tileData = new TileData(1);
        tileData.fileName = "tileSprite";
        tileData.canWalk = false;
        Assert.AreEqual(1, tileData.id);

        var path = Directory.GetCurrentDirectory() + "/" + TILEDATA_FILE_FOR_TEST;

        ObjectSerializer.SerializeObject(tileData, path);

        var loadedTileData = ObjectSerializer.DeSerializeObject<TileData>(path);
        Assert.IsNotNull(loadedTileData);

        Assert.AreEqual(tileData.canWalk, loadedTileData.canWalk);
        Assert.AreEqual(tileData.fileName, loadedTileData.fileName);
        // id will not be serialized
        Assert.AreEqual(0, loadedTileData.id);

    }

    [TearDown]
    public void Destroy()
    {
        var tileDataPath = Directory.GetCurrentDirectory() + "/" + TILEDATA_FILE_FOR_TEST;

        File.Delete(tileDataPath);
    }
}
