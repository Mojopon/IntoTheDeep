using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Objectives
{
    private List<Func<bool>> objectives = new List<Func<bool>>();

    public void AddObjective(Func<bool> objective)
    {
        objectives.Add(objective);
    }

    public bool IsCompleted()
    {
        var completed = true;
        foreach(Func<bool> objective in objectives)
        {
            completed = objective() == true ? completed : false;
            if(!completed)
            {
                return false;
            }
        }

        return completed;
    }
}
