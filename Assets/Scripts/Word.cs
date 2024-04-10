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


public class Letter 
{
    public char letter;
    public int level;
    public int timesUsed = 0;
}


public struct GameWord 
{
    public string word;
}
