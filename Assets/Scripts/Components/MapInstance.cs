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

    public void Generate(Map map)
    {
        currentMap = map;
        tileMapResource = Instantiate(tileMapResourcePrefab);
        tileMapResource.transform.SetParent(transform);

        CoordToWorldPositionConverter = new Func<int, int, Vector2>((x, y) => CoordToWorldPosition(x, y));

        InstantiateMap();
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

        for (int y = 0; y < currentMap.Depth; y++)
        {
            for (int x = 0; x < currentMap.Width; x++)
            {
                var newTile = Instantiate(tilePrefab,
                                         (Vector3)CoordToWorldPosition(x, y) + new Vector3(0, 0, 5),
                                          Quaternion.identity) as Tile;
                newTile.SetLowerTile(tileMapResource.GetTileFromTileID(30));
                newTile.transform.SetParent(tileHolder.transform);
            }
        }
    }

    Vector2 CoordToWorldPosition(int x, int y)
    {
        return new Vector2((-currentMap.Width * tileSize) / 2f + (tileSize / 2) + (x * tileSize),
                           (-currentMap.Depth * tileSize) / 2f + (tileSize / 2) + (y * tileSize));
    }

    Coord WorldPositionToCoord(Vector3 pos)
    {
        return new Coord(0, 0);
    }
}