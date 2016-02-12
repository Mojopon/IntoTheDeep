using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class MapInstance : MonoBehaviour
{
    public Transform tilePrefab;
    public float tileSize = 1.0f;
    public Vector2 center = Vector2.zero;

    public Map[] maps;

    public Func<int, int, bool> MoveChecker { get; private set; }
    public Func<int, int, Vector2> CoordToWorldPositionConverter { get; private set; }
    public Func<Coord, Coord, Direction[]> Pathfinding { get; private set; }

    private Map map;

    public void Generate() { Generate(0); }

    public void Generate(int id)
    {
        map = maps[id];
        map.Initialize();

        DecorateMap();
        InstantiateMap();
        MoveChecker = new Func<int, int, bool>((x, y) => CanMove(x, y));
        CoordToWorldPositionConverter = new Func<int, int, Vector2>((x, y) => CoordToWorldPosition(x, y));
        Pathfinding = new Func<Coord, Coord, Direction[]>((source, target) => GeneratePath(source, target));
    }

    public void RegisterUtilityUser(IMapInstanceUtilitiesUser user)
    {
        user.MoveChecker = MoveChecker;
        user.CoordToWorldPositionConverter = CoordToWorldPositionConverter;
        user.Pathfinding = Pathfinding;
    }

    void DecorateMap()
    {
        map.GetCell(4, 4).canWalk = false;
    }

    void InstantiateMap()
    {
        for (int y = 0; y < map.Depth; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                var newTile = Instantiate(tilePrefab,
                                         (Vector3)CoordToWorldPosition(x, y) + new Vector3(0, 0, 5),
                                          tilePrefab.rotation) as Transform;
                newTile.localScale = newTile.localScale * tileSize;
                newTile.SetParent(transform);
                if (!map.GetCell(x, y).canWalk) newTile.localScale = newTile.localScale / 2;
            }
        }
    }

    Vector2 CoordToWorldPosition(int x, int y)
    {
        return new Vector2((-map.Width * tileSize) / 2f + (tileSize / 2) + (x * tileSize),
                           (-map.Depth * tileSize) / 2f + (tileSize / 2) + (y * tileSize));
    }

    bool CanMove(int x, int y)
    {
        if (x < 0 || y < 0 || x >= map.Width || y >= map.Depth) return false;
        if (!map.GetCell(x, y).canWalk) return false;

        return true;
    }

    Direction[] GeneratePath(Coord source, Coord target)
    {
        List<Direction> directions = new List<Direction>();
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

            var destination = source + direction.ToCoord();
            if(CanMove(destination.x, destination.y))
            {
                directions.Add(direction);
            }
        }

        return directions.ToArray();
    }
}