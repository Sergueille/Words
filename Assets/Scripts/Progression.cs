using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents persistent data, serialized for saving
/// </summary>
[System.Serializable]
public struct Progression
{
    public bool alreadyPlayed;
    public bool startedRun;
    public string lastVersion;
    [SerializeField] public SafeArray<int> bestLevels;
    [SerializeField] public SafeArray<bool> usedBonus;
    [SerializeField] public SafeArray<bool> usedBenedictions;
    [SerializeField] public SafeArray<bool> usedCurses;


    public static Progression GetDefaultProgression()
    {
        return new Progression {
            alreadyPlayed = false,
            startedRun = false,
            lastVersion = "None"
        };
    }

    public float GetCompletionProgression() {
        int count = 0;
        for (int i = 0; i < (int)GameMode.MaxValue; i++) {
            if (bestLevels.Get(i) >= GameManager.i.thousandLevel) count++;
        }

        return (float)count / (int)GameMode.MaxValue;
    }

    public float GetBonusProgression() {
        int count = 0;
        for (int i = 0; i < (int)BonusType.MaxValue; i++) {
            if (usedBonus.Get(i)) count++;
        }

        return (float)count / (int)BonusType.MaxValue;
    }

    public float GetCurseProgression() {
        int count = 0;
        for (int i = 0; i < Event.CURSE_COUNT; i++) {
            if (usedCurses.Get(i)) count++;
        }

        return (float)count / Event.CURSE_COUNT;
    }

    public float GetBenedictionProgression() {
        int count = 0;
        for (int i = 0; i < Event.BLESSING_COUNT; i++) {
            if (usedBenedictions.Get(i)) count++;
        }

        return (float)count / Event.BLESSING_COUNT;
    }

    public float GetOverallProgression() {
        return (GetCompletionProgression() + GetBonusProgression() + GetCurseProgression() + GetBenedictionProgression()) / 4;
    }
}


/// <summary>
/// Automatically expands the array if set out of bounds, and returns default if get out of bounds
/// Used to make sure save are backward-compatible
/// </summary>
[System.Serializable]
public struct SafeArray<T> {
    public T[] data;

    public T Get(int i) {
        if (data == null) {
            data = new T[i + 1];
        }

        if (i >= data.Length) {
            return default;
        }
        else {
            return data[i];
        }
    }

    public void Set(int i, T val) {
        if (data == null) {
            data = new T[i + 1];
        }

        if (i >= data.Length) {
            T[] tmp = new T[i + 1];

            for (int j = 0; j < data.Length; j++) {
                tmp[j] = data[j];
            }

            data = tmp;
        }

        data[i] = val;
    }
}

