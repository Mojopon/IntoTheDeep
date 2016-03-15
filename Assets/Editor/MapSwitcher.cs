using UnityEngine;
using System.Collections;
using UnityEditor;

public class MapSwitcher : EditorWindow
{
    private Map[] maps;
    private Map currentMap;
    private int currentMapNumber = 0;

    private MapInstance mapInstance;

    public MapSwitcher Init(MapInstance mapInstance, Map[] maps)
    {
        this.mapInstance = mapInstance;
        this.maps = maps;

        return this;
    }

    public Map GetCurrentMap()
    {
        if (currentMapNumber < 0)
        {
            currentMapNumber = 0;
        }
        else if (currentMapNumber >= maps.Length)
        {
            currentMapNumber = maps.Length - 1;
        }

        return maps[currentMapNumber];
    }

    public int GetCurrentMapNumber()
    {
        return currentMapNumber;
    }

    public Map[] GetMaps()
    {
        return maps;
    }

    public void DrawMapSwitchButtons()
    {
        if (maps == null) return;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Current Map: ", GUILayout.Width(110));
        if (GUILayout.Button("+", GUILayout.Width(30), GUILayout.Height(15)))
        {
            currentMapNumber++;
        }
        if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(15)))
        {
            currentMapNumber--;
        }
        currentMapNumber = EditorGUILayout.IntField(currentMapNumber);

        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        var nextMap = GetCurrentMap();
        if (nextMap != currentMap)
        {
            mapInstance.Generate(nextMap);
            currentMap = nextMap;
        }
    }

    public void Dispose()
    {
        if (mapInstance != null) DestroyImmediate(mapInstance.gameObject);
    }
}
