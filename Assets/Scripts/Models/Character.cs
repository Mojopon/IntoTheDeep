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
    public int stamina { get; set; }
    public int strength { get; set; }
    public int agility { get; set; }
    public int intellect { get; set; }
}

public class Character : DisposableCharacter, ICharacter, IWorldUtilitiesUser
{
    public static int canMoveTimePerTurns = 4;

    public bool GodMode { get; set; }
    public string Name { get; set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsPlayer { get; private set; }
    public Alliance Alliance { get; private set; }
    public int MaxMove { get; private set; }
    public bool CanMove
    {
        get
        {
            if (CurrentPhase.Value != Phase.Move || canMoveTime <= 0)
                return false;

            return true;
        }
    }
    public bool IsOnExit { get; set; }

    public enum Phase
    {
        Idle,
        Move,
        Combat,
        TurnEnd,
    }

    public ReactiveProperty<Phase> CurrentPhase { get; private set; }
    public ReactiveProperty<Coord> Location { get; private set; }
    public ReactiveProperty<bool> Dead { get; private set; }
    public ReactiveProperty<Skill> UsedSkill { get; private set; }

    public Func<Character, Coord, bool> MoveChecker { get; set; }
    public Func<Coord, Coord, Direction[]> Pathfinding { get; set; }

    #region ICharacterAttributes Property Group
    public int stamina { get; private set; }
    public int strength { get; private set; }
    public int agility { get; private set; }
    public int intellect { get; private set; }

    public int maxHealth { get { return stamina * 10; } }
    public int maxMana { get { return intellect * 10; } }

    public ReactiveProperty<int> Health { get; private set; }
    public ReactiveProperty<int> Mana { get; private set; }

    public int armor { get; private set; }
    public int meleePower { get; private set; }
    public int rangePower { get; private set; }
    public int spellPower { get; private set; }

    #endregion
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
        this.MoveChecker = new Func<Character, Coord, bool>((chara, coord) => true);
        this.Pathfinding = new Func<Coord, Coord, Direction[]>((c1, c2) => new Direction[] { Direction.None });

        this.CurrentPhase = new ReactiveProperty<Phase>(Phase.Idle);
        this.Location = new ReactiveProperty<Coord>();
        this.Dead = new ReactiveProperty<bool>(false);
        this.UsedSkill = new ReactiveProperty<Skill>();

        this.MaxMove = canMoveTimePerTurns;
        // create as a player on default
        this.IsPlayer = true;

        // create in player side on default
        this.Alliance = Alliance.Player;

        this.Health.Subscribe(x =>
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
        this.Health = new ReactiveProperty<int>(maxHealth);
        this.Mana = new ReactiveProperty<int>(maxMana);

        armor = (stamina * 10) / 2;
        meleePower = (strength * 10) / 2;
        rangePower = (agility * 10) / 2;
        spellPower = (intellect * 10) / 2;
    }

    void SetCharacterAttributes(Attributes attributes)
    {
        if (attributes == null)
        {
            this.stamina = 10;
            this.strength = 10;
            this.agility = 10;
            this.intellect = 10;
        }
        else
        {
            this.stamina = attributes.stamina;
            this.strength = attributes.strength;
            this.agility = attributes.agility;
            this.intellect = attributes.intellect;
        }
    }

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

    public bool CanMoveTo(Direction direction)
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

    public bool Move(Direction direction)
    {
        if (!CanMoveTo(direction)) return false;

        this.Location.Value += direction.ToCoord();

        if (!GodMode)
        {
            canMoveTime--;
        }
        if(canMoveTime == 0)
        {
            SetPhase(Phase.Combat);
        }

        return true;
    }

    public bool Transfer(Coord destination)
    {
        if (!CanTransferTo(destination)) return false;

        this.Location.Value = destination;

        return true;
    }

    public bool CanUseSkill(Skill skill)
    {
        if (CurrentPhase.Value != Phase.Combat) return false;

        return true;
    }

    public bool UseSkill(Skill skill)
    {
        if (!CanUseSkill(skill)) return false;
        // reset value or it wont update same value
        UsedSkill.Value = null;

        UsedSkill.Value = skill;
        SetPhase(Phase.TurnEnd);

        return true;
    }

    public void ApplyHealthChange(int change)
    {
        if(maxHealth < Health.Value + change)
        {
            change = maxHealth - Health.Value;
        }

        Health.Value += change;
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

    public override string ToString()
    {
        return string.Format("[{0}] Health:{1}", Name, Health.Value);
    }
}
