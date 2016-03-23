using UnityEngine;
using System.Collections.Generic;

public class CharacterManager
{
    private readonly int MAXCHARACTER_NUMBER = 10;

    private CharacterDataTable[] characters;

    public CharacterManager()
    {
        characters = new CharacterDataTable[MAXCHARACTER_NUMBER];
    }

    public void Load(ICharacterDataFileManager characterDataFileManager)
    {
        for(int i = 0; i < MAXCHARACTER_NUMBER; i++)
        {
            characters[i] = characterDataFileManager.ReadFromFile(i);
        }
    }

    public void Save(ICharacterDataFileManager characterDataFileManager)
    {
        for(int i = 0; i < MAXCHARACTER_NUMBER; i++)
        {
            if (characters[i] == null) continue;
            characterDataFileManager.WriteToFile(characters[i], i);
        }
    }

    public bool Add(CharacterDataTable character, int slot)
    {
        if (slot < 0 || slot >= characters.Length || characters[slot] != null) return false;

        characters[slot] = character;
        return true;
    }

    public bool IsMax { get { return Count() >= MAXCHARACTER_NUMBER; } }

    public int Count()
    {
        int numOfCharacters = 0;
        foreach (var character in characters) if (character != null) numOfCharacters++;

        return numOfCharacters;
    }
}
