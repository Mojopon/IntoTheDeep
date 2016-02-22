using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransitionWorld
{
    private Map[] maps;
    private int currentMap = 0;

    public TransitionWorld(Map[] maps)
    {
        this.maps = maps;
    }

    private List<Character> players = new List<Character>();
    public void AddPlayer(Character player)
    {
        players.Add(player);
    }

    private World previousWorld;
    public World GoNext()
    {
        if(previousWorld != null)
        {
            previousWorld.Dispose();
            previousWorld = null;
        }

        var nextMap = maps[currentMap++];
        if (currentMap >= maps.Length) currentMap = 0;
        var newWorld = new World(nextMap);

        for(int i = 0; i < players.Count; i++)
        {
            players[i].SetLocation(nextMap.playerStartPosition);
            newWorld.AddCharacter(players[i]);
        }

        previousWorld = newWorld;
        return newWorld;
    }
}
