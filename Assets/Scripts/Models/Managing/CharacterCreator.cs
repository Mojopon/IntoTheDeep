using UnityEngine;
using System.Collections;
using UniRx;

public class CharacterCreator
{
    private CharacterDataTable newCharacter;

    public CharacterCreator()
    {
        newCharacter = new CharacterDataTable();
        newCharacter.name = "";
    }

    public void ChangeName(string name)
    {
        newCharacter.name = name;
    }

    public CharacterDataTable Get()
    {
        return newCharacter;
    }
}
