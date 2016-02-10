using UnityEngine;
using System.Collections;
using NUnit.Framework;

[TestFixture]
public class EnemiesTest
{
    [Test]
    public void ShouldReturnTrueWhenAllEnemiesAreDead()
    {
        var enemyOne = new Character();
        var enemyTwo = new Character();

        var enemies = new Enemies();
        Assert.IsTrue(enemies.IsAnnihilated);

        enemies.AddCharacterAsEnemy(enemyOne);
        enemies.AddCharacterAsEnemy(enemyTwo);

        Assert.IsFalse(enemies.IsAnnihilated);
        enemyOne.ApplyHealthChange(-10);
        enemyTwo.ApplyHealthChange(-10);

        Assert.IsTrue(enemies.IsAnnihilated);
    }
}
