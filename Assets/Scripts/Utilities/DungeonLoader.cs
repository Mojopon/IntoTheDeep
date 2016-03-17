using UnityEngine;
using System.Collections;

public static class DungeonLoader
{
    public static Map[] GetMapsForTheDungeon(DungeonTitle title)
    {
        var levels = MapPatternFileManager.GetDungeonLevel(title);
        var maps = MapPatternFileManager.ReadFromFiles(title, levels);

        ApplyTileDataToMaps(maps);
        ApplyMapEventsToMaps(title, levels, maps);
        return maps;
    }

    private static void ApplyTileDataToMaps(Map[] maps)
    {
        var tileDatas = TileDataFileManager.ReadFromFiles();

        foreach(var map in maps)
        {
            map.ApplyTileData(tileDatas);
        }
    }

    private static void ApplyMapEventsToMaps(DungeonTitle title, int levels, Map[] maps)
    {
        var mapEvents = MapEventFileManager.ReadFromFiles(title, levels);

        if(maps.Length > mapEvents.Length)
        {
            throw new System.Exception("couldnt find MapEvents for the maps");
        }

        for(int i = 0; i < maps.Length; i++)
        {
            mapEvents[i].Apply(maps[i]);
        }
    }
}
