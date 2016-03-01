﻿using UnityEngine;
using System.Collections;

public class Attributes
{
    public int stamina { get; set; }
    public int strength { get; set; }
    public int agility { get; set; }
    public int intellect { get; set; }

    public Attributes()
    {
        this.stamina = 10;
        this.strength = 10;
        this.agility = 10;
        this.intellect = 10;
    }
}

public class CharacterDataTable
{
    public string name { get; set; }

    public Attributes attributes { get; set; }

    public CharacterDataTable()
    {
        this.name = "Unknown";
        this.attributes = new Attributes();
    }

    public CharacterDataTable(string name, Attributes attributes)
    {
        this.name = name;

        this.attributes = attributes;
    }
}