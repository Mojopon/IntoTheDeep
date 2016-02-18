using UnityEngine;
using System.Collections;
using UniRx;
using System;
using System.Collections.Generic;

public class CharacterMoveResult
{
    public Character target;
    public Coord source;
    public Coord destination;
    public CharacterMoveResult(Character target, Coord source, Coord destination)
    {
        this.target = target;
        this.source = source;
        this.destination = destination;
    }
}

public class Attributes
{
    public int health;
    public int strength;
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
    public ReactiveProperty<bool> Dead { get; private set; }

    public Func<Character, Coord, bool> MoveChecker { get; set; }
    public Func<Coord, Coord, Direction[]> Pathfinding { get; set; }

    // character attributes
    public int maxHealth { get; private set; }
    public int maxStrength { get; private set; }

    public ReactiveProperty<int> CurrentHealth { get; private set; }
    public ReactiveProperty<int> CurrentStrength { get; private set; }

    private List<Skill> skills = new List<Skill>();

    public static Character Create(Attributes initialAttributes)
    {
        return new Character(initialAttributes);
    }

    public static Character Create()
    {
        return new Character();
    }

    protected Character() : this(null) { }

    protected Character(Attributes initialAttributes)
    {
        SetCharacterAttributes(initialAttributes);
        InitializeAttributes();

        Initialize();
    }

    void Initialize()
    {
        // initialize world utilities first so it doesnt make exceptions
        MoveChecker = new Func<Character, Coord, bool>((chara, coord) => true);
        Pathfinding = new Func<Coord, Coord, Direction[]>((c1, c2) => new Direction[] { Direction.None });

        this.Location = new ReactiveProperty<Coord>();
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
    }

    void InitializeAttributes()
    {
        this.CurrentHealth = new ReactiveProperty<int>(maxHealth);
        this.CurrentStrength = new ReactiveProperty<int>(maxStrength);
    }

    void SetCharacterAttributes(Attributes attributes)
    {
        if (attributes == null)
        {
            this.maxHealth = 10;
            this.maxStrength = 5;
        }
        else
        {
            this.maxHealth = attributes.health;
            this.maxStrength = attributes.strength;
        }
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

    public void SetLocation(int x, int y)
    {
        SetLocation(new Coord(x, y));
    }

    public void SetLocation(Coord location)
    {
        if (this.X != location.x || this.Y != location.y)
        {
            this.Location.Value = location;
        }
    }

    public bool CanMove(Direction direction)
    {
        if (canMoveTime <= 0) return false;
        var destination = Location.Value + direction.ToCoord();
        if (!MoveChecker(this, destination)) return false;

        return true;
    }

    public bool CanTransferTo(Coord destination)
    {
        if (!MoveChecker(this, destination)) return false;

        return true;
    }

    public CharacterMoveResult Move(Direction direction)
    {
        if (!CanMove(direction)) return null;

        var locationBeforeMove = this.Location.Value;
        this.Location.Value += direction.ToCoord();

        var moveResult = new CharacterMoveResult(this, locationBeforeMove, this.Location.Value);
        canMoveTime--;

        if(canMoveTime == 0)
        {
            SetPhase(Phase.CombatAction);
        }

        return moveResult;
    }

    public CharacterMoveResult Transfer(Coord destination)
    {
        if (!CanTransferTo(destination)) return null;

        var locationBeforeMove = this.Location.Value;
        this.Location.Value = destination;

        return new CharacterMoveResult(this, locationBeforeMove, this.Location.Value);
    }

    public void OnSkillUsed(Skill skill)
    {
        // apply changes to the character after using the skill

        SetPhase(Phase.Idle);
    }

    public void ApplyAttributeChanges(Attributes changes)
    {
        CurrentHealth.Value += changes.health;
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

    public void GetWorldUtilities(IWorldUtilitiesProvider provider)
    {
        MoveChecker = provider.MoveChecker;
        Pathfinding = provider.Pathfinding;
    }
}
