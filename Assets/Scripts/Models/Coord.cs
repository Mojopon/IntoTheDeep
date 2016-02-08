using UnityEngine;
using System.Collections;

public struct Coord
{
    public int x;
    public int y;

    public Coord(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public static bool operator ==(Coord lhs, Coord rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y;
    }

    public static Coord operator +(Coord lhs, Coord rhs)
    {
        return new Coord(lhs.x + rhs.x, lhs.y + rhs.y);
    }

    public static Coord operator -(Coord lhs, Coord rhs)
    {
        return new Coord(lhs.x - rhs.x, lhs.y - rhs.y);
    }

    public static bool operator !=(Coord lhs, Coord rhs)
    {
        return !(lhs == rhs);
    }

    public static Coord zero { get { return new Coord(0, 0); } }
    public static Coord right { get { return new Coord(1, 0); } }
    public static Coord up { get { return new Coord(0, 1); } }
    public static Coord down { get { return new Coord(0, -1); } }
    public static Coord left { get { return new Coord(-1, 0); } }

}
