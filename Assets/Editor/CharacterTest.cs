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
        character = new Character();
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
    public void DiesAtLessThanZeroHealth()
    {
        var isDead = false;
        Assert.IsFalse(character.IsDead);

        character.Dead
                 .Subscribe(x => isDead = x)
                 .AddTo(disposables);
        Assert.IsFalse(isDead);

        character.ApplyHealthChange(-50);
        Assert.IsTrue(isDead);
        Assert.IsTrue(character.IsDead);
    }

    [Test]
    public void ShouldReturnCanMoveorNot()
    {
        character.MoveChecker = new Func<Character, Coord, bool>((chara, c) =>
                                                                {
                                                                    if (c.x == 1 && c.y == 0) return false;

                                                                    return true;
                                                                });

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

    [TearDown]
    public void Cleanup()
    {
        disposables.Dispose();
        disposables = new CompositeDisposable();
    }
}
