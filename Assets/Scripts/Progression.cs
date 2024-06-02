using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Progression
{
    public bool alreadyPlayed;
    public bool startedRun;

    public static Progression GetDefaultProgression()
    {
        return new Progression {
            alreadyPlayed = false,
            startedRun = false,
        };
    }
}
