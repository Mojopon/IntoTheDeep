using UnityEngine;
using System.Collections;
using NUnit.Framework;
using NSubstitute;

[TestFixture]
public class CharacterManagerTest
{
    CharacterManager characterManager;
    ICharacterDataFileManager characterDataFileManager;

    CharacterDataTable characterDataOne;
    CharacterDataTable characterDataTwo;
    CharacterDataTable characterDataThree;

    [SetUp]
    public void Initialize()
    {
        characterManager = new CharacterManager();
        characterDataFileManager = Substitute.For<ICharacterDataFileManager>();

        characterDataOne = new CharacterDataTable();
        characterDataTwo = new CharacterDataTable();
        characterDataThree = new CharacterDataTable();

        characterDataFileManager.ReadFromFile(0).Returns(characterDataOne);
        characterDataFileManager.ReadFromFile(1).Returns(characterDataTwo);
        characterDataFileManager.ReadFromFile(2).Returns(characterDataThree);
    }
}
