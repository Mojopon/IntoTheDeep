using UnityEngine;
using System.Collections;
using UniRx;

public interface ICharacter
{
    ReactiveProperty<Vector2> Location { get; }
    ReactiveProperty<int> Health { get; }
}
