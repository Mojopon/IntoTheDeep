using UnityEngine;
using System.Collections;

public static class MapHelper
{
    public static string TilePatternToText(int[,] tilePattern)
    {
        var mapText = "";
        for (int y = 0; y < tilePattern.GetLength(1); y++)
        {
            for (int x = 0; x < tilePattern.GetLength(0); x++)
            {
                mapText += tilePattern[x, y];
                if (x < tilePattern.GetLength(0) - 1)
                {
                    mapText += ",";
                }
            }
            if (y < tilePattern.GetLength(1) - 1)
            {
                mapText += "\n";
            }
        }

        return mapText;
    }

    public static int[,] TextToTilePattern(string tileText)
    {
        string[] splitted = tileText.Split('\n');
        var width = splitted[0].Split(',').Length;
        int[,] tilePattern = new int[width, splitted.Length];

        for(int y = 0; y < splitted.Length; y++)
        {
            var tileTextLine = splitted[y].Split(',');
            for(int x = 0; x < width; x++)
            {
                tilePattern[x, y] = int.Parse(tileTextLine[x]);
            }
        }

        return tilePattern;
    }
}
