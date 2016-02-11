using UnityEngine;
using System.Collections;
using UniRx;
using System;
using System.Collections.Generic;

public class Character : DisposableCharacter, ICharacter
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsPlayer { get; private set; }
    public Alliance Alliance { get; private set; }

    public ReactiveProperty<Coord> Location { get; private set; }
    public ReactiveProperty<int> CurrentHealth { get; private set; }
    public ReactiveProperty<bool> Dead { get; private set; }

    private List<Skill> skills = new List<Skill>();


    public Character()
    {
        this.Location = new ReactiveProperty<Coord>();
        this.CurrentHealth = new ReactiveProperty<int>(1);
        this.Dead = new ReactiveProperty<bool>(false);

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
        });

        skills.Add(new Skill
        {
            name = "キック",
        });
    }

    public bool Move(Direction direction, Func<int, int, bool> canMove)
    {
        var destination = Location.Value + direction.ToCoord();
        if (!canMove(destination.x, destination.y)) return false;

        this.Location.Value += direction.ToCoord();
        return true;
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
