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

    private List<CharacterDataTable> players = new List<CharacterDataTable>();
    public void AddPlayer(CharacterDataTable playerData)
    {
        players.Add(playerData);
    }

    private List<CharacterDataTable> enemies = new List<CharacterDataTable>();
    public void AddEnemy(CharacterDataTable enemyData)
    {
        enemies.Add(enemyData);
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
            var character = Character.Create(players[i]);
            newWorld.AddCharacter(character, nextMap.playerStartPositions[i]);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemy = Character.Create(enemies[i]);
            var availableCells = nextMap.GetAvailableCells();
            var locationToSpawn = availableCells[Random.Range(0, availableCells.Count)].Location;
            newWorld.AddCharacterAsEnemy(enemy, locationToSpawn);
        }

        previousWorld = newWorld;
        return newWorld;
    }
}
