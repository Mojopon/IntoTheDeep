using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;

public class MapEventEditorWindow : EditorWindow
{
    private MapInstance mapInstance;
    private DungeonTitle selectedDungeon;
    private Map[] maps;
    private Map currentMap;
    private int currentMapNumber = 0;

    private EditEvent editMode;

    public enum EditEvent
    {
        StartPositions,
        EnemySpawns,
    }

    public static bool IsOpened { get; private set; }
    public static void ShowMainWindow(MapInstance mapInstance, DungeonTitle title, Map[] maps)
    {
        var window = (MapEventEditorWindow)EditorWindow.GetWindow(typeof(MapEventEditorWindow), false);
        window.SetMap(mapInstance, title, maps);
        IsOpened = true;
    }

    void SetMap(MapInstance mapInstance, DungeonTitle dungeonTitle, Map[] maps)
    {
        this.mapInstance = mapInstance;
        this.selectedDungeon = dungeonTitle;
        this.maps = maps;
    }

    void OnGUI()
    {
        if (maps == null || maps.Length == 0)
        {
            return;
        }

        // displaying map

        DrawUIForCurrentMapNumber();

        if (currentMapNumber < 0)
        {
            currentMapNumber = 0;
        }
        else if (currentMapNumber >= maps.Length)
        {
            currentMapNumber = maps.Length - 1;
        }

        var nextMap = maps[currentMapNumber];
        if (nextMap != currentMap)
        {
            mapInstance.Generate(nextMap);
            currentMap = nextMap;
        }

        // displaying map end

        DrawEditModeSelection();
    }

    void DrawUIForCurrentMapNumber()
    {
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
    }

    void OnDestroy()
    {
        if (mapInstance != null) DestroyImmediate(mapInstance.gameObject);
        IsOpened = false;
    }

    void DrawEditModeSelection()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Edit Mode: ", GUILayout.Width(110));
        editMode = (EditEvent)EditorGUILayout.EnumPopup(editMode);
        GUILayout.EndHorizontal();
    }
}
