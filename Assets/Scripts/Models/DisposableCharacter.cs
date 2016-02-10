using UnityEngine;
using System.Collections;
using System;
using UniRx;

public abstract class DisposableCharacter : IDisposable
{
    public CompositeDisposable Disposables = new CompositeDisposable();
    public void Dispose()
    {
        Disposables.Dispose();
    }
}
