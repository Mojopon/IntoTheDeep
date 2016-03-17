using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;

public class MapEventEditorWindow : EditorWindow
{
    private MapEditor mapEditor;

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
    public static void ShowMainWindow(MapEditor mapEditor, MapInstance mapInstance, DungeonTitle title, Map[] maps)
    {
        var window = (MapEventEditorWindow)EditorWindow.GetWindow(typeof(MapEventEditorWindow), false);
        window.SetMap(mapEditor, mapInstance, title, maps);
        IsOpened = true;
    }

    void SetMap(MapEditor mapEditor, MapInstance mapInstance, DungeonTitle dungeonTitle, Map[] maps)
    {
        this.mapEditor = mapEditor;
        this.selectedDungeon = dungeonTitle;
        this.mapInstance = mapInstance;
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

    private Vector2[] startPositionsOnWorld;
    void DrawStartPositionEditor()
    {
        var startPositionsMapEvent = currentMapEvents.startPositions;

        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(windowRect.width), GUILayout.Height(windowRect.height));

        List<Coord> startPositions = new List<Coord>();
        for (int i = 0; i < startPositionsMapEvent.positions.Count; i++)
        {
            int x, y;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Player " + i + " Position: ", GUILayout.Width(110));
            x = EditorGUILayout.IntField(startPositionsMapEvent.positions[i].x);
            y = EditorGUILayout.IntField(startPositionsMapEvent.positions[i].y);
            GUILayout.EndHorizontal();

            if(x != startPositionsMapEvent.positions[i].x || y != startPositionsMapEvent.positions[i].y)
            {
                startPositionsMapEvent.EditStartPosition(i, new Coord(x, y));
            }

            startPositions.Add(new Coord(x, y));
        }
        startPositionsOnWorld = startPositions.Select(c => mapInstance.CoordToWorldPosition(c.x, c.y)).ToArray();
        mapEditor.ClearLabels();
        for(int i = 0; i < startPositionsOnWorld.Length; i++)
        {
            mapEditor.AddLabel(startPositionsOnWorld[i], Vector3.one * mapInstance.tileSize, Vector3.zero, Color.red, i.ToString());
        }

        GUILayout.EndScrollView();
    }

    void OnDestroy()
    {
        if (mapSwitcher == null) return;

        mapSwitcher.Dispose();
        mapEditor.ClearLabels();

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
