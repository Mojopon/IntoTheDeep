using UnityEngine;
using System.Collections;
using UniRx;

public interface ICharacterAttributes
{
    int maxHealth { get; }
    int maxStrength { get; }

    ReactiveProperty<int> CurrentHealth { get; }
    ReactiveProperty<int> CurrentStrength { get; }
}
