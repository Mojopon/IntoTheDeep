using UnityEngine;
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

    public DungeonTitle selectedDungeon;
    public int dungeonLevels;

    public Map[] maps;

    private int currentMapNumber = -1;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Dungeon : ", GUILayout.Width(110));
        selectedDungeon = (DungeonTitle)EditorGUILayout.EnumPopup(selectedDungeon);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Levels: ", GUILayout.Width(110));
        dungeonLevels = EditorGUILayout.IntField(dungeonLevels);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Start Editing"))
        {

            maps = MapDataFileManager.ReadMapsFromFile(selectedDungeon.ToString(), dungeonLevels);
        }
        EditorGUILayout.EndHorizontal();


        /*
        var mapEditor = target as MapEditor;
        if (mapEditor == null || !mapEditor.CanInstantiateEditingMap())
        {
            DestroyInstantiatedMap();
            currentMapNumber = -1;
            return;
        }

        if(currentMapNumber != mapEditor.editingMap)
        {
            DestroyInstantiatedMap();

            var mapHolder =  new GameObject("MapHolder");
            mapHolder.tag = "EditingMapHolder";

            var mapInstance = mapEditor.InstantiateEditingMap();
            mapInstance.SetParent(mapHolder.transform);
        }

        */
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
