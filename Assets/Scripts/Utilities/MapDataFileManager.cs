using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public static class MapDataFileManager
{
    private static readonly string RESOURCE_FOLDER = "Resources/";
    private static readonly string DUNGEON_DATA_FOLDER = "DungeonDatas/";

    public static Map[] ReadMapsFromFile(string dungeonName, int levels)
    {
        List<Map> maps = new List<Map>();
        for (int i = 0; i < levels; i++)
        {
            var mapPattern = ReadMapPatternFromFile(CreatePathFromDungeonNameAndLevel(dungeonName, i), i);
            maps.Add(new Map(mapPattern));
        }

        return maps.ToArray();
    }

    private static int[,] ReadMapPatternFromFile(string path, int level)
    {
        int[,] tilePattern;

        if(!File.Exists(path))
        {
            // create blank map if the file doesnt exists
            tilePattern = new int[5, 5];
            WriteMapPatternToFile(path, tilePattern);
            return tilePattern;
        }

        using (StreamReader sw = new StreamReader(path, Encoding.ASCII))
        {
            tilePattern = MapHelper.TextToTilePattern(sw.ReadToEnd());
        }

        return tilePattern;
    }

    public static void WriteMapsToFiles(string dungeonName, Map[] maps)
    {
        int levels = maps.Length;

        for(int i = 0; i < levels; i++)
        {
            WriteMapToFile(dungeonName, maps, i);
        }
    }

    public static void WriteMapToFile(string dungeonName, Map[] maps, int outputLevel)
    {
        if (outputLevel < 0 || outputLevel >= maps.Length) return; 

        var dungeonDataFolderPath = RESOURCE_FOLDER + DUNGEON_DATA_FOLDER + dungeonName;
        if (!Directory.Exists(dungeonDataFolderPath))
        {
            Directory.CreateDirectory(dungeonDataFolderPath);
        }

        WriteMapPatternToFile(CreatePathFromDungeonNameAndLevel(dungeonName, outputLevel), maps[outputLevel].GetTilePattern());
    }


    private static void WriteMapPatternToFile(string path, int[,] tilePattern)
    {
        using (StreamWriter sw = new StreamWriter(path, false, Encoding.ASCII))
        {
            var text = MapHelper.TilePatternToText(tilePattern);
            sw.Write(text);
        }
    }

    private static string CreatePathFromDungeonNameAndLevel(string dungeonName, int level)
    {
        return RESOURCE_FOLDER + DUNGEON_DATA_FOLDER + dungeonName + "/" + level.ToString() + ".MapPattern" + ".txt";
    }

    private static void InitializeFolders()
    {
        if (!Directory.Exists(RESOURCE_FOLDER))
        {
            Directory.CreateDirectory(RESOURCE_FOLDER);
        }

        var dungeonDataFolderPath = RESOURCE_FOLDER + DUNGEON_DATA_FOLDER;
        if (!Directory.Exists(dungeonDataFolderPath))
        {
            Directory.CreateDirectory(dungeonDataFolderPath);
        }
    }
}
