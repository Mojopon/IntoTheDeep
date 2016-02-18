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
    public void ShouldEffectToCharactersInTheRange()
    {
        var character = CreateCharacter();
        character.SetLocation(1, 1);

        var skill = new Skill()
        {
            name = "斬る",
            skillType = SkillType.Active,
            effectType = EffectType.Damage,
            power = 1,
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

        Assert.IsTrue(world.AddCharacter(character));
        Assert.IsTrue(world.AddCharacterAsEnemy(enemyOne));
        Assert.IsTrue(world.AddCharacterAsEnemy(enemyTwo));
        Assert.IsTrue(world.AddCharacterAsEnemy(enemyThree));

        var combatResult = Combat.GetCombatResult(character, skill, new System.Func<Coord, Character>((coord) => world.GetCharacter(coord)), 0);

        var performances = combatResult.GetPerformances().Select(x => x.target).ToList();
        Assert.IsTrue(performances.Contains(enemyOne));
        Assert.IsTrue(performances.Contains(enemyTwo));
        Assert.IsFalse(performances.Contains(enemyThree));
    }

    [Test]
    public void ShouldDealDamageToTheCharacter()
    {
        Assert.Fail();
    } 

    Character CreateCharacter()
    {
        return Character.Create();
    }
}
