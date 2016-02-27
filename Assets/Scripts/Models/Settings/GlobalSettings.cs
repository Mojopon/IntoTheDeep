using UnityEngine;
using System.Collections;

public class GlobalSettings
{
    public float CharacterMoveSpeed = 0.1f;

    private static GlobalSettings instance = null;
    public static GlobalSettings Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GlobalSettings();
            }

            return instance;
        }
    }

    private GlobalSettings() { }
}
