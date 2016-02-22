using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class MapInstance : MonoBehaviour, IMapInstanceUtilitiesProvider
{
    public Transform tilePrefab;
    public float tileSize = 1.0f;
    public Vector2 center = Vector2.zero;

    public Map[] maps;

    public Func<int, int, Vector2> CoordToWorldPositionConverter { get; private set; }

    private Map currentMap;

    public void Generate() { Generate(0); }

    public void Generate(int id)
    {
        currentMap = maps[id];
        currentMap.Initialize();

        DecorateMap();
        InstantiateMap();
        CoordToWorldPositionConverter = new Func<int, int, Vector2>((x, y) => CoordToWorldPosition(x, y));
    }

    public Map GetCurrentMap()
    {
        return currentMap;
    }

    void DecorateMap()
    {
        currentMap.GetCell(4, 4).canWalk = false;
    }

    void InstantiateMap()
    {
        for (int y = 0; y < currentMap.Depth; y++)
        {
            for (int x = 0; x < currentMap.Width; x++)
            {
                var newTile = Instantiate(tilePrefab,
                                         (Vector3)CoordToWorldPosition(x, y) + new Vector3(0, 0, 5),
                                          tilePrefab.rotation) as Transform;
                newTile.localScale = newTile.localScale * tileSize;
                newTile.SetParent(transform);
                if (!currentMap.GetCell(x, y).canWalk) newTile.localScale = newTile.localScale / 2;
                if (currentMap.GetCell(x, y).isExit) newTile.localScale = newTile.localScale / 3;
            }
        }
    }

    Vector2 CoordToWorldPosition(int x, int y)
    {
        return new Vector2((-currentMap.Width * tileSize) / 2f + (tileSize / 2) + (x * tileSize),
                           (-currentMap.Depth * tileSize) / 2f + (tileSize / 2) + (y * tileSize));
    }
}