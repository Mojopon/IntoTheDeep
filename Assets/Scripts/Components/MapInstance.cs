using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UniRx;

public class MapInstance : MonoBehaviour, IMapInstanceUtilitiesProvider
{
    public TileMapResource tileMapResourcePrefab;
    public Tile tilePrefab;
    public float tileSize = 1.0f;
    public Vector2 center = Vector2.zero;

    public Func<int, int, Vector2> CoordToWorldPositionConverter { get; private set; }

    private Map currentMap;
    private TileMapResource tileMapResource;

    private GameObject tileHolder;
    private Tile[,] tiles;

    private IDisposable subscriptionFoCurrentMap;
    public void Generate(Map map)
    {
        if(subscriptionFoCurrentMap != null)
        {
            subscriptionFoCurrentMap.Dispose();
            subscriptionFoCurrentMap = null;
        }

        currentMap = map;
        tileMapResource = Instantiate(tileMapResourcePrefab);
        tileMapResource.transform.SetParent(transform);

        CoordToWorldPositionConverter = new Func<int, int, Vector2>((x, y) => CoordToWorldPosition(x, y));

        InstantiateMap();

        subscriptionFoCurrentMap = currentMap.CellChangeObservable
                                             .Subscribe(c => ApplyTileSpriteFromTheMap(c.x, c.y));
    }

    public Map GetCurrentMap()
    {
        return currentMap;
    }

    void InstantiateMap()
    {
        if(tileHolder != null)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(tileHolder);
            }
            else {
                Destroy(tileHolder);
            }
        }

        tileHolder = new GameObject("TileHolder");
        tileHolder.transform.SetParent(transform);

        tiles = new Tile[currentMap.Width, currentMap.Depth];
        for (int y = 0; y < currentMap.Depth; y++)
        {
            for (int x = 0; x < currentMap.Width; x++)
            {
                var newTile = Instantiate(tilePrefab,
                                         (Vector3)CoordToWorldPosition(x, y) + new Vector3(0, 0, 5),
                                          Quaternion.identity) as Tile;
                newTile.transform.SetParent(tileHolder.transform);
                tiles[x, y] = newTile;
                ApplyTileSpriteFromTheMap(x, y);
            }
        }
    }

    void ApplyTileSpriteFromTheMap(int x, int y)
    {
        tiles[x, y].SetLowerTile(tileMapResource.GetTileFromTileID(30));
        tiles[x, y].SetMiddleTile(tileMapResource.GetTileFromTileID(currentMap.GetCell(x, y).tileID));
    }

    public Vector2 CoordToWorldPosition(int x, int y)
    {
        return new Vector2((-currentMap.Width * tileSize) / 2f + (tileSize / 2) + (x * tileSize),
                           (-currentMap.Depth * tileSize) / 2f + (tileSize / 2) + (y * tileSize));
    }

    public Coord WorldPositionToCoord(Vector2 position)
    {
        int x = Mathf.RoundToInt(position.x / tileSize + (currentMap.Width - 1) / 2f);
        int y = Mathf.RoundToInt(position.y / tileSize + (currentMap.Depth - 1) / 2f);

        return new Coord(x, y);
    }

    Coord WorldPositionToCoord(Vector3 pos)
    {
        return new Coord(0, 0);
    }
}