using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class MapTileEditorWindow : EditorWindow
{
    private readonly string TILE_ASSETS_PATH = "Assets/Resources/MapTiles";

    private MapInstance mapInstance;
    private DungeonTitle dungeonTitle;
    private Map[] maps;
    private Map currentMap;
    private int currentMapNumber = 0;

    private float gridSize = 50.0f;

    private int selectedTileID;
    private string selectedImagePath;

    private Rect windowRect;
    private Vector2 scrollPos;

    public static void ShowMapEditorMainWindow(MapInstance mapInstance, DungeonTitle title, Map[] maps)
    {
        var window = (MapTileEditorWindow)EditorWindow.GetWindow(typeof(MapTileEditorWindow), false);
        window.SetMap(mapInstance, title, maps);
    }

    void SetMap(MapInstance mapInstance, DungeonTitle dungeonTitle, Map[] maps)
    {
        this.mapInstance = mapInstance;
        this.dungeonTitle = dungeonTitle;
        this.maps = maps;
    }

    void OnGUI()
    {
        if (maps == null || maps.Length == 0)
        {
            return;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Current Map: ", GUILayout.Width(110));
        if (GUILayout.Button("+", GUILayout.Width(30), GUILayout.Height(15)))
        {
            currentMapNumber++;
        }
        if(GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(15)))
        {
            currentMapNumber--;
        }
        currentMapNumber = EditorGUILayout.IntField(currentMapNumber);

        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if (currentMapNumber < 0)
        {
            currentMapNumber = 0;
        }
        else if (currentMapNumber >= maps.Length)
        {
            currentMapNumber = maps.Length - 1;
        }

        var nextMap = maps[currentMapNumber];
        if(nextMap != currentMap)
        {
            mapInstance.Generate(nextMap);
            currentMap = nextMap;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("map size : ", GUILayout.Width(110));
        currentMap.Width = EditorGUILayout.IntField(currentMap.Width);
        currentMap.Depth = EditorGUILayout.IntField(currentMap.Depth);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        DrawSelectedImage();

        windowRect = new Rect(this.position.x, this.position.y + 70, this.position.width, this.position.height - 70);

        DrawImageParts();
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
        var tilesetPath = TILE_ASSETS_PATH;
        if (!Directory.Exists(tilesetPath))
        {
            Debug.Log("Couldnt find TileSet Folder! " + tilesetPath);
            return;
        }

        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(windowRect.width), GUILayout.Height(windowRect.height - 110));

        float x = 0.0f;
        float y = 00.0f;
        float w = 50.0f;
        float h = 50.0f;
        float maxW = 300.0f;

        string path = TILE_ASSETS_PATH;

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

