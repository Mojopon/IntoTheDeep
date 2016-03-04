using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

public static class TileDataFileManager
{
    public static TileDatas ReadFromFiles()
    {
        var tileDirectory = ResourcePath.TILE_DATA_FOLDER_FULL_PATH;
        if(!Directory.Exists(tileDirectory))
            Directory.CreateDirectory(tileDirectory);

        string path = ResourcePath.TILE_ASSETS_PATH;
        string[] filePaths = Directory.GetFiles(path, "*.png");
        var fileNames = filePaths.Select(x => x.Replace(".png", "")).Select(x => Path.GetFileName(x)).ToArray();

        var tileDatas = new TileDatas();
        int i = 0;
        foreach(var name in fileNames)
        {
            TileData tileData = null;
            var filePath = GetFilePathFromFileName(name);
            if (!File.Exists(filePath))
            {
                tileData = new TileData();
                tileData.fileName = name;
                // create new TileData file
                WriteTileDataToTheFile(tileData);
            }
            else
            {
                tileData = ReadFromTheFile(filePath);
            }

            tileData.id = i;
            tileDatas.Add(tileData);

            i++;
        }

        return tileDatas;
    }

    private static TileData ReadFromTheFile(string filePath)
    {
        return ObjectSerializer.DeSerializeObject<TileData>(filePath);
    }

    public static void WriteToFiles(TileDatas tileDatas)
    {
        var tileDirectory = ResourcePath.TILE_DATA_FOLDER_FULL_PATH;

        foreach (var tileData in tileDatas.GetAllTiles())
        {
            WriteTileDataToTheFile(tileData);
        }
    }

    private static void WriteTileDataToTheFile(TileData tileData)
    {
        ObjectSerializer.SerializeObject(tileData, GetFilePathFromFileName(tileData.fileName));
    }

    private static string GetFilePathFromFileName(string tileFileName)
    {
        var tileDirectory = ResourcePath.TILE_DATA_FOLDER_FULL_PATH;
        return tileDirectory + tileFileName +  ".TileData" + ".txt";
    }
}
