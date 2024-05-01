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

    public static int GetLetterScore(Letter l) 
    {
        return l.level;
    }
}


public class Letter : System.ICloneable, System.IComparable
{
    public enum Effect {
        None, 
        Poisonous, Doomed,
        Doubled, Polymorphic,
    }

    public char letter;
    public int level;
    public int timesUsed = 0;
    public Effect effect;

    /// <summary>
    /// Get the score the letter will give.
    /// </summary>
    /// <param name="played">Is the letter played right now? If yes, wWill apply on play effects.</param>
    public int GetScore(bool played) {
        if (effect == Effect.Doubled) {
            return 2 * level;
        }
        else if (effect == Effect.Poisonous) {
            Debug.Log("TEEEST");
            if (played) {
                Util.GetRandomElement(GameManager.i.letters).level--;
            }
        }
        else if (effect == Effect.Doomed) {
            Debug.Log("TEEEST");
            if (played) {
                level--;
            }
        }
        else if (effect == Effect.Polymorphic) {
            Debug.Log("TEEEST");
            return 0;
        }
        
        return level;
    }

    public bool IsChar(char a) {
        return (letter == a) || effect == Effect.Polymorphic; // FIXME: make this reflexive (?)
    }

    public bool HasNegativeEffect() {
        return effect switch {
            Effect.Poisonous | Effect.Doomed => true,
            _ => false
        };
    }

    public object Clone() {
        return this.MemberwiseClone();
    }

    public int CompareTo(object obj)
    {
        if (obj is Letter) {
            return level.CompareTo((obj as Letter).level); 
        }
        else return 0;
    }
}


public struct GameWord 
{
    public string word;
}
