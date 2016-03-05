using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;
using UniRx;

public class TileDataEditorWindow : EditorWindow
{
    private TileData selectedTile = null;

    private bool setCanWalkProperty = true;
    private bool setIsExitProperty = false;

    private Dictionary<int, bool> toggledTiles;

    private Vector2 scrollPos;
    private int windowRectHeightOffset;
    private Rect windowRect;

    private TileDatas tileDatas;
    private bool tileDataLoaded { get { return tileDatas != null; } }

    public static void ShowMainWindow()
    {
        EditorWindow.GetWindow(typeof(TileDataEditorWindow), false);
    }


    void OnGUI()
    {
        windowRectHeightOffset = 0;

        if (!tileDataLoaded)
        {
            LoadTileDatas();
        }

        DrawSelectedTileProperties();

        DrawTilePropertiesControl();

        windowRect = new Rect(this.position.x, this.position.y + windowRectHeightOffset, this.position.width, this.position.height - windowRectHeightOffset);

        if (toggledTiles == null) CreateToggleTileList();

        DrawTilesSelection();
    }

    void LoadTileDatas()
    {
        tileDatas = TileDataFileManager.ReadFromFiles();
    }

    void CreateToggleTileList()
    {
        string path = ResourcePath.TILE_ASSETS_PATH;
        string[] names = Directory.GetFiles(path, "*.png");

        toggledTiles = new Dictionary<int, bool>();

        for(int i = 0; i < names.Length; i++)
        {
            toggledTiles.Add(i, false);
        }
    }

    void DrawSelectedTileProperties()
    {
        if (selectedTile == null) return;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(selectedTile.fileName);
        GUILayout.Label("Can Walk:");
        GUILayout.Label(" [" + selectedTile.canWalk.ToString() + "] ");
        GUILayout.Label("Is Exit:");
        GUILayout.Label(" [" + selectedTile.isExit.ToString() + "] ");

        EditorGUILayout.EndHorizontal();

        windowRectHeightOffset += 20;
    }

    void DrawTilePropertiesControl()
    {
        if (toggledTiles == null || !toggledTiles.ContainsValue(true)) return;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Can Walk:");
        setCanWalkProperty = GUILayout.Toggle(setCanWalkProperty, "", GUILayout.Width(40));
        GUILayout.Label("Is Exit:");
        setIsExitProperty = GUILayout.Toggle(setIsExitProperty, "", GUILayout.Width(40));
        if (GUILayout.Button("Apply To Toggled Tiles"))
        {
            foreach(var pair in toggledTiles)
            {
                // skip when its not toggled
                if (!pair.Value) continue;

                var targetTile = tileDatas.Get(pair.Key);
                ApplyTilePropetiesToTile(targetTile);
            }

            TileDataFileManager.WriteToFiles(tileDatas);
        }
        EditorGUILayout.EndHorizontal();

        windowRectHeightOffset += 20;
    }

    void ApplyTilePropetiesToTile(TileData tile)
    {
        tile.canWalk = setCanWalkProperty;
        tile.isExit = setIsExitProperty;
    }

    void DrawTilesSelection()
    {
        if (toggledTiles == null) return;

        string path = ResourcePath.TILE_ASSETS_PATH;
        if (!Directory.Exists(path))
        {
            Debug.Log("Couldnt find TileSet Folder! " + path);
            return;
        }

        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(windowRect.width), GUILayout.Height(windowRect.height));

        float x = 0.0f;
        float y = 00.0f;
        float w = 50.0f;
        float h = 50.0f;
        float maxW = 300.0f;

        string[] names = Directory.GetFiles(path, "*.png");
        int i = 0;
        EditorGUILayout.BeginVertical();
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
            Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(d, typeof(Texture2D));
            if (GUILayout.Button(tex, GUILayout.MaxWidth(w), GUILayout.MaxHeight(h), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
            {
                selectedTile = tileDatas.Get(i);
            }

            toggledTiles[i] = GUILayout.Toggle(toggledTiles[i], "", GUILayout.Width(40));
            x += w + 10;
            i++;
        }
        EditorGUILayout.EndVertical();
        GUILayout.EndScrollView();
    }
}
