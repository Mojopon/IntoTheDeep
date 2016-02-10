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
        var moveChecker = new Func<int, int, bool>((x, y) => true);

        character.Location
                 .Subscribe(x =>
                 {
                     coordAfterMove = x;
                     Debug.Log(x);
                 }).AddTo(disposables);
                 

        character.Move(Direction.Right, moveChecker);
        Assert.AreEqual(1, coordAfterMove.x);
        Assert.AreEqual(0, coordAfterMove.y);

        character.Move(Direction.Up, moveChecker);
        Assert.AreEqual(1, coordAfterMove.x);
        Assert.AreEqual(1, coordAfterMove.y);
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

    [TearDown]
    public void Cleanup()
    {
        disposables.Dispose();
        disposables = new CompositeDisposable();
    }
}
