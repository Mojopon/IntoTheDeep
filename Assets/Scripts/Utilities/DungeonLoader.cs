using UnityEngine;
using System.Collections;

public static class DungeonLoader
{
    public static Map[] GetMapsForTheDungeon(DungeonTitle title)
    {
        var levels = MapPatternFileManager.GetDungeonLevel(title);
        var maps = MapPatternFileManager.ReadFromFiles(title, levels);

        ApplyTileDataToMaps(maps);

        maps[0].playerStartPositions = new Coord[] { new Coord(1, 1), new Coord(2, 1), new Coord(3, 1), new Coord(4, 1), };
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
}
