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

    public Func<Character, Coord, bool> MoveChecker { get; private set; }
    public Func<int, int, Vector2> CoordToWorldPositionConverter { get; private set; }
    public Func<Coord, Coord, Direction[]> Pathfinding { get; private set; }

    private Map currentMap;

    public void Generate() { Generate(0); }

    public void Generate(int id)
    {
        currentMap = maps[id];
        currentMap.Initialize();

        DecorateMap();
        InstantiateMap();
        MoveChecker = new Func<Character, Coord, bool>((character, destination) => CanMove(character, destination));
        CoordToWorldPositionConverter = new Func<int, int, Vector2>((x, y) => CoordToWorldPosition(x, y));
        Pathfinding = new Func<Coord, Coord, Direction[]>((source, target) => GeneratePath(source, target));
    }

    public void ProvideMapInstanceUtilities(IMapInstanceUtilitiesUser user)
    {
        user.CoordToWorldPositionConverter = new Func<int, int, Vector2>((x, y) => CoordToWorldPosition(x, y));
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
            }
        }
    }

    Vector2 CoordToWorldPosition(int x, int y)
    {
        return new Vector2((-currentMap.Width * tileSize) / 2f + (tileSize / 2) + (x * tileSize),
                           (-currentMap.Depth * tileSize) / 2f + (tileSize / 2) + (y * tileSize));
    }

    bool CanMove(Character character, Coord coord)
    {
        int x = coord.x;
        int y = coord.y;
        if (x < 0 || y < 0 || x >= currentMap.Width || y >= currentMap.Depth) return false;
        if (!currentMap.GetCell(x, y).canWalk) return false;

        return true;
    }

    Direction[] GeneratePath(Coord source, Coord target)
    {
        List<Direction> directions = new List<Direction>();
        var currentPosition = source;
        for (int i = 0; i < 10; i++)
        {
            var rand = UnityEngine.Random.Range(0, 5);
            var direction = Direction.None;
            switch (rand)
            {
                case 0:
                    direction = Direction.None;
                    break;
                case 1:
                    direction = Direction.Up;
                    break;
                case 2:
                    direction = Direction.Right;
                    break;
                case 3:
                    direction = Direction.Down;
                    break;
                case 4:
                    direction = Direction.Left;
                    break;
            }

            var destination = currentPosition + direction.ToCoord();
            if(CanMove(null, destination))
            {
                currentPosition += direction.ToCoord();
                directions.Add(direction);
            }
        }

        return directions.ToArray();
    }
}