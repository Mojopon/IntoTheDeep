using UnityEngine;
using System.Collections;

public interface IMapEvent
{
    void Apply(Map targetMap);
}
