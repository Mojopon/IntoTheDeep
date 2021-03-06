﻿using UnityEngine;
using System.Collections;
using UniRx;

public interface ICharacter : ICharacterAttributes
{
    string Name { get; }
    int X { get; }
    int Y { get; }
    bool IsDead { get; }
    bool IsPlayer { get; }
    Alliance Alliance { get; }
    int MaxMove { get; }
    bool CanMove { get; }
    bool IsOnExit { get; }

    ReactiveProperty<Character.Phase> CurrentPhase { get; }
    ReactiveProperty<Coord> Location { get; }
    ReactiveProperty<bool> Dead { get; }
    ReactiveProperty<Skill> UsedSkill { get; }
}
