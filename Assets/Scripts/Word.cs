using System.Collections.Generic;
using UnityEngine;

public static class Word 
{
    public static Dictionary<string, GameWord> words;

    public static void Init() 
    {
        words = new();

        string fileText = ((TextAsset)Resources.Load("words")).text;
        string[] lines = fileText.Split('\n');

        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            words.Add(trimmed, new GameWord {
                word = trimmed,
                timesUsed = 0,
            });
        }
    }

    public static bool IsWordAllowed(string word) 
    {
        return words.ContainsKey(word);
    }

    public static GameWord GetWord(string word) 
    {
        return words[word];
    }
}


[System.Serializable]
public class Letter : System.ICloneable, System.IComparable
{
    [System.Serializable]
    public enum Effect {
        None, 
        Poisonous, Doomed, Locked,
        Doubled, Polymorphic,
        Electric, Burning,
    }

    public char letter;

    [SerializeField]
    private int _level;
    public int Level {
        get => _level;
        set {
            if (effect != Effect.Locked) {
                _level = value; 
            }
        }
    }

    public int timesUsed = 0;
    public Effect effect;

    /// <summary>
    /// Get the score the letter will give.
    /// </summary>
    /// <param name="played">Is the letter played right now? If yes, wWill apply on play effects.</param>
    public int GetScore(bool played) {
        if (effect == Effect.Doubled) {
            return 2 * Level;
        }
        else if (effect == Effect.Poisonous) {
            if (played) {
                Util.GetRandomElement(GameManager.i.gi.letters).Level--;
            }
        }
        else if (effect == Effect.Doomed) {
            if (played) {
                Level--;
            }
        }
        else if (effect == Effect.Burning) {
            if (played) {
                Level++;
            }
        }
        else if (effect == Effect.Electric) {
            if (played) {
                effect = Effect.None;
            }

            return 10 + Level;
        }
        else if (effect == Effect.Polymorphic) {
            return 0;
        }
        
        return Level;
    }

    public void OnNotPlayed() {
        if (effect == Effect.Burning) {
            Level--;
        }
    }

    public bool HasNegativeEffect() {
        return effect switch {
            Effect.Poisonous | Effect.Doomed | Effect.Locked => true,
            _ => false
        };
    }

    public object Clone() {
        return this.MemberwiseClone();
    }

    public int CompareTo(object obj)
    {
        if (obj is Letter) {
            return Level.CompareTo((obj as Letter).Level); 
        }
        else return 0;
    }
}


public class GameWord 
{
    public string word;
    public int timesUsed;
}
