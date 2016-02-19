using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;
using System;

[TestFixture]
public class CharacterTest
{
    CompositeDisposable disposables = new CompositeDisposable();

    Character character;

    [SetUp]
    public void Initialize()
    {
        character = Character.Create();
    }

    [Test]
    public void ShouldMoveToTheGivenDirection()
    {
        Coord coordAfterMove = Coord.zero;
        Assert.AreEqual(0, coordAfterMove.x);
        Assert.AreEqual(0, coordAfterMove.y);

        character.Location
                 .Subscribe(x =>
                 {
                     coordAfterMove = x;
                 }).AddTo(disposables);

        var changedToCombatActionPhase = false;
        character.CurrentPhase.Where(x => x == Character.Phase.CombatAction).Subscribe(x => changedToCombatActionPhase = true);
        // cant move until it goes move phase
        Assert.IsFalse(character.CanMove(Direction.Right));
        character.Move(Direction.Right);
        Assert.AreEqual(0, coordAfterMove.x);
        Assert.AreEqual(0, coordAfterMove.y);
        Assert.IsFalse(changedToCombatActionPhase);
   
        // need to set it to be move phase otherwise move method will be ignored
        character.SetPhase(Character.Phase.Move);

        character.Move(Direction.Right);
        Assert.AreEqual(1, coordAfterMove.x);
        Assert.AreEqual(0, coordAfterMove.y);
        Assert.IsFalse(changedToCombatActionPhase);

        character.Move(Direction.Up);
        Assert.AreEqual(1, coordAfterMove.x);
        Assert.AreEqual(1, coordAfterMove.y);
        Assert.IsFalse(changedToCombatActionPhase);

        character.Move(Direction.Up);
        Assert.AreEqual(1, coordAfterMove.x);
        Assert.AreEqual(2, coordAfterMove.y);
        Assert.IsFalse(changedToCombatActionPhase);

        // cant move more than 4 times per turns
        character.Move(Direction.Up);
        Assert.AreEqual(1, coordAfterMove.x);
        Assert.AreEqual(3, coordAfterMove.y);
        // should change current phase to CombatAction after moving for 4 times
        Assert.IsTrue(changedToCombatActionPhase);

        // cant move more than 4 times per turns
        character.Move(Direction.Up);
        Assert.AreEqual(1, coordAfterMove.x);
        Assert.AreEqual(3, coordAfterMove.y);
    }

    [Test]
    public void ShouldReturnMoveResult()
    {
        var moveResult = character.Move(Direction.Right);
        // should return null when the character failed moving
        Assert.IsNull(moveResult);

        character.SetPhase(Character.Phase.Move);
        moveResult = character.Move(Direction.Right);
        Assert.AreEqual(character, moveResult.target);
        Assert.AreEqual(new Coord(0, 0), moveResult.source);
        Assert.AreEqual(new Coord(1, 0), moveResult.destination);

        moveResult = character.Move(Direction.Up);

        Assert.AreEqual(character, moveResult.target);
        Assert.AreEqual(new Coord(1, 0), moveResult.source);
        Assert.AreEqual(new Coord(1, 1), moveResult.destination);
    }

    [Test]
    public void ShouldReturnCanMoveorNot()
    {
        character.MoveChecker = new Func<Character, Coord, bool>((chara, c) =>
                                                                {
                                                                    if (c.x == 1 && c.y == 0) return false;

                                                                    return true;
                                                                });

        // should return false when its not in move phase
        Assert.IsFalse(character.CanMove(Direction.Up));
        character.SetPhase(Character.Phase.Move);

        Assert.IsFalse(character.CanMove(Direction.Right));
        Assert.IsTrue(character.CanMove(Direction.Up));
        Assert.IsTrue(character.CanMove(Direction.Down));
        Assert.IsTrue(character.CanMove(Direction.Left));
    }

    [Test]
    public void ShouldBeIdlePhaseAfterSkillUsed()
    {
        character.SetPhase(Character.Phase.CombatAction);

        var skill = character.GetSkills()[0];

        character.OnSkillUsed(skill);

        Assert.AreEqual(Character.Phase.Idle, character.CurrentPhase.Value);
    }

    [Test]
    public void ShouldTransferCharacterToTheLocation()
    {
        // character can be transfered without going to move phase 
        Assert.IsTrue(character.CanTransferTo(new Coord(5, 6)));

        var moveResult = character.Transfer(new Coord(5, 6));
        Assert.AreEqual(5, character.X);
        Assert.AreEqual(6, character.Y);

        Assert.AreEqual(character,moveResult.target);
        Assert.AreEqual(new Coord(0, 0), moveResult.source);
        Assert.AreEqual(new Coord(5, 6), moveResult.destination);
    }

    // Combat test
    [Test]
    public void ShouldCreateCharacterWithGivenAttributes ()
    {
        int initialStamina = 20;
        int initialStrength = 12;
        int initialAgility = 15;
        int initialIntellect = 14;

        var attributes = new Attributes()
        {
            stamina = initialStamina,
            strength = initialStrength,
            agility = initialAgility,
            intellect = initialIntellect,
        };

        var createdCharacter = Character.Create(attributes);

        Assert.AreEqual(initialStamina, createdCharacter.stamina);
        Assert.AreEqual(initialStrength, createdCharacter.strength);
        Assert.AreEqual(initialAgility, createdCharacter.agility);
        Assert.AreEqual(initialIntellect, createdCharacter.intellect);
    }

    [Test]
    public void AllAttributesShouldBeTenByDefault()
    {
        Assert.AreEqual(10, character.stamina);
        Assert.AreEqual(10, character.strength);
        Assert.AreEqual(10, character.agility);
        Assert.AreEqual(10, character.intellect);
    }

    [Test]
    public void CharacterMaxHealthShouldBeTenTimesOfStamina()
    {
        Assert.AreEqual(10 * character.stamina, character.maxHealth);
    }

    [Test]
    public void CharacterMaxManaShouldBeTenTimesOfIntellect()
    {
        Assert.AreEqual(10 * character.intellect, character.maxMana);
    }

    [Test]
    public void ShouldSetCurrentHealthByCharactersMaxHealth()
    {
        Assert.AreEqual(character.maxHealth, character.Health.Value);
    }

    [Test]
    public void ShouldSetCurrentmanaByCharactersMaxMana()
    {
        Assert.AreEqual(character.maxMana, character.Mana.Value);
    }

    [Test]
    public void ShouldApplyHealthChange()
    {
        character = Character.Create();

        var currentHealth = character.Health.Value;
        // set minus value to deal damage to the character 
        character.ApplyHealthChange(-50);
        Assert.AreEqual(currentHealth - 50, character.Health.Value);

        // set plus value to heal the character
        currentHealth = character.Health.Value;
        character.ApplyHealthChange(10);
        Assert.AreEqual(currentHealth + 10, character.Health.Value);

        // current health should never go over maxhealth
        character.ApplyHealthChange(100);
        Assert.AreEqual(character.maxHealth, character.Health.Value);
    }

    [Test]
    public void DiesAtLessThanZeroHealth()
    {
        var isDead = false;
        Assert.IsFalse(character.IsDead);

        character.Dead
                 .Subscribe(x => isDead = x)
                 .AddTo(disposables);
        Assert.IsFalse(isDead);

        character.ApplyHealthChange(-100);
        Assert.IsTrue(isDead);
        Assert.IsTrue(character.IsDead);
    }

    [TearDown]
    public void Cleanup()
    {
        disposables.Dispose();
        disposables = new CompositeDisposable();
    }
}
