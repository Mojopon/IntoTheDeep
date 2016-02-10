using UnityEngine;
using System.Collections;
using UniRx;

public interface ICharacter
{
    int X { get; }
    int Y { get; }
    bool IsDead { get; }
    bool IsPlayer { get; }
    Alliance Alliance { get; }

    ReactiveProperty<Coord> Location { get; }
    ReactiveProperty<int> Health { get; }
    ReactiveProperty<bool> Dead { get; }
}
