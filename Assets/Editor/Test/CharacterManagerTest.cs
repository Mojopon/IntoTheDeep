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

    [Test]
    public void ShouldLoadAndSaveCharacters()
    {
        for(int i = 0; i < 10; i ++)
        {
            characterDataFileManager.DidNotReceive().ReadFromFile(i);
        }

        characterManager.Load(characterDataFileManager);

        for (int i = 0; i < 10; i++)
        {
            characterDataFileManager.Received().ReadFromFile(i);
        }

        characterManager.Save(characterDataFileManager);

        characterDataFileManager.Received().WriteToFile(characterDataOne, 0);
        characterDataFileManager.Received().WriteToFile(characterDataTwo, 1);
        characterDataFileManager.Received().WriteToFile(characterDataThree, 2);
        for (int i = 3; i < 10; i++)
        {
            characterDataFileManager.DidNotReceive().WriteToFile(Arg.Any<CharacterDataTable>(), i);
        }
    }

    [Test]
    public void ShouldAddCharacter()
    {
        characterManager.Load(characterDataFileManager);

        var characterDataFour = new CharacterDataTable();
        characterManager.Add(characterDataFour, characterManager.Count());

        characterManager.Save(characterDataFileManager);

        characterDataFileManager.Received().WriteToFile(characterDataOne, 0);
        characterDataFileManager.Received().WriteToFile(characterDataTwo, 1);
        characterDataFileManager.Received().WriteToFile(characterDataThree, 2);
        characterDataFileManager.Received().WriteToFile(characterDataFour, 3);
        for (int i = 4; i < 10; i++)
        {
            characterDataFileManager.DidNotReceive().WriteToFile(Arg.Any<CharacterDataTable>(), i);
        }
    }
}
