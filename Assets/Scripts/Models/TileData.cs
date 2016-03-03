using UnityEngine;
using System.Collections.Generic;
using System;

public class TileDatas
{
    Dictionary<int, TileData> tiles = new Dictionary<int, TileData>();

    public TileDatas() { }

    public void Add(params TileData[] tileDatas)
    {
        foreach (var tileData in tileDatas)
            Add(tileData);
    }

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

[Serializable]
public class TileData
{
    public int id { get; private set; }
    public bool canWalk = true;

    public TileData(int tileID)
    {
        this.id = tileID;
    }
}
