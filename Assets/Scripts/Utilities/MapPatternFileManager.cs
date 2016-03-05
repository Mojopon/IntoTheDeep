using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public static class MapPatternFileManager
{
    public static Map[] ReadFromFiles(DungeonTitle title, int levels)
    {
        return ReadFromFiles(title.ToString(), levels);
    }

    public static Map[] ReadFromFiles(string dungeonName, int levels)
    {
        if(!Directory.Exists(GetPathFromDungeonName(dungeonName)))
        {
            Directory.CreateDirectory(GetPathFromDungeonName(dungeonName));
        }

        List<Map> maps = new List<Map>();
        for (int i = 0; i < levels; i++)
        {
            var mapPattern = ReadFromTheFile(GetPathFromDungeonNameAndLevel(dungeonName, i), i);
            maps.Add(new Map(mapPattern));
        }

        return maps.ToArray();
    }

    private static int[,] ReadFromTheFile(string path, int level)
    {
        int[,] tilePattern;

        if(!File.Exists(path))
        {
            // create blank map if the file doesnt exists
            tilePattern = new int[5, 5];
            WriteMapPatternToTheFile(path, tilePattern);
            return tilePattern;
        }

        using (StreamReader sw = new StreamReader(path, Encoding.ASCII))
        {
            tilePattern = MapHelper.TextToTilePattern(sw.ReadToEnd());
        }

        return tilePattern;
    }

    public static void WriteToFiles(DungeonTitle title, Map[] maps)
    {
        WriteToFiles(title.ToString(), maps);
    }

    public static void WriteToFiles(string dungeonName, Map[] maps)
    {
        int levels = maps.Length;

        for(int i = 0; i < levels; i++)
        {
            WriteToTheFile(dungeonName, maps, i);
        }
    }

    public static void WriteToTheFile(string dungeonName, Map[] maps, int outputLevel)
    {
        if (outputLevel < 0 || outputLevel >= maps.Length) return; 

        var dungeonDataFolderPath = GetPathFromDungeonName(dungeonName);
        if (!Directory.Exists(dungeonDataFolderPath))
        {
            Directory.CreateDirectory(dungeonDataFolderPath);
        }

        WriteMapPatternToTheFile(GetPathFromDungeonNameAndLevel(dungeonName, outputLevel), maps[outputLevel].GetTilePattern());
    }

    private static void WriteMapPatternToTheFile(string path, int[,] tilePattern)
    {
        using (StreamWriter sw = new StreamWriter(path, false, Encoding.ASCII))
        {
            var text = MapHelper.TilePatternToText(tilePattern);
            sw.Write(text);
        }
    }

    public static int GetDungeonLevel(DungeonTitle title)
    {
        return GetFileCount(title.ToString());
    }

    private static int GetFileCount(string dungeonName)
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

    private static string GetPathFromDungeonName(string dungeonName)
    {
        return ResourcePath.RESOURCE_FOLDER + ResourcePath.DUNGEON_DATA_FOLDER + dungeonName + "/";
    }

    private static string GetPathFromDungeonNameAndLevel(string dungeonName, int level)
    {
        return ResourcePath.RESOURCE_FOLDER + ResourcePath.DUNGEON_DATA_FOLDER + dungeonName + "/" + level.ToString() + ".MapPattern" + ".txt";
    }

    private static void InitializeFolders()
    {
        if (!Directory.Exists(ResourcePath.RESOURCE_FOLDER))
        {
            Directory.CreateDirectory(ResourcePath.RESOURCE_FOLDER);
        }

        var dungeonDataFolderPath = ResourcePath.RESOURCE_FOLDER + ResourcePath.DUNGEON_DATA_FOLDER;
        if (!Directory.Exists(dungeonDataFolderPath))
        {
            Directory.CreateDirectory(dungeonDataFolderPath);
        }
    }
}
