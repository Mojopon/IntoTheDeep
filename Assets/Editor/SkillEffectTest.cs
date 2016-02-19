using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;

[TestFixture]
public class SkillEffectTest
{
    [Test]
    public void ShouldDecreaseTargetHealth()
    {
        var character = Character.Create();

        var effect = new DamageSkillEffect(character, 50);
        Assert.AreEqual(100, character.Health.Value);
        effect.Apply();
        Assert.AreEqual(50, character.Health.Value);
    }

    [Test]
    public void NeverBeAppliedTwice()
    {
        var character = Character.Create();

        var effect = new DamageSkillEffect(character, 50);
        Assert.AreEqual(100, character.Health.Value);
        effect.Apply();
        Assert.AreEqual(50, character.Health.Value);
        Assert.IsTrue(effect.IsApplied);
        effect.Apply();
        Assert.AreEqual(50, character.Health.Value);
    }

    [Test]
    public void DamageSkillEffectShouldReturnDamageType()
    {
        var character = Character.Create();
        var effect = new DamageSkillEffect(character, 50);

        Assert.AreEqual(EffectType.Damage, effect.EffectType);
    }
}
