using UnityEngine;
using System.Collections;
using UniRx;
using System;

public class Character
{
    public int X = 0;
    public int Y = 0;

    public ReactiveProperty<Coord> Location { get; private set; }
    public ReactiveProperty<int> Health { get; private set; }

    private CompositeDisposable disposables = new CompositeDisposable();

    public Character()
    {
        Location = new ReactiveProperty<Coord>();
        Health = new ReactiveProperty<int>();

        Location.Subscribe(coord =>
        {
            X = coord.x;
            Y = coord.y;
        })
        .AddTo(disposables);
    }

    public bool Move(Direction direction, Func<int, int, bool> canMove)
    {
        var destination = Location.Value + direction.ToCoord();
        if (!canMove(destination.x, destination.y)) return false;

        Location.Value += direction.ToCoord();
        return true;
    }
}
