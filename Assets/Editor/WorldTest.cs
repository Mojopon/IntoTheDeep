using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UniRx;

[TestFixture]
public class WorldTest
{
    Map map;
    World world;
    [SetUp]
    public void Initialize()
    {
        map = new Map();
        map.Width = 10;
        map.Depth = 10;
        map.Initialize();
        world = new World(map);
    }

    [Test]
    public void ShouldReturnTrueWhenAllEnemiesAreDead()
    {
        var enemyOne = Character.Create();
        var enemyTwo = Character.Create();
        Assert.IsTrue(world.EnemyIsAnnihilated);

        world.AddCharacterAsEnemy(enemyOne);
        world.AddCharacterAsEnemy(enemyTwo);

        Assert.IsFalse(world.EnemyIsAnnihilated);
        enemyOne.ApplyHealthChange(-100);
        enemyTwo.ApplyHealthChange(-100);

        Assert.IsTrue(world.EnemyIsAnnihilated);
    }

    [Test]
    public void ShouldUpdateAddedCharacterWhenCharacterAdded()
    {
        var character = Character.Create();
        Character addedCharacter = null;
        world.AddedCharacter.Subscribe(x => addedCharacter = x);
        world.AddCharacter(character);
        Assert.IsNotNull(addedCharacter);
        Assert.AreEqual(addedCharacter, character);
    }

    [Test]
    public void ShouldGetCharacterInTheCell()
    {
        var character = Character.Create();
        character.SetLocation(0, 0);
        world.AddCharacter(character);
        Assert.AreEqual(character, world.GetCharacter(new Coord(0, 0)));
    }

    [Test]
    public void CantAddToTheCellCantMoveTo()
    {
        var character = Character.Create();
        Assert.IsTrue(world.AddCharacter(character, 0, 0));

        var characterTwo = Character.Create();
        // false meand we couldnt add the character to the world(in the place)
        Assert.False(world.AddCharacter(characterTwo, 0, 0));
    }

    [Test]
    public void CantMoveToTheObstacleAfterAddedToTheWorld()
    {
        var character = Character.Create();
        character.SetPhase(Character.Phase.Move);
        Assert.IsTrue(character.CanMove(Direction.Right));
        Assert.AreEqual(0, character.X);
        Assert.AreEqual(0, character.Y);

        map = new Map();
        map.Width = 5;
        map.Depth = 5;
        map.Initialize();
        map.GetCell(1, 0).canWalk = false;
        world = new World(map);

        world.AddCharacter(character);
        Assert.IsFalse(character.CanMove(Direction.Right));
        character.Move(Direction.Right);
        Assert.AreEqual(0, character.X);
        Assert.AreEqual(0, character.Y);
    }

    [Test]
    public void CantTransferToTheObstacleOrOtherCharacter()
    {
        var character = Character.Create();
        Assert.IsTrue(character.CanTransferTo(new Coord(1, 0)));

        map = new Map();
        map.Width = 5;
        map.Depth = 5;
        map.Initialize();
        map.GetCell(1, 0).canWalk = false;
        world = new World(map);

        Assert.True(world.AddCharacter(character));
        // cant transfer out of the map
        Assert.IsFalse(character.CanTransferTo(new Coord(2, 6)));
        Assert.IsFalse(character.CanTransferTo(new Coord(-1, 0)));

        Assert.IsFalse(character.CanTransferTo(new Coord(1, 0)));
        Assert.IsFalse(character.Transfer(new Coord(1, 0)));

        Assert.IsTrue(character.CanTransferTo(new Coord(2, 0)));
        var characterTwo = Character.Create();
        characterTwo.SetLocation(2, 0);
        Assert.True(world.AddCharacter(characterTwo));
        Assert.IsFalse(character.CanTransferTo(new Coord(2, 0)));
        Assert.IsFalse(character.Transfer(new Coord(2, 0)));
    }

    [Test]
    public void CantMoveToTheOccupiedPlace()
    {
        var character = Character.Create();
        character.SetPhase(Character.Phase.Move);
        var characterTwo = Character.Create();
        characterTwo.SetPhase(Character.Phase.Move);
        var enemy = Character.Create();

        Assert.IsTrue(world.AddCharacter(character, 0, 1));
        Assert.IsTrue(world.AddCharacter(characterTwo, 1, 1));
        Assert.IsTrue(world.AddCharacter(enemy, 0, 2));

        Assert.IsTrue(character.CanMove(Direction.Down));
        Assert.IsFalse(character.CanMove(Direction.Right));
        Assert.IsFalse(character.CanMove(Direction.Up));

        Assert.IsTrue(characterTwo.CanMove(Direction.Right));

        characterTwo.SetPhase(Character.Phase.Move);
        world.ApplyMove(characterTwo, Direction.Right);
        Assert.AreEqual(2, characterTwo.X);
        Assert.AreEqual(1, characterTwo.Y);

        Assert.IsTrue(character.CanMove(Direction.Down));
        Assert.IsTrue(character.CanMove(Direction.Right));
        Assert.IsFalse(character.CanMove(Direction.Up));
    }

    [Test]
    public void ShouldReturnNextCharacterAndSetItToBeMovePhase()
    {
        // returns null when no characters in the world
        Assert.IsNull(world.CurrentActor.Value);

        Character currentActor = null;
        world.CurrentActor.Subscribe(x => currentActor = x);

        var character = Character.Create();
        var characterTwo = Character.Create();

        Assert.IsTrue(world.AddCharacter(character, 0, 0));
        Assert.IsTrue(world.AddCharacter(characterTwo, 1, 1));

        Assert.AreEqual(Character.Phase.Idle, character.CurrentPhase.Value);
        Assert.AreEqual(Character.Phase.Idle, characterTwo.CurrentPhase.Value);

        world.GoNextCharacterPhase();
        Assert.AreEqual(character, currentActor);
        Assert.AreEqual(Character.Phase.Move, character.CurrentPhase.Value);
        Assert.AreEqual(Character.Phase.Idle, characterTwo.CurrentPhase.Value);
        character.SetPhase(Character.Phase.Idle);

        world.GoNextCharacterPhase();
        Assert.AreEqual(characterTwo, currentActor);
        Assert.AreEqual(Character.Phase.Idle, character.CurrentPhase.Value);
        Assert.AreEqual(Character.Phase.Move, characterTwo.CurrentPhase.Value);
        characterTwo.SetPhase(Character.Phase.Idle);

        world.GoNextCharacterPhase();
        Assert.AreEqual(character, currentActor);
        Assert.AreEqual(Character.Phase.Move, character.CurrentPhase.Value);
        Assert.AreEqual(Character.Phase.Idle, characterTwo.CurrentPhase.Value);
    }

    [Test]
    public void ApplyMovementTotheCharacterAndtheMap()
    {
        var character = Character.Create();
        world.AddCharacter(character);
        CharacterMoveResult moveResult = null;
        world.MoveResult.Subscribe(x => moveResult = x);

        Assert.AreEqual(0, character.X);
        Assert.AreEqual(0, character.Y);

        Assert.IsTrue(map.GetCell(0, 0).hasCharacter);
        Assert.AreEqual(character, map.GetCell(0, 0).characterInTheCell);

        world.GoNextCharacterPhase();
        Assert.AreEqual(character, world.CurrentActor.Value);

        Assert.IsTrue(character.CanMove(Direction.Right));
        world.ApplyMove(character, Direction.Right);
        Assert.AreEqual(1, character.X);
        Assert.AreEqual(0, character.Y);
        // MoveResult property should be updated when applied move 
        Assert.IsNotNull(moveResult);
        Assert.AreEqual(character, moveResult.target);
        Assert.AreEqual(new Coord(0, 0), moveResult.source);
        Assert.AreEqual(new Coord(1, 0), moveResult.destination);
        Assert.IsFalse(map.GetCell(0, 0).hasCharacter);
        Assert.IsNull(map.GetCell(0, 0).characterInTheCell);

        Assert.IsTrue(map.GetCell(1, 0).hasCharacter);
        Assert.AreEqual(character, map.GetCell(1, 0).characterInTheCell);
    }

    [Test]
    public void ShouldApplyTransferResult()
    {
        var character = Character.Create();
        world.AddCharacter(character);
        CharacterMoveResult moveResult = null;
        world.MoveResult.Subscribe(x => moveResult = x);

        // failed transfer doesnt make moveResult changed
        Assert.IsFalse(character.CanTransferTo(new Coord(11, 0)));
        world.ApplyTransfer(character, new Coord(11, 0));
        Assert.IsNull(moveResult);
        Assert.AreEqual(0, character.X);
        Assert.AreEqual(0, character.Y);

        Assert.True(character.CanTransferTo(new Coord(5, 2)));
        world.ApplyTransfer(character, new Coord(5, 2));
        Assert.AreEqual(character, moveResult.target);
        Assert.AreEqual(new Coord(0, 0), moveResult.source);
        Assert.AreEqual(new Coord(5, 2), moveResult.destination);
    }


    [Test]
    public void ApplyTransferToTheCharacter()
    {
        var character = Character.Create();
        world.AddCharacter(character);
        CharacterMoveResult moveResult = null;
        world.MoveResult.Subscribe(x => moveResult = x);

        Assert.AreEqual(0, character.X);
        Assert.AreEqual(0, character.Y);

        Assert.IsTrue(map.GetCell(0, 0).hasCharacter);
        Assert.AreEqual(character, map.GetCell(0, 0).characterInTheCell);
    }

    [Test]
    public void ShouldSetToBeEnemyAllianceWhenAddedAsEnemy()
    {
        var enemy = Character.Create();
        //should be created as player on default
        Assert.AreEqual(Alliance.Player, enemy.Alliance);
        world.AddCharacterAsEnemy(enemy);
        Assert.AreEqual(Alliance.Enemy, enemy.Alliance);
    }

    [Test]
    public void ShouldReturnHostiles()
    {
        var character = Character.Create();
        var characterTwo = Character.Create();

        var enemy = Character.Create();
        var enemyTwo = Character.Create();

        
        Assert.IsTrue(world.AddCharacter(character, 0, 0));
        Assert.IsTrue(world.AddCharacter(characterTwo, 1, 1));

        Assert.IsTrue(world.AddCharacterAsEnemy(enemy, 2, 2));
        Assert.IsTrue(world.AddCharacterAsEnemy(enemyTwo, 3, 3));

        var playersHostiles = world.GetAllHostiles(character);

        Assert.IsTrue(playersHostiles.Contains(enemy));
        Assert.IsTrue(playersHostiles.Contains(enemyTwo));
        Assert.IsFalse(playersHostiles.Contains(character));
        Assert.IsFalse(playersHostiles.Contains(characterTwo));

        playersHostiles = world.GetAllHostiles(characterTwo);

        Assert.IsTrue(playersHostiles.Contains(enemy));
        Assert.IsTrue(playersHostiles.Contains(enemyTwo));
        Assert.IsFalse(playersHostiles.Contains(character));
        Assert.IsFalse(playersHostiles.Contains(characterTwo));

        var enemysHostiles = world.GetAllHostiles(enemy);
        Assert.IsFalse(enemysHostiles.Contains(enemy));
        Assert.IsFalse(enemysHostiles.Contains(enemyTwo));
        Assert.IsTrue(enemysHostiles.Contains(character));
        Assert.IsTrue(enemysHostiles.Contains(characterTwo));

        enemysHostiles = world.GetAllHostiles(enemyTwo);
        Assert.IsFalse(enemysHostiles.Contains(enemy));
        Assert.IsFalse(enemysHostiles.Contains(enemyTwo));
        Assert.IsTrue(enemysHostiles.Contains(character));
        Assert.IsTrue(enemysHostiles.Contains(characterTwo));
    }

    [Test]
    public void ShouldReturnClosestHostile()
    {
        var character = Character.Create();
        var characterTwo = Character.Create();

        var enemy = Character.Create();
        var enemyTwo = Character.Create();

        world.AddCharacter(character, 1, 1);
        world.AddCharacter(characterTwo, 5, 5);

        //should return null when theres no hostiles
        Assert.IsNull(world.GetClosestHostile(character));
        Assert.IsNull(world.GetClosestHostile(characterTwo));

        world.AddCharacterAsEnemy(enemy, 4, 4);
        world.AddCharacterAsEnemy(enemyTwo, 3, 3);

        var closestHostile = world.GetClosestHostile(character);
        Assert.AreEqual(enemyTwo, closestHostile);

        closestHostile = world.GetClosestHostile(characterTwo);
        Assert.AreEqual(enemy, closestHostile);

        closestHostile = world.GetClosestHostile(enemy);
        Assert.AreEqual(characterTwo, closestHostile);

        closestHostile = world.GetClosestHostile(enemyTwo);
        Assert.AreEqual(character, closestHostile);
    }

    [Test]
    public void ShouldDealDamageToTheCharacter()
    {
        var character = Character.Create();
        character.SetLocation(1, 1);

        var enemyOne = Character.Create();
        enemyOne.SetLocation(0, 1);
        var enemyTwo = Character.Create();
        enemyTwo.SetLocation(1, 2);
        var enemyThree = Character.Create();
        enemyThree.SetLocation(0, 0);

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

        Assert.IsTrue(world.AddCharacter(character));
        Assert.IsTrue(world.AddCharacterAsEnemy(enemyOne));
        Assert.IsTrue(world.AddCharacterAsEnemy(enemyTwo));
        Assert.IsTrue(world.AddCharacterAsEnemy(enemyThree));

        // characters health should be 100 by default
        Assert.AreEqual(100, enemyOne.Health.Value);
        Assert.AreEqual(100, enemyTwo.Health.Value);
        Assert.AreEqual(100, enemyThree.Health.Value);

        world.ApplyUseSkill(character, skill);

        Assert.IsTrue(100 > enemyOne.Health.Value);
        Assert.IsTrue(100 > enemyTwo.Health.Value);
        Assert.IsTrue(100 == enemyThree.Health.Value);
    }

    [Test]
    public void ShouldUpdateCombatResult()
    {
        CharacterCombatResult result = null;
        world.CombatResult.Subscribe(x => result = x);

        var character = Character.Create();
        character.SetLocation(1, 1);

        var enemyOne = Character.Create();
        enemyOne.SetLocation(0, 1);
        var enemyTwo = Character.Create();
        enemyTwo.SetLocation(1, 2);
        var enemyThree = Character.Create();
        enemyThree.SetLocation(0, 0);

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

        Assert.IsTrue(world.AddCharacter(character));
        Assert.IsTrue(world.AddCharacterAsEnemy(enemyOne));
        Assert.IsTrue(world.AddCharacterAsEnemy(enemyTwo));
        Assert.IsTrue(world.AddCharacterAsEnemy(enemyThree));

        // characters health should be 100 by default
        Assert.AreEqual(100, enemyOne.Health.Value);
        Assert.AreEqual(100, enemyTwo.Health.Value);
        Assert.AreEqual(100, enemyThree.Health.Value);

        world.ApplyUseSkill(character, skill);
        Assert.IsNotNull(result);
        Assert.AreEqual(result, world.CombatResult.Value);

        Assert.AreEqual(character, result.user);
        Assert.AreEqual(skill, result.usedSkill);

        var enemyOneHealth = enemyOne.Health.Value;
        var enemyTwoHealth = enemyTwo.Health.Value;

        var oldResult = result;

        Assert.IsTrue(enemyOneHealth == enemyOne.Health.Value);
        Assert.IsTrue(enemyTwoHealth == enemyTwo.Health.Value);
        Assert.IsTrue(100 == enemyThree.Health.Value);

        world.ApplyUseSkill(character, skill);
        Assert.AreNotEqual(oldResult, world.CombatResult.Value);
        Assert.AreNotEqual(oldResult, result);

        Assert.IsTrue(enemyOneHealth > enemyOne.Health.Value);
        Assert.IsTrue(enemyTwoHealth > enemyTwo.Health.Value);
        Assert.IsTrue(100 == enemyThree.Health.Value);
    }
}
