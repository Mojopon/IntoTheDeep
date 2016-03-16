using UnityEngine;
using System.Collections;

public class MapFixtureFactory
{
    public static Map Create(int width, int depth)
    {
        return CreateFromPattern(new int[width, depth]);
    }

    public static Map CreateFromPattern(int[,] mapPattern)
    {
        return CreateFromPattern(mapPattern, false);
    }

    public static Map CreateFromPattern(int[,] mapPatternRaw, bool invertDirection)
    {
        int[,] mapPattern;

        if (invertDirection)
        {
            mapPattern = GetRotatedMapPattern(mapPatternRaw);
        }
        else
        {
            mapPattern = mapPatternRaw;
        }

        return new Map(mapPattern);
    }

    public static int[,] GetRotatedMapPattern(int[,] mapPatternRaw)
    {
        int[,] mapPattern = new int[mapPatternRaw.GetLength(1), mapPatternRaw.GetLength(0)];

        for (int y = 0; y < mapPattern.GetLength(1); y++)
        {
            for (int x = 0; x < mapPattern.GetLength(0); x++)
            {
                mapPattern[x, y] = mapPatternRaw[y, x];
            }
        }

        return mapPattern;
    }
}
