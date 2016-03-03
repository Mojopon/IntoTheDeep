using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class TileDataEditorWindow : EditorWindow
{
    private int selectedTileID;

    private Dictionary<int, bool> toggledTiles;

    private Vector2 scrollPos;
    private Rect windowRect;

    public static void ShowMainWindow()
    {
        EditorWindow.GetWindow(typeof(TileDataEditorWindow));
    }


    void OnGUI()
    {
        windowRect = new Rect(this.position.x, this.position.y, this.position.width, this.position.height);

        if (toggledTiles == null) CreateToggleTileList();

        DrawTilesSelection();
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
                selectedTileID = i;
            }

            toggledTiles[i] = GUILayout.Toggle(toggledTiles[i], "", GUILayout.Width(40));
            x += w + 10;
            i++;
        }
        EditorGUILayout.EndVertical();
        GUILayout.EndScrollView();
    }
}
