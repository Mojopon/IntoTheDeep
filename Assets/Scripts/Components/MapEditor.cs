using UnityEngine;
using System.Collections;
using System.IO;

public class MapEditor : MonoBehaviour
{
    public MapInstance mapInstancePrefab;

    private Map editingMap;

    public Map[] GetMaps(DungeonTitle dungeon, int levels)
    {
        return MapDataFileManager.ReadMapsFromFile(dungeon.ToString(), levels);
    }

    public Transform StartEditingMap(Map map)
    {
        this.editingMap = map;
        return InstantiateEditingMap(map);
    }

    public Transform InstantiateEditingMap(Map map)
    {
        var instance = Instantiate(mapInstancePrefab) as MapInstance;
        instance.Generate(map);
        return instance.transform;
    }
}
