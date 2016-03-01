using UnityEngine;
using System.Collections;
using System.IO;

public class MapEditor : MonoBehaviour
{
    public int editingMap;
    public MapInstance mapInstancePrefab;
    [HideInInspector]
    public Map[] maps;

    public Map[] GetMaps()
    {
        foreach(var map in maps)
        {
            map.Initialize();
        }

        return maps;
    }

    public bool CanInstantiateEditingMap()
    {
        if (editingMap < 0 || editingMap >= maps.Length) return false;

        return true;
    }

    public Transform InstantiateEditingMap()
    {
        var map = maps[editingMap];
        map.Initialize();
        var instance = Instantiate(mapInstancePrefab) as MapInstance;
        instance.Generate(map);
        return instance.transform;
    }
}
