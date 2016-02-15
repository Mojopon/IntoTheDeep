using UnityEngine;
using System.Collections;

public interface IWorldEventSubscriber
{
    System.IDisposable Subscribe(IWorldEventPublisher publisher);
}
