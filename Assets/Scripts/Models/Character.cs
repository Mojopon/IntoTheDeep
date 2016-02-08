using UnityEngine;
using System.Collections;
using UniRx;

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

    public void Move(Direction direction)
    {
        Location.Value += direction.ToCoord();
    }
}
