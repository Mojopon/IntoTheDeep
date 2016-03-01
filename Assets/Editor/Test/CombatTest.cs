using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Linq;

[TestFixture]
public class CombatTest
{
    World world;
    Map map;
    Character character;
    Character enemy;
    Skill skill;

    [SetUp]
    public void Initialize()
    {
        map = new Map();
        map.Width = 10;
        map.Depth = 8;
        map.Initialize();
        world = new World(map);

        skill = new Skill
        {
            name = "斬る",
            effects = new Effect[]
            {
                new DamageEffect()
                {
                    ranges = new Coord[] { new Coord(-1, 0), new Coord(0, 1)},
                    effectAttribute = EffectAttribute.MeleePower,
                    minMultiply = 1f,
                    maxMultiply = 1f,
                }
            }
        };
    }

    [Test]
    public void UserShouldBeTheCharacterSkillUsed()
    {
        var character = CreateCharacter();

        Assert.IsTrue(world.AddCharacter(character, 1, 1));
        var combatResult = Combat.DoCombat(character, skill, world.CharacterOnTheLocation, 0);
        Assert.AreEqual(character, combatResult.user);
    }

    [Test]
    public void ShouldEffectToCharactersInTheRange()
    {
        var character = CreateCharacter();
        character.SetLocation(1, 1);

        var enemyOne = CreateCharacter();
        enemyOne.SetLocation(0, 1);
        var enemyTwo = CreateCharacter();
        enemyTwo.SetLocation(1, 2);
        var enemyThree = CreateCharacter();
        enemyThree.SetLocation(0, 0);

        Assert.IsTrue(world.AddCharacter(character, 1, 1));
        Assert.IsTrue(world.AddCharacterAsEnemy(enemyOne, 0, 1));
        Assert.IsTrue(world.AddCharacterAsEnemy(enemyTwo, 1, 2));
        Assert.IsTrue(world.AddCharacterAsEnemy(enemyThree, 0, 0));

        var combatResult = Combat.DoCombat(character, skill, world.CharacterOnTheLocation, 0);

        var performances = combatResult.GetCombatLog().Select(x => x.target).ToList();
        Assert.IsTrue(performances.Contains(enemyOne));
        Assert.IsTrue(performances.Contains(enemyTwo));
        Assert.IsFalse(performances.Contains(enemyThree));
    }

    [Test]
    public void ShouldCreateCombatLog()
    {
        var character = CreateCharacter();
        character.SetLocation(1, 1);

        var enemyOne = CreateCharacter();
        enemyOne.SetLocation(0, 1);
        var enemyTwo = CreateCharacter();
        enemyTwo.SetLocation(1, 2);
        var enemyThree = CreateCharacter();
        enemyThree.SetLocation(0, 0);

        Assert.IsTrue(world.AddCharacter(character, 1, 1));
        Assert.IsTrue(world.AddCharacterAsEnemy(enemyOne, 0, 1));
        Assert.IsTrue(world.AddCharacterAsEnemy(enemyTwo, 1, 2));
        Assert.IsTrue(world.AddCharacterAsEnemy(enemyThree, 0, 0));

        var combatResult = Combat.DoCombat(character, skill, world.CharacterOnTheLocation, 0);

        var combatLogs = combatResult.GetCombatLog();

        foreach(var combatLog in combatLogs)
        {
            Assert.IsTrue(combatLog.target == enemyOne || combatLog.target == enemyTwo || combatLog.target == enemyThree);
            Assert.IsTrue(combatLog.combatType == CombatLog.CombatType.Damage);
        }
    }

    Character CreateCharacter()
    {
        return Character.Create();
    }
}
