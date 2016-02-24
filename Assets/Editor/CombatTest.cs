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

    [SetUp]
    public void Initialize()
    {
        map = new Map();
        map.Width = 10;
        map.Depth = 8;
        map.Initialize();
        world = new World(map);
    }

    [Test]
    public void UserShouldBeTheCharacterSkillUsed()
    {
        var character = CreateCharacter();

        var skill = new Skill()
        {
            name = "斬る",
            skillType = SkillType.Active,
            effectType = EffectType.Damage,
            minMultiply = 1f,
            maxMultiply = 1f,
            range = new Coord[]
            {
                new Coord(-1, 0),
                new Coord(0, 1),
            }
        };

        Assert.IsTrue(world.AddCharacter(character, 1, 1));
        var combatResult = Combat.GetCombatResult(character, skill, world.CharacterOnTheLocation, 0);
        Assert.AreEqual(character, combatResult.user);
    }

    [Test]
    public void ShouldEffectToCharactersInTheRange()
    {
        var character = CreateCharacter();
        character.SetLocation(1, 1);

        var skill = new Skill()
        {
            name = "斬る",
            skillType = SkillType.Active,
            effectType = EffectType.Damage,
            minMultiply = 1f,
            maxMultiply = 1f,
            range = new Coord[]
            {
                new Coord(-1, 0),
                new Coord(0, 1),
            }
        };


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

        var combatResult = Combat.GetCombatResult(character, skill, world.CharacterOnTheLocation, 0);

        var performances = combatResult.GetPerformances().Select(x => x.target).ToList();
        Assert.IsTrue(performances.Contains(enemyOne));
        Assert.IsTrue(performances.Contains(enemyTwo));
        Assert.IsFalse(performances.Contains(enemyThree));
    }

    [Test]
    public void ShouldPerformUsedSkill()
    {
        var character = CreateCharacter();
        character.SetLocation(1, 1);

        var skill = new Skill()
        {
            name = "斬る",
            skillType = SkillType.Active,
            effectType = EffectType.Damage,
            minMultiply = 1f,
            maxMultiply = 1f,
            range = new Coord[]
            {
                new Coord(-1, 0),
                new Coord(0, 1),
            }
        };


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

        var combatResult = Combat.GetCombatResult(character, skill, world.CharacterOnTheLocation, 0);

        var performances = combatResult.GetPerformances();
        foreach(var performance in performances)
        {
            Assert.AreEqual(skill, performance.receivedSkill);
        }
    }

    Character CreateCharacter()
    {
        return Character.Create();
    }
}
