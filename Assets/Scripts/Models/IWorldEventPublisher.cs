using UnityEngine;
using System.Collections;
using UniRx;

public interface IWorldEventPublisher
{
    // actor means the character who is currently moving or doing combat 
    ReactiveProperty<Character> AddedCharacter { get; }
    ReactiveProperty<Character> CurrentActor { get; }
    ReactiveProperty<CharacterMoveResult> MoveResult { get; }
    ReactiveProperty<CharacterCombatResult> CombatResult { get; }
}
