using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class MapEventFileManager
{
    public static void WriteToFiles(MapEvents[] mapEventsArray, DungeonTitle dungeonTitle)
    {
        string dungeonName = dungeonTitle.ToString();
        for (int i = 0; i < mapEventsArray.Length; i++)
        {
            var mapEvents = mapEventsArray[i];
            WriteStartPositionsToTheFile(mapEvents.startPositions, dungeonName, i);
        }
    }

    private static void WriteStartPositionsToTheFile(StartPositions startPositions, string dungeonName, int targetLevel)
    {
        ObjectSerializer.SerializeObject(startPositions, GetStartPositionMapEventPath(dungeonName, targetLevel));
    }

    public static MapEvents[] ReadFromFiles(DungeonTitle dungeonTitle, int levels)
    {
        return ReadFromFiles(dungeonTitle.ToString(), levels);
    }

    public static MapEvents[] ReadFromFiles(string dungeonName, int levels)
    {
        List<MapEvents> mapEventsList = new List<MapEvents>();

        if (!Directory.Exists(ResourcePath.GetDungeonFolderFromDungeonName(dungeonName)))
        {
            throw new System.Exception("Couldnt find dungeon folder!");
        }

        for (int i = 0; i < levels; i++)
        {
            var mapEvents = new MapEvents();
            CheckIfFilesExists(dungeonName, i);
            mapEvents.startPositions = ReadStartPositionMapEventFromFile(dungeonName, i);

            mapEventsList.Add(mapEvents);
        }

        return mapEventsList.ToArray();
    }

    private static StartPositions ReadStartPositionMapEventFromFile(string dungeonName, int targetLevel)
    {
        return ObjectSerializer.DeSerializeObject<StartPositions>(GetStartPositionMapEventPath(dungeonName, targetLevel));
    }

    private static void CheckIfFilesExists(string dungeonName, int targetLevel)
    {
        if (!File.Exists(GetStartPositionMapEventPath(dungeonName, targetLevel)))
        {
            WriteStartPositionsToTheFile(StartPositions.Create(), dungeonName, targetLevel);
        }
    }

    private static string GetStartPositionMapEventPath(string dungeonName, int targetLevel)
    {
        return ResourcePath.GetDungeonLevelFolder(dungeonName, targetLevel) + "StartPositions" + ".txt";
    }
}
