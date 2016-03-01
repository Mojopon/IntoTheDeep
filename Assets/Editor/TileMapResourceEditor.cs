using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(TileMapResource))]
public class TileMapResourceEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Load TileSet"))
        {
            var tilemapResource = (TileMapResource)target;
            tilemapResource.tileSet = Resources.LoadAll<Sprite>("MapTiles");
        }
    }
}
