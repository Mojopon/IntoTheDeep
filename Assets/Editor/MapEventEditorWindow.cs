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

    private MapEvents[] allMapEvents;
    private MapEvents currentMapEvents;

    private EditEvent editMode;

    private Rect windowRect;
    private Vector2 scrollPos;

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
        if (maps != null)
        {
            this.mapSwitcher = ScriptableObject.CreateInstance<MapSwitcher>().Init(mapInstance, maps);
            this.allMapEvents = MapEventFileManager.ReadFromFiles(selectedDungeon, MapPatternFileManager.GetDungeonLevel(selectedDungeon));
        }
    }

    void OnGUI()
    {
        if (mapSwitcher == null) return;
        mapSwitcher.DrawMapSwitchButtons();
        var nextMap = mapSwitcher.GetCurrentMap();

        if(currentMap != nextMap)
        {
            currentMap = nextMap;
            currentMapEvents = allMapEvents[mapSwitcher.GetCurrentMapNumber()];
        }

        DrawSaveButton();
        DrawEditModeSelection();

        windowRect = new Rect(this.position.x, this.position.y, this.position.width, this.position.height - 50);

        switch(editMode)
        {
            case EditEvent.StartPositions:
                DrawStartPositionEditor();
                break;
        }
    }

    void DrawStartPositionEditor()
    {
        var startPositionsMapEvent = currentMapEvents.startPositions;

        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(windowRect.width), GUILayout.Height(windowRect.height));

        for (int i = 0; i < startPositionsMapEvent.startPositions.Count; i++)
        {
            int x, y;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Player " + i + " Position: ", GUILayout.Width(110));
            x = EditorGUILayout.IntField(startPositionsMapEvent.startPositions[i].x);
            y = EditorGUILayout.IntField(startPositionsMapEvent.startPositions[i].y);
            GUILayout.EndHorizontal();

            if(x != startPositionsMapEvent.startPositions[i].x || y != startPositionsMapEvent.startPositions[i].y)
            {
                startPositionsMapEvent.EditStartPosition(i, new Coord(x, y));
            }
        }

        GUILayout.EndScrollView();
    }

    void OnDestroy()
    {
        mapSwitcher.Dispose();
        IsOpened = false;
    }

    void DrawSaveButton()
    {
        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Save"))
        {
            MapEventFileManager.WriteToFiles(allMapEvents, selectedDungeon);
        }

        GUILayout.EndHorizontal();
    }

    void DrawEditModeSelection()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Edit Mode: ", GUILayout.Width(110));
        editMode = (EditEvent)EditorGUILayout.EnumPopup(editMode);
        GUILayout.EndHorizontal();
    }
}
