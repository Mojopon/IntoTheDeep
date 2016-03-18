using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class ResourcePath
{
    public static readonly string RESOURCE_FOLDER = "Resources/";

    public static readonly string DUNGEON_DATA_FOLDER = "DungeonDatas/";
    public static readonly string DUNGEON_DATA_FOLDER_FULL_PATH = RESOURCE_FOLDER + DUNGEON_DATA_FOLDER;

    public static readonly string TILE_DATA_FOLDER = "TileDatas/";
    public static readonly string TILE_DATA_FOLDER_FULL_PATH = RESOURCE_FOLDER + TILE_DATA_FOLDER;

    public static readonly string CHARACTER_DATA_FOLDER = "CharacterDatas/";
    public static readonly string CHARACTER_DATA_FOLDER_FULL_PATH = RESOURCE_FOLDER + CHARACTER_DATA_FOLDER;

    public static readonly string MAPTILE_FOLDER_NAME = "MapTiles";
    public static readonly string TILE_ASSETS_PATH = "Assets/Resources/" + MAPTILE_FOLDER_NAME;

    public static void CheckAllDirectories()
    {
        var foldersToCheck = new List<string>()
        {
            RESOURCE_FOLDER,
            DUNGEON_DATA_FOLDER_FULL_PATH,
            TILE_DATA_FOLDER_FULL_PATH,
            CHARACTER_DATA_FOLDER_FULL_PATH,
        };

        foreach(var path in foldersToCheck)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }

    public static string GetDungeonFolderFromDungeonName(string dungeonName)
    {
        return RESOURCE_FOLDER + DUNGEON_DATA_FOLDER + dungeonName + "/";
    }

    public static string GetDungeonLevelFolder(string dungeonName, int targetLevel)
    {
        return RESOURCE_FOLDER + DUNGEON_DATA_FOLDER + dungeonName + "/" + targetLevel + "/";
    }
}
