using UnityEngine;
using System.Collections;

public class TileMapResource : MonoBehaviour
{
    public Sprite[] tileSet;

    public Sprite GetTileFromTileID(int tileID)
    {
        if(tileID < 0 || tileID >= tileSet.Length)
        {
            return null;
        }

        return tileSet[tileID];
    }
}
