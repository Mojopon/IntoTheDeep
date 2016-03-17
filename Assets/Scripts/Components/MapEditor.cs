using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class MapEditor : MonoBehaviour
{
    public MapInstance mapInstancePrefab;

    private Map editingMap;

    public Map[] GetMaps(DungeonTitle dungeon, int levels)
    {
        return MapPatternFileManager.ReadFromFiles(dungeon.ToString(), levels);
    }

    private List<GizmoLabel> gizmoLabels;

    public void AddLabel(Vector3 position, Vector3 size, Vector3 labelOffset, Color color, string text)
    {
        if (gizmoLabels == null) gizmoLabels = new List<GizmoLabel>();

        gizmoLabels.Add(new GizmoLabel(position, size, labelOffset, color, text));
    }

    public void ClearLabels()
    {
        if (gizmoLabels == null) return;

        gizmoLabels.Clear();
    }
    

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (gizmoLabels == null) return;

        foreach (var gizmoLabel in gizmoLabels)
        {
            if (gizmoLabel.color != null)
            {
                Gizmos.color = gizmoLabel.color;
            }
            else
            {
                Gizmos.color = Color.white;
            }

            Gizmos.DrawWireCube(gizmoLabel.position, gizmoLabel.size);
            UnityEditor.Handles.Label(gizmoLabel.position + gizmoLabel.labelOffset, gizmoLabel.text);
        }
#endif
    }
}

public class GizmoLabel
{
    public Vector3 position;
    public Vector3 size;
    public Vector3 labelOffset;
    public Color color;
    public string text = "";

    public GizmoLabel() { }
    
    public GizmoLabel(Vector3 position, Vector3 size, Vector3 labelOffset, Color color, string text)
    {
        this.position = position;
        this.size = size;
        this.labelOffset = labelOffset;
        this.color = color;
        this.text = text;
    }
}
