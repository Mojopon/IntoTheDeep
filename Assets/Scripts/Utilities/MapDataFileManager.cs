using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public static class MapDataFileManager
{
    private static readonly string RESOURCE_FOLDER = "Resources/";
    private static readonly string DUNGEON_DATA_FOLDER = "DungeonDatas/";

    public static Map[] ReadMapsFromFile(DungeonTitle title, int levels)
    {
        return ReadMapsFromFile(title.ToString(), levels);
    }

    public static Map[] ReadMapsFromFile(string dungeonName, int levels)
    {
        if(!Directory.Exists(GetPathFromDungeonName(dungeonName)))
        {
            Directory.CreateDirectory(GetPathFromDungeonName(dungeonName));
        }

        List<Map> maps = new List<Map>();
        for (int i = 0; i < levels; i++)
        {
            var mapPattern = ReadMapPatternFromFile(GetPathFromDungeonNameAndLevel(dungeonName, i), i);
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

    public static void WriteMapsToFiles(DungeonTitle title, Map[] maps)
    {
        WriteMapsToFiles(title.ToString(), maps);
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

        WriteMapPatternToFile(GetPathFromDungeonNameAndLevel(dungeonName, outputLevel), maps[outputLevel].GetTilePattern());
    }

    public static int GetAllMapPatternFileCount(DungeonTitle title)
    {
        return GetAllMapPatternFileCount(title.ToString());
    }

    private static int GetAllMapPatternFileCount(string dungeonName)
    {
        var mapPatternFileNames = GetAllMapPatternFiles(dungeonName);

        if (mapPatternFileNames == null) return 0;

        return GetAllMapPatternFiles(dungeonName).Length;
    }

    public static string[] GetAllMapPatternFiles(DungeonTitle title)
    {
        return GetAllMapPatternFiles(title.ToString());
    }

    public static string[] GetAllMapPatternFiles(string dungeonName)
    {
        var mapPatternFileNames = new List<string>();
        int i = 0;
        string nextMapFile = GetPathFromDungeonNameAndLevel(dungeonName, i);
        if (!File.Exists(nextMapFile))
        {
            return null;
        }

        while(!string.IsNullOrEmpty(nextMapFile))
        {
            mapPatternFileNames.Add(nextMapFile);
            i++;

            nextMapFile = GetPathFromDungeonNameAndLevel(dungeonName, i);
            if(!File.Exists(nextMapFile))
            {
                nextMapFile = null;
            }
        }

        return mapPatternFileNames.ToArray();
    }

    private static void WriteMapPatternToFile(string path, int[,] tilePattern)
    {
        using (StreamWriter sw = new StreamWriter(path, false, Encoding.ASCII))
        {
            var text = MapHelper.TilePatternToText(tilePattern);
            sw.Write(text);
        }
    }

    private static string GetPathFromDungeonName(string dungeonName)
    {
        return RESOURCE_FOLDER + DUNGEON_DATA_FOLDER + dungeonName + "/";
    }

    private static string GetPathFromDungeonNameAndLevel(string dungeonName, int level)
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
