using UnityEngine;
using System.Collections;
using NUnit.Framework;

[TestFixture]
public class EffectTest
{
    Character player;
    Character target;

    [SetUp]
    public void Initialize()
    {
        player = Character.Create();
        target = Character.Create();
    }

    [Test]
    public void ShouldDecreateTargetHealth()
    {
        var currentTargetHealth = target.Health.Value;

        var damageEffect = new DamageEffect();

        damageEffect.AffectTarget(player, target);
        Assert.IsTrue(target.Health.Value < currentTargetHealth);
    }
}
