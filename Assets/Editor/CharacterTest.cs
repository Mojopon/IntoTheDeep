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
        int initialHealth = 20;
        int initialStrength = 10;

        var attributes = new Attributes()
        {
            health = initialHealth,
            strength = initialStrength,
        };

        var createdCharacter = Character.Create(attributes);

        Assert.AreEqual(initialHealth, createdCharacter.maxHealth);
        Assert.AreEqual(initialStrength, createdCharacter.maxStrength);
    }

    [Test]
    public void ShouldApplyAttributesChange()
    {
        int initialHealth = 20;
        int initialStrength = 10;

        var attributes = new Attributes()
        {
            health = initialHealth,
            strength = initialStrength,
        };

        character = Character.Create(attributes);

        var attributeChanges = new Attributes()
        {
            health = -15,
            strength = -2,
        };

        character.ApplyAttributeChanges(attributeChanges);
        Assert.AreEqual(5, character.CurrentHealth.Value);
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

        var attributeChanges = new Attributes()
        {
            health = -15,
        };
        character.ApplyAttributeChanges(attributeChanges);
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
