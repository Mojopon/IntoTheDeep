using UnityEngine;
using System.Collections;
using NUnit.Framework;

[TestFixture]
public class ObjectivesTest
{
    private Objectives objectives;

    [SetUp]
    public void Initialize()
    {
        objectives = new Objectives();
    }

    [Test]
    public void ShouldReturnWhenAllObjectivesAccompleshed()
    {
        // should return true on default
        Assert.IsTrue(objectives.IsCompleted());

        var enemyCount = 2;
        objectives.AddObjective(() => enemyCount <= 0);
        Assert.IsFalse(objectives.IsCompleted());
        enemyCount--;
        Assert.IsFalse(objectives.IsCompleted());
        enemyCount--;
        // should be true when all objective is accomplished
        Assert.IsTrue(objectives.IsCompleted());

        enemyCount = 2;
        Assert.IsFalse(objectives.IsCompleted());
        var stairsFound = false;
        objectives.AddObjective(() => stairsFound);
        Assert.IsFalse(objectives.IsCompleted());
        stairsFound = true;
        Assert.IsFalse(objectives.IsCompleted());
        enemyCount = 0;
        Assert.IsTrue(objectives.IsCompleted());
    }
}
