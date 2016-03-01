using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

public class CheckDirectoryForMapEditor : Editor
{
    public int fileCheckClear = 0;
    private string maptileDirectory = "Sprites/MapTiles/";
    public string[] resourcesPath, resourceFile;

    public void BeginDirectoryCheck()
    {
        CheckPathsAndFiles();
        if(fileCheckClear == 1)
        {
            Debug.Log("File Check Clear");
        }
    }

    void CheckPathsAndFiles()
    {
        if(!Directory.Exists(maptileDirectory))
        {
            Debug.LogError("MapTiles folder not found");
        }
    }
}
