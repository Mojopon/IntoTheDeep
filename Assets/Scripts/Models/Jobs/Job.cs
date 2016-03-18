using UnityEngine;
using System.Collections;
using System;

public interface IJob
{
    // status gain per level
    float stamina { get; }
    float strength { get; }
    float agility { get; }
    float intellect { get; }
}

public class Job : IJob
{
    public float stamina { get; private set; }
    public float strength { get; private set; }
    public float agility { get; private set; }
    public float intellect { get; private set; }
}
