using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System;
using System.IO;

[TestFixture]
public class CharacterDataFileManagerTest
{
    private readonly string TEST_FOLDER_PATH = "Test/";

    private CharacterDataFileManager fileManager;
    private CharacterDataTable characterData;

    [SetUp]
    public void Initialize()
    {
        fileManager = new CharacterDataFileManager(TEST_FOLDER_PATH);
        characterData = new CharacterDataTable()
        {
            name = "Player",
            level = 3,
            expToNextLevel = 150,
        };

        fileManager.WriteToFile(characterData.ToSerializableCharacterData(), 0);
    }

    [Test]
    public void ShouldLoadCharacterDataFromFile()
    {
        var loadedCharacterData = fileManager.ReadFromFile(0);

        Assert.AreEqual(characterData.name, loadedCharacterData.name);
        Assert.AreEqual(characterData.level, loadedCharacterData.level);
        Assert.AreEqual(characterData.expToNextLevel, loadedCharacterData.expToNextLevel);
    }

    [TearDown]
    public void Destroy()
    {
        File.Delete(fileManager.GetCharacterDataFilePath(0));
        Directory.Delete(fileManager.GetCharacterDataFolder(0));
        Directory.Delete(TEST_FOLDER_PATH);
    }
}
