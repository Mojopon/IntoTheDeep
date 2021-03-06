﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class MapTileEditorWindow : EditorWindow
{
    private MapInstance mapInstance; 

    private DungeonTitle selectedDungeon;
    private Map currentMap;

    private MapSwitcher mapSwitcher;

    private float gridSize = 50.0f;

    private int selectedTileID;
    private string selectedImagePath;
    private bool tileSelected { get { return selectedImagePath != null; } }

    private Rect windowRect;
    private Vector2 scrollPos;

    private Vector2 mousePos;
    private bool mouseDown = false;

    private bool changeTile = false;

    public static bool IsOpened { get; private set; }
    public static void ShowMainWindow(MapInstance mapInstance, DungeonTitle title, Map[] maps)
    {
        var window = (MapTileEditorWindow)EditorWindow.GetWindow(typeof(MapTileEditorWindow), false);
        window.SetMap(mapInstance, title, maps);
        IsOpened = true;
    }

    void SetMap(MapInstance mapInstance, DungeonTitle dungeonTitle, Map[] maps)
    {
        this.mapInstance = mapInstance;
        this.selectedDungeon = dungeonTitle;
        if (maps != null)
        {
            this.mapSwitcher = ScriptableObject.CreateInstance<MapSwitcher>().Init(mapInstance, maps);
        }
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (selectedImagePath == null) return;

        Event e = Event.current;
        Ray worldRays = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        mousePos = worldRays.origin;

        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        if (e.type == EventType.layout)
        {
            HandleUtility.AddDefaultControl(controlID);
        }
        switch (e.type)
        {
            case EventType.mouseDown:
                {
                    if(e.button == 0)
                    {
                        mouseDown = true;
                    }
                }
                break;
            case EventType.MouseUp:
                {
                    if(e.button == 0)
                    {
                        mouseDown = false;
                    }
                }
                break;
        }

        if(mouseDown)
        {
            changeTile = true;
        }else
        {
            changeTile = false;
        }

        if(changeTile)
        {
            var coord = mapInstance.WorldPositionToCoord(mousePos);
            currentMap.SetTileID(coord.x, coord.y, selectedTileID);
        }
    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }

    void OnDestroy()
    {
        mapSwitcher.Dispose();
        IsOpened = false;
    }

    void OnGUI()
    {
        if (mapSwitcher == null) return;

        mapSwitcher.DrawMapSwitchButtons();
        currentMap = mapSwitcher.GetCurrentMap();

        DrawUIForMapWidth();
        DrawUIForMapDepth();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
        {
            MapPatternFileManager.WriteToFiles(selectedDungeon, mapSwitcher.GetMaps());
            Debug.Log("Map Tile Pattern has been saved");
        }
        if (GUILayout.Button("Clear"))
        {
            for (int y = 0; y < currentMap.Depth; y++)
            {
                for (int x = 0; x < currentMap.Width; x++)
                {
                    currentMap.SetTileID(x, y, 0);
                }
            }
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        DrawSelectedImage();

        windowRect = new Rect(this.position.x, this.position.y, this.position.width, this.position.height - 50);

        DrawImageParts();
    }

    void DrawUIForMapWidth()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Width : ", GUILayout.Width(110));
        if (GUILayout.Button("+", GUILayout.Width(30), GUILayout.Height(15)))
        {
            currentMap.IncreaseMapWidth();
            mapInstance.Generate(currentMap);
        }
        if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(15)))
        {
            currentMap.DecreaseMapWidth();
            mapInstance.Generate(currentMap);
        }
        EditorGUILayout.IntField(currentMap.Width);
        GUILayout.EndHorizontal();
    }

    void DrawUIForMapDepth()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Depth : ", GUILayout.Width(110));
        if (GUILayout.Button("+", GUILayout.Width(30), GUILayout.Height(15)))
        {
            currentMap.IncreaseMapDepth();
            mapInstance.Generate(currentMap);
        }
        if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(15)))
        {
            currentMap.DecreaseMapDepth();
            mapInstance.Generate(currentMap);
        }
        EditorGUILayout.IntField(currentMap.Depth);
        GUILayout.EndHorizontal();
    }

    void DrawSelectedImage()
    {
        if (selectedImagePath != null)
        {
            Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(selectedImagePath, typeof(Texture2D));
            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label("select : " + selectedTileID);
            GUILayout.Box(tex);
            EditorGUILayout.EndVertical();

        }
    }

    void DrawImageParts()
    {
        string path = ResourcePath.TILE_ASSETS_PATH;
        if (!Directory.Exists(path))
        {
            Debug.Log("Couldnt find TileSet Folder! " + path);
            return;
        }

        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(windowRect.width), GUILayout.Height(windowRect.height - 110));

        float x = 0.0f;
        float y = 00.0f;
        float w = 50.0f;
        float h = 50.0f;
        float maxW = 300.0f;

        string[] names = Directory.GetFiles(path, "*.png");
        EditorGUILayout.BeginVertical();
        int i = 0;
        foreach (string d in names)
        {
            if (x > maxW)
            {
                x = 0.0f;
                y += h;
                EditorGUILayout.EndHorizontal();
            }
            if (x == 0.0f)
            {
                EditorGUILayout.BeginHorizontal();
            }
            GUILayout.FlexibleSpace();
            Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(d, typeof(Texture2D));
            if (GUILayout.Button(tex, GUILayout.MaxWidth(w), GUILayout.MaxHeight(h), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
            {
                selectedImagePath = d;
                selectedTileID = i;
            }
            GUILayout.FlexibleSpace();
            x += w;
            i++;
        }
        EditorGUILayout.EndVertical();

        GUILayout.EndScrollView();

    }
}

