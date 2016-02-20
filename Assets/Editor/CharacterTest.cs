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
        character.CurrentPhase.Where(x => x == Character.Phase.Combat).Subscribe(x => changedToCombatActionPhase = true);
        // cant move until it goes move phase
        Assert.IsFalse(character.CanMoveTo(Direction.Right));
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
    public void ShouldBeAbleToWaitInThePlace()
    {
        Assert.IsFalse(character.CanMove);
        Assert.IsFalse(character.CanMoveTo(Direction.None));

        character.SetPhase(Character.Phase.Move);
        Assert.IsTrue(character.CanMoveTo(Direction.None));
        Assert.IsTrue(character.CanMove);
        // character has chances to move 4 times per turn
        Assert.IsTrue(character.CanMove);
        Assert.IsTrue(character.CanMoveTo(Direction.None));
        Assert.IsTrue(character.Move(Direction.None));
        Assert.AreEqual(0, character.X);
        Assert.AreEqual(0, character.Y);
        Assert.IsTrue(character.CanMove);
        Assert.IsTrue(character.CanMoveTo(Direction.None));
        Assert.IsTrue(character.Move(Direction.None));
        Assert.AreEqual(0, character.X);
        Assert.AreEqual(0, character.Y);
        Assert.IsTrue(character.CanMove);
        Assert.IsTrue(character.CanMoveTo(Direction.None));
        Assert.IsTrue(character.Move(Direction.None));
        Assert.AreEqual(0, character.X);
        Assert.AreEqual(0, character.Y);
        Assert.IsTrue(character.CanMove);
        Assert.IsTrue(character.CanMoveTo(Direction.None));
        Assert.IsTrue(character.Move(Direction.None));
        Assert.AreEqual(0, character.X);
        Assert.AreEqual(0, character.Y);
        //it should be failed
        Assert.IsFalse(character.CanMove);
        Assert.IsFalse(character.CanMoveTo(Direction.None));
        Assert.False(character.Move(Direction.None));
        Assert.AreEqual(Character.Phase.Combat, character.CurrentPhase.Value);
        Assert.AreEqual(0, character.X);
        Assert.AreEqual(0, character.Y);
    }

    [Test]
    public void CanMoveShouldBeTrueWhenItCanMove()
    {
        Assert.IsFalse(character.CanMove);

        character.SetPhase(Character.Phase.Move);
        Assert.IsTrue(character.CanMove);

        // can move 4 times per turn by default
        character.Move(Direction.Right);
        Assert.IsTrue(character.CanMove);
        character.Move(Direction.Right);
        Assert.IsTrue(character.CanMove);
        character.Move(Direction.Right);
        Assert.IsTrue(character.CanMove);
        character.Move(Direction.Right);
        Assert.IsFalse(character.CanMove);
    }

    [Test]
    public void ShouldReturnMoveSucceed()
    {
        Assert.IsFalse(character.Move(Direction.Right));

        character.SetPhase(Character.Phase.Move);
        Assert.IsTrue(character.Move(Direction.Right));
        Assert.IsTrue(character.Move(Direction.Up));
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
        Assert.IsFalse(character.CanMoveTo(Direction.Up));
        character.SetPhase(Character.Phase.Move);

        Assert.IsFalse(character.CanMoveTo(Direction.Right));
        Assert.IsTrue(character.CanMoveTo(Direction.Up));
        Assert.IsTrue(character.CanMoveTo(Direction.Down));
        Assert.IsTrue(character.CanMoveTo(Direction.Left));
    }

    [Test]
    public void ShouldTransferCharacterToTheLocation()
    {
        // character can be transfered without going to move phase 
        Assert.IsTrue(character.CanTransferTo(new Coord(5, 6)));

        var moveResult = character.Transfer(new Coord(5, 6));
        Assert.AreEqual(5, character.X);
        Assert.AreEqual(6, character.Y);
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

    [Test]
    public void ShouldGoToCombatPhaseFromMovePhase()
    {
        Assert.AreEqual(Character.Phase.Idle, character.CurrentPhase.Value);

        character.SetPhase(Character.Phase.Move);
        Assert.AreEqual(Character.Phase.Move, character.CurrentPhase.Value);

        // character can move 4 times per turn by default
        Assert.IsTrue(character.Move(Direction.Right));
        Assert.IsTrue(character.Move(Direction.Right));
        Assert.IsTrue(character.Move(Direction.Right));
        Assert.IsTrue(character.Move(Direction.Right));
        Assert.AreEqual(Character.Phase.Combat, character.CurrentPhase.Value);

        Assert.IsFalse(character.Move(Direction.Right));
    }

    [Test]
    public void ShouldGoToTurnEndPhaseFromCombatPhase()
    {
        var skill = character.GetSkills()[0];

        Assert.IsFalse(character.CanUseSkill(skill));

        character.SetPhase(Character.Phase.Combat);
        Assert.AreEqual(Character.Phase.Combat, character.CurrentPhase.Value);

        Assert.IsTrue(character.CanUseSkill(skill));
        character.UseSkill(skill);
    }

    [TearDown]
    public void Cleanup()
    {
        disposables.Dispose();
        disposables = new CompositeDisposable();
    }
}
