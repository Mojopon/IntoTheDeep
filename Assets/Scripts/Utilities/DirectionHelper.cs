using UnityEngine;
using System.Collections;

public static class DirectionHelper
{
    public static Vector2 ToVector2(this Direction dir)
    {
        switch(dir)
        {
            case Direction.None:
            default:
                return Vector2.zero;
            case Direction.Up:
                return Vector2.up;
            case Direction.Right:
                return Vector2.right;
            case Direction.Down:
                return -Vector2.up;
            case Direction.Left:
                return -Vector2.right;
        }
    }

    public static Coord ToCoord(this Direction dir)
    {
        switch (dir)
        {
            case Direction.None:
            default:
                return Coord.zero;
            case Direction.Up:
                return Coord.up;
            case Direction.Right:
                return Coord.right;
            case Direction.Down:
                return Coord.down;
            case Direction.Left:
                return Coord.left;
        }
    }

    public static Direction GetOpposide(this Direction dir)
    {
        switch (dir)
        {
            case Direction.None:
            default:
                return Direction.None;
            case Direction.Up:
                return Direction.Down;
            case Direction.Right:
                return Direction.Left;
            case Direction.Down:
                return Direction.Up;
            case Direction.Left:
                return Direction.Right;
        }
    }
}
