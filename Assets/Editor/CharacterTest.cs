using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;

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
                 .Subscribe(x => coordAfterMove = x)
                 .AddTo(disposables);

        character.Move(Direction.Right);
        Assert.AreEqual(1, coordAfterMove.x);
        Assert.AreEqual(0, coordAfterMove.y);

        character.Move(Direction.Up);
        Assert.AreEqual(1, coordAfterMove.x);
        Assert.AreEqual(1, coordAfterMove.y);
    }

    [TearDown]
    public void Cleanup()
    {
        disposables.Dispose();
    }
}
