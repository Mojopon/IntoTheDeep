using UnityEngine;
using System.Collections;
using UniRx;

public interface ICharacterAttributes
{
    int stamina { get; }
    int strength { get; }
    int agility { get; }
    int intellect { get; }

    int armor { get; }
    int meleePower { get; }
    int rangePower { get; }
    int spellPower { get; }

    int maxHealth { get; }
    int maxMana { get; }
    ReactiveProperty<int> Health { get; }
    ReactiveProperty<int> Mana { get; }
}
