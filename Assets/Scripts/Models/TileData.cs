using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml.Serialization;

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

    public TileData[] GetAllTiles()
    {
        var allTiles = new List<TileData>();
        foreach(var pair in tiles)
        {
            allTiles.Add(pair.Value);
        }

        return allTiles.ToArray();
    }
}

[Serializable]
public class TileData
{
    [XmlIgnore]
    public int id { get; set; }

    public string fileName;
    public bool canWalk = true;
    public bool isExit = false;

    public TileData() { }

    public TileData(int tileID)
    {
        this.id = tileID;
    }
}
