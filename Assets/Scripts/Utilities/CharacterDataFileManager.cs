using UnityEngine;
using System.Collections;
using System.IO;

public interface ICharacterDataFileManager
{
    CharacterDataTable ReadFromFile(int slot);
    bool CharacterDataExists(int slot);
    void WriteToFile(SerializableCharacterData characterData, int slot);
    void WriteToFile(CharacterDataTable characterData, int slot);
}

public class CharacterDataFileManager : ICharacterDataFileManager
{

    private string rootPath;
    public static readonly string CHARACTER_DATA_FILE = "characterdata.txt";


    public CharacterDataFileManager(string characterDataFileFolder)
    {
        this.rootPath = characterDataFileFolder;

        if(!Directory.Exists(rootPath))
        {
            Directory.CreateDirectory(rootPath);
        }
    }

    private void DirectoryCheck(int slot)
    {
        if (!Directory.Exists(GetCharacterDataFolder(slot)))
        {
            Directory.CreateDirectory(GetCharacterDataFolder(slot));
        }
    }

    public CharacterDataTable ReadFromFile(int slot)
    {
        DirectoryCheck(slot);

        var filePath = GetCharacterDataFilePath(slot);
        if (!File.Exists(filePath))
        {
            return null;
        }

        var characterDataRaw = ObjectSerializer.DeSerializeObject<SerializableCharacterData>(filePath);

        return RawDataToDataTable(characterDataRaw);
    }

    public bool CharacterDataExists(int slot)
    {
        var filePath = GetCharacterDataFilePath(slot);
        if (!File.Exists(filePath))
        {
            return false;
        }

        return true;
    }

    private CharacterDataTable RawDataToDataTable(SerializableCharacterData characterDataRaw)
    {
        return new CharacterDataTable(characterDataRaw);
    }

    public void WriteToFile(SerializableCharacterData characterData, int slot)
    {
        DirectoryCheck(slot);

        var path = GetCharacterDataFilePath(slot);

        if(characterData == null)
        {
            characterData = new SerializableCharacterData();
        }

        ObjectSerializer.SerializeObject(characterData, path);
    }

    public void WriteToFile(CharacterDataTable characterData, int slot)
    {
        WriteToFile(characterData.ToSerializableCharacterData(), slot);
    }


    public string GetCharacterDataFolder(int slot)
    {
        return rootPath + slot + "/";
    }

    public string GetCharacterDataFilePath(int slot)
    {
        return rootPath + slot + "/" + CHARACTER_DATA_FILE;
    }
}
