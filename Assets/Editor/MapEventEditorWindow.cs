using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;

public class MapEventEditorWindow : EditorWindow
{
    private MapInstance mapInstance;
    private DungeonTitle selectedDungeon;
    private Map currentMap;

    private MapSwitcher mapSwitcher;

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
        this.selectedDungeon = dungeonTitle;
        this.mapSwitcher = ScriptableObject.CreateInstance<MapSwitcher>().Init(mapInstance, maps);
    }

    void OnGUI()
    {
        mapSwitcher.DrawMapSwitchButtons();
        currentMap = mapSwitcher.GetCurrentMap();

        DrawEditModeSelection();
    }

    void OnDestroy()
    {
        mapSwitcher.Dispose();
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
