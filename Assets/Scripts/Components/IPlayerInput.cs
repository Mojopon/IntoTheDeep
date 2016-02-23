using UnityEngine;
using System.Collections;
using UniRx;

public interface IPlayerInput
{
    IObservable<bool> OnEnterButtonObservable { get; }
    IObservable<bool> OnCancelButtonObservable { get; }
    IObservable<Direction> MoveDirectionObservable { get; }
}
