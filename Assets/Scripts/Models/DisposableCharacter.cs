using UnityEngine;
using System.Collections;
using System;
using UniRx;

public abstract class DisposableCharacter : IDisposable
{
    public ReactiveProperty<bool> DisposedReactiveProperty = new ReactiveProperty<bool>(false);

    public CompositeDisposable Disposables = new CompositeDisposable();
    public void Dispose()
    {
        DisposedReactiveProperty.Value = true;

        Disposables.Dispose();
    }
}
