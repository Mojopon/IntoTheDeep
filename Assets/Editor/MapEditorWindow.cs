using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class MapEditorWindow : EditorWindow
{
    private Object imgDirectory;

    private Map map;

    private float gridSize = 50.0f;

    private string selectedImagePath;

    private Rect windowRect;
    private Vector2 scrollPos;

    static void ShowMapEditorMainWindow(Map mapToEdit)
    {
        var window = (MapEditorWindow)EditorWindow.GetWindow(typeof(MapEditorWindow), false);
        window.SetMap(mapToEdit);
    }

    void SetMap(Map map)
    {
        this.map = map;
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Image Directory : ", GUILayout.Width(110));
        imgDirectory = EditorGUILayout.ObjectField(imgDirectory, typeof(UnityEngine.Object), true);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("map size : ", GUILayout.Width(110));
        map.Width = EditorGUILayout.IntField(map.Width);
        map.Depth = EditorGUILayout.IntField(map.Depth);
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
            GUILayout.Label("select : " + selectedImagePath);
            GUILayout.Box(tex);
            EditorGUILayout.EndVertical();

        }
    }

    void DrawImageParts()
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(windowRect.width), GUILayout.Height(windowRect.height - 110));
        if (imgDirectory != null)
        {
            float x = 0.0f;
            float y = 00.0f;
            float w = 50.0f;
            float h = 50.0f;
            float maxW = 300.0f;

            string path = AssetDatabase.GetAssetPath(imgDirectory);
            string[] names = Directory.GetFiles(path, "*.png");
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
                GUILayout.FlexibleSpace();
                Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(d, typeof(Texture2D));
                if (GUILayout.Button(tex, GUILayout.MaxWidth(w), GUILayout.MaxHeight(h), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false)))
                {
                    selectedImagePath = d;
                }
                GUILayout.FlexibleSpace();
                x += w;
            }
            EditorGUILayout.EndVertical();
        }
        GUILayout.EndScrollView();

    }
}

