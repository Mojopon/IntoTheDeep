using UnityEngine;
using System.Collections;
using System;

public class MapInstance : MonoBehaviour
{
    public Transform tilePrefab;
    public float tileSize = 1.0f;
    public int mapWidth = 5;
    public int mapDepth = 5;
    public Vector2 center = Vector2.zero;

    public Func<int, int, bool> MoveChecker { get; private set; }
    public Func<int, int, Vector2> CoordToWorldPositionConverter { get; private set; }

    void Start()
    {
        InstantiateMap();
        MoveChecker = new Func<int, int, bool>((x, y) => CanMove(x, y));
        CoordToWorldPositionConverter = new Func<int, int, Vector2>((x, y) => CoordToWorldPosition(x, y));
    }

    public void RegisterUtilityUser(IMapInstanceUtilitiesUser user)
    {
        user.MoveChecker = MoveChecker;
        user.CoordToWorldPositionConverter = CoordToWorldPositionConverter;
    }

    void InstantiateMap()
    {
        for(int y = 0; y < mapDepth; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                var newTile = Instantiate(tilePrefab, CoordToWorldPosition(x, y), tilePrefab.rotation) as Transform;
                newTile.localScale = newTile.localScale * tileSize;
            }
        }
    }

    Vector2 CoordToWorldPosition(int x, int y)
    {
        return new Vector2((-mapWidth * tileSize) /2f + (tileSize / 2) + (x * tileSize),
                           (-mapDepth * tileSize) / 2f + (tileSize / 2) + (y * tileSize));
    }

    bool CanMove(int x, int y)
    {
        if (x < 0 || y < 0 || x >= mapWidth || y >= mapDepth) return false;

        return true;
    }
}
