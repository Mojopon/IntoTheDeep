using UnityEngine;
using System.Collections.Generic;

public class TileDatas
{
    Dictionary<int, TileData> tiles = new Dictionary<int, TileData>();

    public TileDatas() { }

    public void Add(TileData tileData)
    {
        if (tiles.ContainsKey(tileData.id)) return;

        tiles.Add(tileData.id, tileData);
    }

    public TileData Get(int id)
    {
        if (!tiles.ContainsKey(id)) return null;

        return tiles[id];
    }
}

public class TileData
{
    public int id { get; private set; }
    public bool canWalk { get; set; }

    public TileData(int tileID)
    {
        this.id = tileID;
    }
}
