using System.Collections.Generic;
using UnityEngine;
using TMPro;

using BonusSpawner = Util.Spawner<System.Func<BonusInfo>>;
using System.Runtime.InteropServices;

public class Bonus : MonoBehaviour, System.ICloneable
{
    public BonusInfo info;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Transform scoreTextUp;
    [SerializeField] private Transform scoreTextDown;

    public System.Action popupAction;
    public string popupActionText;

    private bool isOscillating = false; 

    public void Init()
    {
        info = GetRandomBonus();
        nameText.text = info.name;
        scoreText.transform.position = scoreTextDown.position;
    }

    private void Update()
    {
        if (isOscillating)
        {
            transform.eulerAngles = new Vector3(0, 0, Mathf.Sin(Time.time * 15.0f) * 4);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    public static BonusInfo GetRandomBonus() 
    {
        System.Func<BonusInfo> bonusGetter = Util.GetRandomWithSpawners(new BonusSpawner[] {
            new BonusSpawner {
                weight = 2.5f,
                data = () => {
                    string pair = Util.GetRandomElement(new string[] {
                        "AR", "AT", "AP", "AS", "AL", "AM", "AC", "AB", "AN", "EA", "ET", "ES", "ED", "EL", "EM", "EC", "EN", "RA", "RE", 
                        "RI", "RO", "RS", "TA", "TE", "TR", "TI", "TO", "TH", "UR", "US", "UL", "UN", "IA", "IE", "IT", "IO", "IS", "ID", 
                        "IL", "IC", "OR", "OT", "OU", "OP", "OS", "OG", "OL", "OM", "OC", "ON", "PA", "PE", "PR", "PI", "PO", "PH", "SE", 
                        "ST", "SU", "SI", "SS", "SH", "DE", "DI", "GE", "HA", "HE", "HI", "HO", "LA", "LE", "LY", "LI", "LO", "LL", "MA", 
                        "ME", "MI", "MO", "CA", "CE", "CT", "CI", "CO", "CH", "VE", "BL", "NA", "NE", "NT", "NI", "NO", "NS", "ND", "NG", 
                        "NC", 
                    });

                    return new BonusInfo {
                        name = pair,
                        description = $"If the word contains {Util.DecorateArgument(pair)}, improves these letters by one level",
                        onScore = (word) => {
                            if (word.Contains(pair))
                            {
                                GameManager.i.ImproveLetter(pair[0]);
                                GameManager.i.ImproveLetter(pair[1]);

                                return new BonusAction {
                                    isAffected = true,
                                    score = 0,
                                };
                            }
                            else
                            {
                                return new BonusAction {
                                    isAffected = false,
                                    score = 0,
                                };
                            }
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 0.8f,
                data = () => {
                    return new BonusInfo {
                        name = "Consonants",
                        description = "Improves a random consonant in the word",
                        onScore = (word) => {
                            int consonantCount = 0;
                            foreach (char c in word)
                            {
                                if (!Util.IsVowel(c)) consonantCount++;
                            }

                            int randCons = Random.Range(0, consonantCount);
                            int i = 0;
                            foreach (char c in word)
                            {
                                if (!Util.IsVowel(c)) 
                                {
                                    if (i == randCons) {
                                        GameManager.i.ImproveLetter(c);
                                        return new BonusAction {
                                            isAffected = true,
                                            score = 0,
                                        };
                                    }

                                    i++;
                                }                            
                            }

                            return new BonusAction {
                                isAffected = false,
                                score = 0,
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    return new BonusInfo {
                        name = "Best copy",
                        description = "Looks for the most improved letter in the word, then adds its level to the score",
                        onScore = (word) => {
                            int bestLevel = 0;

                            foreach (char c in word)
                            {
                                Letter l = GameManager.i.GetLetterFromChar(c);

                                if (l.Level > bestLevel) bestLevel = l.Level;
                            }

                            return new BonusAction {
                                isAffected = true,
                                score = bestLevel,
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    return new BonusInfo {
                        name = "Worst copy",
                        description = "Looks for the least improved letter in the word, then adds 3 times its level to the score",
                        onScore = (word) => {
                            int worstLevel = 100000000;

                            foreach (char c in word)
                            {
                                Letter l = GameManager.i.GetLetterFromChar(c);

                                if (l.Level < worstLevel) worstLevel = l.Level;
                            }

                            return new BonusAction {
                                isAffected = true,
                                score = 3 * worstLevel,
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 0.8f,
                data = () => {
                    return new BonusInfo {
                        name = "Short word",
                        description = "If the word is 4 letters long, improves the 4 letters.",
                        onScore = (word) => {
                            if (word.Length == 4)
                            {
                                char[] lettersToImprove = new char[4];

                                for (int i = 0; i < 4; i++)
                                {
                                    GameManager.i.ImproveLetter(word[i]);
                                }

                                return new BonusAction {
                                    isAffected = true,
                                    score = 0,
                                };
                            }
                            else
                            {
                                return new BonusAction {
                                    isAffected = false,
                                    score = 0,
                                };
                            }
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 0.8f,
                data = () => {
                    return new BonusInfo {
                        name = "Rare letters",
                        description = $"Improves twice the level of every {Util.DecorateArgument('Q')}, {Util.DecorateArgument('J')}, {Util.DecorateArgument('X')} and {Util.DecorateArgument('Z')} in the word, and give 10 points if one is present",
                        onScore = (word) => {
                            bool improved = false;

                            foreach (char c in word)
                            {
                                if (GameManager.i.AreCharsEqual(c, 'Q')
                                 || GameManager.i.AreCharsEqual(c, 'J')
                                 || GameManager.i.AreCharsEqual(c, 'Z')
                                 || GameManager.i.AreCharsEqual(c, 'X'))
                                {
                                    improved = true;
                                    GameManager.i.ImproveLetter(c);
                                    GameManager.i.ImproveLetter(c);
                                }
                            }    
                        
                            return new BonusAction {
                                isAffected = improved,
                                score = improved ? 10 : 0,
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 0.8f,
                data = () => {
                    return new BonusInfo {
                        name = "Double",
                        description = $"If the word contains two identical adjacent letters, improve the level of the letter.",
                        onScore = (word) => {
                            bool improved = false;

                            char last = '\0';
                            foreach (char c in word)
                            {
                                if (GameManager.i.AreCharsEqual(last, c))
                                {
                                    improved = true;
                                    GameManager.i.ImproveLetter(c);
                                }

                                last = c;
                            }    
                        
                            return new BonusAction {
                                isAffected = improved,
                                score = 0,
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    return new BonusInfo {
                        name = "The initial",
                        description = $"Improves the first letter of the word",
                        onScore = (word) => {  
                            if (word.Length > 0) 
                            {
                                GameManager.i.ImproveLetter(word[0]);
                                return new BonusAction {
                                    isAffected = true,
                                    score = 0,
                                };
                            }
                            else
                            {
                                return new BonusAction {
                                    isAffected = false,
                                    score = 0,
                                };
                            }
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 0.7f,
                data = () => {
                    return new BonusInfo {
                        name = "More vowels",
                        description = "If the word has strictly more vowels than consonants, improves every letter of the word",
                        onScore = (word) => {  
                            int vowelCount = Util.CountVowels(word);
                            int consonantCount = Util.CountConsonant(word);

                            if (vowelCount > consonantCount)
                            {
                                foreach (char c in word) {
                                    GameManager.i.ImproveLetter(c);
                                }

                                return new BonusAction {
                                    isAffected = true,
                                    score = 0
                                };
                            }
                            else
                            {
                                return new BonusAction {
                                    isAffected = false,
                                    score = 0,
                                };
                            }
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    int rand = Random.Range(7, 10);

                    return new BonusInfo {
                        name = $"{rand} letters",
                        description = $"If the word is exactly {rand} letters long, gives half the points of the word (not counting other bonuses).",
                        onScore = (word) => {  
                            if (word.Length == rand)
                            {
                                int score = 0;
                                foreach (char c in word)
                                {
                                    score += GameManager.i.GetLetterFromChar(c).GetScore(false);
                                }

                                score /= 2;

                                return new BonusAction {
                                    isAffected = true,
                                    score = score,
                                };
                            }
                            else
                            {
                                return new BonusAction {
                                    isAffected = false,
                                    score = 0,
                                };
                            }
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    return new BonusInfo {
                        name = $"Triple",
                        description = $"If the word contains three identical letters, improve the level of the letter.",
                        onScore = (word) => { 
                            int[] counts = new int[26];
                            bool improved = false;

                            foreach (char c in word)
                            {
                                for (int i = 0; i < 26; i++)
                                {
                                    if (GameManager.i.AreCharsEqual((char)(i + 'A'), c)) {
                                        counts[i]++;
                                    }
                                }
                            }

                            for (int i = 0; i < 26; i++)
                            {
                                if (counts[i] >= 3)
                                {
                                    improved = true;
                                    GameManager.i.ImproveLetter((char)(i + 'A'));
                                }
                            }

                            return new BonusAction {
                                isAffected = improved,
                                score = 0,
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 0.9f,
                data = () => {
                    return new BonusInfo {
                        name = $"Frequent vowel",
                        description = $"Looks for the vowel that appeared the most in the word, then adds 6 points per time it appeared",
                        onScore = (word) => {  
                            int[] counts = new int[26];

                            foreach (char c in word)
                            {
                                counts[c - 'A']++;
                            }

                            int best = 0;
                            for (int i = 0; i < 26; i++)
                            {
                                if (Util.IsVowel((char)(i + 'A')) && best < counts[i])
                                {
                                    best = counts[i];
                                }
                            }

                            return new BonusAction {
                                isAffected = best > 0,
                                score = 6 * best,
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'V', 'B', 'K' });

                    return new BonusInfo {
                        name = $"Emergency {letter}",
                        description = $"If the word contains {Util.DecorateArgument(letter)} and the level of the letter is even, improves its level by 3 and gives 30 points.",
                        onScore = (word) => {  
                            Letter l = GameManager.i.GetLetterFromChar(letter);

                            foreach (char c in word)
                            {   
                                if (GameManager.i.AreCharsEqual(c, letter))
                                {
                                    if (l.Level % 2 == 0)
                                    {
                                        GameManager.i.ImproveLetter(letter);
                                        GameManager.i.ImproveLetter(letter);
                                        GameManager.i.ImproveLetter(letter);

                                        return new BonusAction {
                                            isAffected = true,
                                            score = 30,
                                        };
                                    }
                                }
                            }

                            return new BonusAction {
                                isAffected = false,
                                score = 0,
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        name = "Equalizer",
                        description = "Improves the least improved letter in the word (will improve the leftmost if multiple letters)",
                        onScore = (word) => {
                            int least = 100000000;
                            int leastId = 0;
                            for (int i = 0; i < word.Length; i++)
                            {
                                Letter letter = GameManager.i.GetLetterFromChar(word[i]);
                                if (letter.Level < least)
                                {
                                    least = letter.Level;
                                    leastId = i;
                                }
                            }

                            GameManager.i.ImproveLetter(word[leastId]);

                            return new BonusAction {
                                isAffected = true,
                                score = 0,
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        name = "Palindrome",
                        description = "If the first letter and the last letter of the word are identical, improves this letter and gives 15 points",
                        onScore = (word) => {
                            if (GameManager.i.AreCharsEqual(word[0], word[word.Length - 1]))
                            {
                                GameManager.i.ImproveLetter(word[0]);

                                return new BonusAction {
                                    isAffected = true,
                                    score = 15,
                                };
                            }
                            else
                            {
                                return new BonusAction {
                                    isAffected = false,
                                    score = 0,
                                };
                            }
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        name = "oOo",
                        description = $"Improves the letters located directly after an {Util.DecorateArgument("O")}",
                        onScore = (word) => {
                            bool improved = false;
                            bool lastWasO = false;
                            foreach (char c in word) {
                                if (lastWasO) {
                                    improved = true;                                    
                                    GameManager.i.ImproveLetter(c);
                                }

                                lastWasO = GameManager.i.AreCharsEqual(c, 'O');
                            }

                            return new BonusAction {
                                isAffected = improved,
                                score = 0,
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'N', 'R', 'I' });

                    return new BonusInfo {
                        name = $"{letter} everywhere",
                        description = $"Improves every {Util.DecorateArgument(letter)} in the word. If the word doesn't contain any, -500 points.",
                        onScore = (word) => {
                            bool improved = false;
                            
                            foreach (char c in word) {
                                if (GameManager.i.AreCharsEqual(c, letter)) {
                                    improved = true;
                                    GameManager.i.ImproveLetter(c);
                                }
                            }

                            return new BonusAction {
                                isAffected = true,
                                score = improved ? 0 : -500,
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'N', 'R', 'I' });

                    return new BonusInfo {
                        name = $"Forbidden {letter}",
                        description = $"Improves {Util.DecorateArgument(letter)} by two levels. If the word contain this letter, -500 points.",
                        onScore = (word) => {
                            bool found = false;
                            foreach (char c in word) {
                                found |= c == letter;
                            }

                            GameManager.i.ImproveLetter(letter);
                            GameManager.i.ImproveLetter(letter);

                            return new BonusAction {
                                isAffected = true,
                                score = found ? -500 : 00,
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        name = "Concentrate",
                        description = $"Lower the level of the least improved letter in the word, and improves the level of the most improved letter in word. (Will improve the leftmost letter if many)",
                        onScore = (word) => {
                            char highest = '\0';
                            int highestLevel = -9999999;
                            char lowest = '\0';
                            int lowestLevel = 99999999;

                            foreach (char c in word) {
                                Letter l = GameManager.i.GetLetterFromChar(c);

                                if (l.Level > highestLevel) {
                                    highestLevel = l.Level;
                                    highest = c;
                                }

                                if (l.Level < lowestLevel) {
                                    lowestLevel = l.Level;
                                    lowest = c;
                                }
                            }

                            GameManager.i.ImproveLetter(highest);
                            GameManager.i.GetLetterFromChar(lowest).Level -= 1;

                            return new BonusAction {
                                isAffected = true,
                                score = 0,
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 0.7f,
                data = () => {
                    return new BonusInfo {
                        name = "Relative numbers",
                        description = $"Improves letters which level is strictly less than 1. Gives 25 points per letter improved.",
                        onScore = (word) => {
                            int improvementCount = 0;

                            foreach (char c in word) {
                                Letter l = GameManager.i.GetLetterFromChar(c);

                                if (l.Level <= 0) {
                                    improvementCount++;
                                    l.Level++;
                                }
                            }

                            return new BonusAction {
                                isAffected = improvementCount > 0,
                                score = improvementCount * 25,
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'P', 'G', 'Y' });

                    return new BonusInfo {
                        name = $"Charged {letter}",
                        description = $"Gives the Electric effect to letters located after every {Util.DecorateArgument(letter)}. The next time they are played, Electric letters will loose their effect and give 10 additional points.",
                        onScore = (word) => {
                            int improvementCount = 0;
                            bool next = false;
                            Letter baseLetter = GameManager.i.GetLetterFromChar(letter);

                            foreach (char c in word) {
                                Letter l = GameManager.i.GetLetterFromChar(c);

                                if (next) {
                                    l.effect = Letter.Effect.Electric;
                                    improvementCount++;
                                    next = false;
                                }

                                if (l == baseLetter) {
                                    next = true;
                                }
                            }

                            return new BonusAction {
                                isAffected = improvementCount > 0,
                                score = 0,
                            };
                        }
                    };
                }
            }
        });

        return bonusGetter();
    }

    public void OnClick()
    {
        BonusPopup.i.ShowPopup(info.name, info.description, popupAction, popupActionText);
    }

    public BonusAction ScoreWithInterface(string word, bool withInterface)
    {
        BonusAction res;
        if (word.Length == 0) // Do not evaluate function on empty word (it may cause errors)
        {
            res = new BonusAction {
                isAffected = false, score = 0,
            };
        }
        else
        {
            res = info.onScore(word.ToUpper());
        }

        isOscillating = res.isAffected && withInterface;

        if (withInterface && res.isAffected && res.score > 0)
        {
            scoreText.text = $"+{res.score}";

            LeanTween.move(scoreText.gameObject, scoreTextUp.position, 0.4f).setEaseOutElastic();
        }
        else
        {   
            if (scoreText.transform.position != scoreTextDown.position)
            {
                LeanTween.move(scoreText.gameObject, scoreTextDown.position, 0.4f).setEaseInQuad();
            }
        }

        return res;
    }

    public void ResetInterface() 
    {
        isOscillating = false;
        if (scoreText.transform.position != scoreTextDown.position)
        {
            LeanTween.move(scoreText.gameObject, scoreTextDown.position, 0.4f).setEaseInQuad();
        }
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}

[StructLayout(LayoutKind.Sequential)]
public class BonusInfo
{
    public string name;
    public string description;
    public System.Func<string, BonusAction> onScore; // TODO: make sure it's upper
}

[StructLayout(LayoutKind.Sequential)]
public struct BonusAction
{
    public bool isAffected;
    public int score;
}
