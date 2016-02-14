using UnityEngine;
using System.Collections;
using UniRx;
using System;
using System.Collections.Generic;

public class CharacterMoveResult
{
    public Character target;
    public List<Direction> moveDirections;
    public CharacterMoveResult(Character target, List<Direction> moveDirections)
    {
        this.target = target;
        this.moveDirections = moveDirections;
    }
}

public class CharacterCombatActionResult
{
    public Character user;
    public Character[] targets;
    public Skill usedSkill;
}

public class Character : DisposableCharacter, ICharacter, IWorldUtilitiesUser
{
    public static int canMoveTimePerTurns = 4;

    public int X { get; private set; }
    public int Y { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsPlayer { get; private set; }
    public Alliance Alliance { get; private set; }
    public int MaxMove { get; private set; }

    public ReactiveProperty<Coord> Location { get; private set; }
    public ReactiveProperty<int> CurrentHealth { get; private set; }
    public ReactiveProperty<bool> Dead { get; private set; }

    public Func<Character, Coord, bool> MoveChecker { get; set; }
    public Func<Coord, Coord, Direction[]> Pathfinding { get; set; }

    private List<Skill> skills = new List<Skill>();


    public Character()
    {
        this.Location = new ReactiveProperty<Coord>();
        this.CurrentHealth = new ReactiveProperty<int>(1);
        this.Dead = new ReactiveProperty<bool>(false);

        this.MaxMove = canMoveTimePerTurns;
        // create as a player on default
        this.IsPlayer = true;

        // create in player side on default
        this.Alliance = Alliance.Player;

        this.CurrentHealth.Subscribe(x =>
            {
                if (x <= 0) Dead.Value = true;
                else Dead.Value = false;
            })
            .AddTo(Disposables);

        this.Location.Subscribe(coord =>
            {
                 X = coord.x;
                 Y = coord.y;
            })
            .AddTo(Disposables);

        this.Dead.Subscribe(x => IsDead = x)
                 .AddTo(Disposables);

        skills.Add(new Skill
        {
            name = "パンチ",
            range = new Coord[]
            {
                new Coord(0, 1),
            }
        });

        skills.Add(new Skill
        {
            name = "キック",
            range = new Coord[]
            {
                new Coord(1, 0),
                new Coord(-1, 0),
            }
        });

        InitializeWorldUtilities();
    }

    // initialize world utilities so it doesnt make exception before provided utilities by a provider
    void InitializeWorldUtilities()
    {
        MoveChecker = new Func<Character, Coord, bool>((chara, coord) => true);
        Pathfinding = new Func<Coord, Coord, Direction[]>((c1, c2) => new Direction[] { Direction.None });
    }

    public enum Phase
    {
        Idle,
        Move,
        CombatAction,
    }

    public ReactiveProperty<Phase> CurrentPhase = new ReactiveProperty<Phase>();
    private int canMoveTime;
    public void SetPhase(Phase phase)
    {
        CurrentPhase.Value = phase;

        switch(CurrentPhase.Value)
        {
            case Phase.Move:
                canMoveTime = MaxMove;
                break;
        }
    }

    public bool CanMove(Direction direction)
    {
        var destination = Location.Value + direction.ToCoord();
        if (!MoveChecker(this, destination)) return false;

        return true;
    }

    public void Move(Direction direction)
    {
        if (canMoveTime <= 0) return;

        this.Location.Value += direction.ToCoord();
        canMoveTime--;

        if(canMoveTime == 0)
        {
            SetPhase(Phase.CombatAction);
        }
    }

    public void OnSkillUsed(Skill skill)
    {
        // apply changes to the character after using the skill

        SetPhase(Phase.Idle);
    }

    public void ApplyHealthChange(int amount)
    {
        this.CurrentHealth.Value += amount;
    }

    public void SetIsPlayer(bool flag)
    {
        this.IsPlayer = flag;
    }

    public void SetAlliance(Alliance alliance)
    {
        this.Alliance = alliance;
    }

    public Skill[] GetSkills()
    {
        return skills.ToArray();
    }
}
