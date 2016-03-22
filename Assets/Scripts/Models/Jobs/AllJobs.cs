using UnityEngine;
using System.Collections.Generic;

public class AllJobs
{
    List<Job> allJobDatas = new List<Job>()
    {
        new Job()
        {
            id   = 0,
            name = "無職",

            stamina   = 0,
            strength  = 0,
            agility   = 0,
            intellect = 0,

            staminaGain   = 0,
            strengthGain  = 0,
            agilityGain   = 0,
            intellectGain = 0,
        },

        new Job()
        {
            id   = 1,
            name = "剣士",

            stamina   = 5,
            strength  = 10,
            agility   = 3,
            intellect = 2,

            staminaGain   = 2f,
            strengthGain  = 2.5f,
            agilityGain   = 1f,
            intellectGain = 1f,
        },

    };
}
