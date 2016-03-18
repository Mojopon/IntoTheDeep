using UnityEngine;
using System.Collections;
using System.IO;

public class CharacterDataFileManager
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
            WriteToFile(null, slot);
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


    public string GetCharacterDataFolder(int slot)
    {
        return rootPath + slot + "/";
    }

    public string GetCharacterDataFilePath(int slot)
    {
        return rootPath + slot + "/" + CHARACTER_DATA_FILE;
    }
}
