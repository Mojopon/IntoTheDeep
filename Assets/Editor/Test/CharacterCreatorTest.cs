using UnityEngine;
using System.Collections;
using NUnit.Framework;

[TestFixture]
public class CharacterCreatorTest
{
    CharacterCreator characterCreator;

    [SetUp]
    public void Initialize()
    {
        characterCreator = new CharacterCreator();
    }

    [Test]
    public void ShouldChangeCreatedCharacterName()
    {
        characterCreator.ChangeName("aaabb");

        var newCharacter = characterCreator.Get();
        Assert.AreEqual("aaabb", newCharacter.name);
    }
}
