using UnityEngine;
using System.Collections;

public class MapEditor : MonoBehaviour
{
    public Map[] maps;

    public Map[] GetMaps()
    {
        foreach(var map in maps)
        {
            map.Initialize();
        }

        return maps;
    }
}
