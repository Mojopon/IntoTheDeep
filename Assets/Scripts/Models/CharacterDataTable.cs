using UnityEngine;
using System.Collections;

public class Attributes
{
    public int stamina { get; set; }
    public int strength { get; set; }
    public int agility { get; set; }
    public int intellect { get; set; }

    public Attributes()
    {
        this.stamina = 10;
        this.strength = 10;
        this.agility = 10;
        this.intellect = 10;
    }
}

public interface ICharacterData
{
    string name { get; set; }
    int level { get; set; }
    int expToNextLevel { get; set; }
    int jobID { get; set; }
}

public class CharacterDataTable : ICharacterData
{
    public string name { get; set; }
    public int level { get; set; }
    public int expToNextLevel { get; set; }
    public int jobID { get; set; }

    public Attributes attributes { get; set; }

    public CharacterDataTable()
    {
        this.name = "Unknown";
        this.level = 1;
        this.expToNextLevel = 100;
        this.attributes = new Attributes();
        this.jobID = 0;
    }

    public CharacterDataTable(SerializableCharacterData characterDataRaw)
    {
        this.name = characterDataRaw.name;
        this.level = characterDataRaw.level;
        this.expToNextLevel = characterDataRaw.expToNextLevel;
        this.jobID = characterDataRaw.jobID;
        this.attributes = new Attributes();
    }

    public CharacterDataTable(string name, Attributes attributes)
    {
        this.name = name;

        this.attributes = attributes;
    }

    public SerializableCharacterData ToSerializableCharacterData()
    {
        var serializableCharacterData = new SerializableCharacterData()
        {
            name = this.name,
            level = this.level,
            expToNextLevel = this.expToNextLevel,
            jobID = this.jobID,
        };

        return serializableCharacterData;
    }
}
