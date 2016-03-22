using UnityEngine;
using System.Collections.Generic;

public class CharacterManager
{
    private int maxCharactersNumber = 10;

    private CharacterDataTable[] characters;

    public CharacterManager()
    {
        characters = new CharacterDataTable[maxCharactersNumber];
    }

    public void Load(ICharacterDataFileManager characterDataFileManager)
    {
        for(int i = 0; i < maxCharactersNumber; i++)
        {
            characters[i] = characterDataFileManager.ReadFromFile(i);
        }
    }

    public void Save(ICharacterDataFileManager characterDataFileManager)
    {
        for(int i = 0; i < maxCharactersNumber; i++)
        {
            characterDataFileManager.WriteToFile(characters[i], i);
        }
    }

    public bool IsMax { get { return Count() >= maxCharactersNumber; } }

    public int Count()
    {
        int numOfCharacters = 0;
        foreach (var character in characters) if (character != null) numOfCharacters++;

        return numOfCharacters;
    }
}
