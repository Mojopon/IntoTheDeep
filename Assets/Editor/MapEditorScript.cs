﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;
using System.Text;

[CustomEditor(typeof(MapEditor))]
public class MapEditorScript : Editor
{
    private const string RESOURCE_FOLDER = "Resources/";
    private const string DUNGEON_DATA_FOLDER = "DungeonDatas/";

    private DungeonTitle previousSelectedDungeon;
    private bool dungeonSelectChanged = false;
    public DungeonTitle selectedDungeon;
    public int dungeonLevels = -1;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawSelectDungeonMenus();
    }

    void DrawSelectDungeonMenus()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Dungeon : ", GUILayout.Width(110));
        selectedDungeon = (DungeonTitle)EditorGUILayout.EnumPopup(selectedDungeon);
        EditorGUILayout.EndHorizontal();

        if (previousSelectedDungeon != selectedDungeon) dungeonSelectChanged = true;

        if(dungeonLevels == -1 || dungeonSelectChanged)
        {
            dungeonLevels = MapDataFileManager.GetAllMapPatternFileCount(selectedDungeon);
            dungeonSelectChanged = false;
        }

        previousSelectedDungeon = selectedDungeon;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Levels: ", GUILayout.Width(110));
        dungeonLevels = EditorGUILayout.IntField(dungeonLevels);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Start Editing"))
        {
            var mapInstance = SpawnMapInstance();
            var editor = (MapEditor)target;
            MapTileEditorWindow.ShowMapEditorMainWindow(mapInstance, selectedDungeon, editor.GetMaps(selectedDungeon, dungeonLevels));
        }
        EditorGUILayout.EndHorizontal();

        
    }

    MapInstance SpawnMapInstance()
    {
        var mapInstanceObject = GameObject.FindGameObjectWithTag("MapInstance");

        if(mapInstanceObject == null)
        {
            var editor = (MapEditor)target;
            return Instantiate(editor.mapInstancePrefab);
        }else
        {
            return mapInstanceObject.GetComponent<MapInstance>();
        }
    }

    void DestroyInstantiatedMap()
    {
        var mapHolder = GameObject.FindGameObjectWithTag("EditingMapHolder");

        if(mapHolder!= null)
        {
            DestroyImmediate(mapHolder);
        }
    }
}
