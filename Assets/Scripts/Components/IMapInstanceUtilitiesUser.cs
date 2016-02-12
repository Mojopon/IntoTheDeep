using UnityEngine;
using System.Collections;
using System;

public interface IMapInstanceUtilitiesUser
{
    Func<int, int, Vector2> CoordToWorldPositionConverter { get; set; }
}
