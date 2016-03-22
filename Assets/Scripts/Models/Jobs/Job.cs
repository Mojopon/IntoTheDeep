using UnityEngine;
using System.Collections;
using System;

public interface IJob
{
    int id      { get; }
    string name { get; }

    int stamina    { get; }
    int strength   { get; }
    int agility    { get; }
    int intellect  { get; }

    float staminaGain   { get; }
    float strengthGain  { get; }
    float agilityGain   { get; }
    float intellectGain { get; }
}

public class Job : IJob
{
    public int    id   { get; set; }
    public string name { get; set; }

    public int stamina    { get; set; }
    public int strength   { get; set; }
    public int agility    { get; set; }
    public int intellect  { get; set; }

    public float staminaGain    { get; set; }
    public float strengthGain   { get; set; }
    public float agilityGain    { get; set; }
    public float intellectGain  { get; set; }
}
