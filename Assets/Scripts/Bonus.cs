using UnityEngine;
using TMPro;

using BonusSpawner = Util.Spawner<System.Func<BonusInfo>>;
using System.Runtime.InteropServices;
using System.Collections.Generic;

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

    public void Init(BonusInfo info)
    {
        this.info = info;
        nameText.text = GetName();
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
                        type = BonusType.TwoLetters,
                        stringArg = pair,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.8f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Consonants,
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.BestCopy
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.WorstCopy,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.8f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.ShortWord,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.8f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.RareLetters,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.8f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Double,
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Initial,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.7f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.MoreVowels,
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    int rand = Random.Range(7, 10);

                    return new BonusInfo {
                        type = BonusType.Length,
                        intArg = rand,
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Triple,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.9f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.FrequentVowel,
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'V', 'B', 'K' });

                    return new BonusInfo {
                        type = BonusType.Emergency,
                        stringArg = letter.ToString(),
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Equalizer,
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Palindrome,
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.OOO,
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'N', 'R', 'I', 'H' });

                    return new BonusInfo {
                        type = BonusType.Everywhere,
                        stringArg = letter.ToString(),
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'N', 'R', 'I', 'H' });

                    return new BonusInfo {
                        type = BonusType.Forbidden,
                        stringArg = letter.ToString(),
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Concentrate,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.7f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.RelativeNumbers,
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'P', 'G', 'Y' });

                    return new BonusInfo {
                        type = BonusType.Charged,
                        stringArg = letter.ToString(),
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Constant,
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Diversity,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.7f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Effects,
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.LongWord,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.7f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Sacrifice,
                    };
                }
            },
        });

        return bonusGetter();
    }

    public BonusAction OnScore(string word)
    {
        if (info.type == BonusType.TwoLetters)
        {
            if (word.Contains(info.stringArg))
            {
                GameManager.i.ImproveLetter(info.stringArg[0]);
                GameManager.i.ImproveLetter(info.stringArg[1]);

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
        else if (info.type == BonusType.Charged)
        {
            int improvementCount = 0;
            bool next = false;
            Letter baseLetter = GameManager.i.GetLetterFromChar(info.stringArg[0]);

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
        else if (info.type == BonusType.RelativeNumbers)
        {
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
        else if (info.type == BonusType.Concentrate)
        {
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
            GameManager.i.ImproveLetter(highest);
            GameManager.i.GetLetterFromChar(lowest).Level -= 1;

            return new BonusAction {
                isAffected = true,
                score = 0,
            };
        }
        else if (info.type == BonusType.Forbidden)
        {
            bool found = false;
            foreach (char c in word) {
                found |= c == info.stringArg[0];
            }

            GameManager.i.ImproveLetter(info.stringArg[0]);
            GameManager.i.ImproveLetter(info.stringArg[0]);

            return new BonusAction {
                isAffected = true,
                score = found ? -500 : 00,
            };
        }
        else if (info.type == BonusType.Everywhere)
        {
            bool improved = false;
            
            foreach (char c in word) {
                if (GameManager.i.AreCharsEqual(c, info.stringArg[0])) {
                    improved = true;
                    GameManager.i.ImproveLetter(c);
                }
            }

            return new BonusAction {
                isAffected = true,
                score = improved ? 0 : -500,
            };
        }
        else if (info.type == BonusType.OOO)
        {
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
        else if (info.type == BonusType.Palindrome)
        {
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
        else if (info.type == BonusType.Equalizer)
        {
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
        else if (info.type == BonusType.Emergency)
        {
            Letter l = GameManager.i.GetLetterFromChar(info.stringArg[0]);

            foreach (char c in word)
            {   
                if (GameManager.i.AreCharsEqual(c, info.stringArg[0]))
                {
                    if (l.Level % 2 == 0)
                    {
                        GameManager.i.ImproveLetter(info.stringArg[0]);
                        GameManager.i.ImproveLetter(info.stringArg[0]);
                        GameManager.i.ImproveLetter(info.stringArg[0]);

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
        else if (info.type == BonusType.FrequentVowel)
        {
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
        else if (info.type == BonusType.Triple)
        {
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
        else if (info.type == BonusType.Length)
        {
            if (word.Length == info.intArg)
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
        else if (info.type == BonusType.MoreVowels)
        {
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
        else if (info.type == BonusType.Initial)
        {
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
        else if (info.type == BonusType.Double)
        {
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
        else if (info.type == BonusType.ShortWord)
        {
            if (word.Length == 4)
            {
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
        else if (info.type == BonusType.RareLetters)
        {
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
        else if (info.type == BonusType.WorstCopy)
        {
            int worstLevel = 100000000;
            Letter worstLetter = null;

            foreach (char c in word)
            {
                Letter l = GameManager.i.GetLetterFromChar(c);

                if (l.Level < worstLevel) 
                {
                    worstLetter = l;
                    worstLevel = l.Level;
                }
            }

            return new BonusAction {
                isAffected = true,
                score = 6 * worstLetter.GetScore(false),
            };
        }
        else if (info.type == BonusType.BestCopy)
        {
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
        else if (info.type == BonusType.Consonants)
        {
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
        else if (info.type == BonusType.Constant)
        {
            int firstLevel = GameManager.i.GetLetterFromChar(word[0]).Level;
            bool ok = true;

            for (int i = 1; i < word.Length; i++)
            {
                if (firstLevel != GameManager.i.GetLetterFromChar(word[i]).Level)
                {
                    ok = false;
                    break;
                }
            }

            if (ok)
            {
                int score = 0;
                foreach (char c in word)
                {
                    score += GameManager.i.GetLetterFromChar(c).GetScore(false);
                }

                for (int i = 0; i < 5; i++)
                {
                    if (word.Length >= i + 1)
                        GameManager.i.ImproveLetter(word[i]);
                }

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
        else if (info.type == BonusType.Diversity)
        {
            bool[] used = new bool[26];

            foreach (char c in word)
            {
                if (used[c - 'A']) 
                {
                    return new BonusAction {
                        isAffected = false,
                        score = 0,
                    };
                }
                else
                {
                    used[c - 'A'] = true;
                }
            }

            foreach (char c in word)
            {
                if (Util.IsVowel(c))
                {
                    GameManager.i.ImproveLetter(c);
                }
            }

            return new BonusAction {
                isAffected = true,
                score = 0,
            };
        }
        else if (info.type == BonusType.Effects)
        {
            bool affected = false;

            foreach (char c in word)
            {
                if (GameManager.i.GetLetterFromChar(c).effect == Letter.Effect.None)
                {
                    GameManager.i.ImproveLetter(c);
                    affected = true;
                }
            }

            return new BonusAction {
                isAffected = affected,
                score = 0,
            };
        }
        else if (info.type == BonusType.LongWord)
        {
            return new BonusAction {
                isAffected = word.Length > 12,
                score = 0,
            };
        }
        
        else if (info.type == BonusType.Sacrifice)
        {
            GameManager.i.GetLetterFromChar(word[0]).effect = Letter.Effect.Doomed;

            return new BonusAction {
                isAffected = true,
                score = 40,
            };
        }
        else
        {
            throw new System.Exception("Missing branch!");
        }
    }

    public string GetName()
    {
        if (info.type == BonusType.TwoLetters)
        {
            return info.stringArg;
        }
        else if (info.type == BonusType.Charged)
        {
            return $"Charged {info.stringArg}";
        }
        else if (info.type == BonusType.RelativeNumbers)
        {
            return "Relative numbers";
        }
        else if (info.type == BonusType.Concentrate)
        {
            return "Concentrate";
        }
        else if (info.type == BonusType.Forbidden)
        {
            return $"Forbidden {info.stringArg}";
        }
        else if (info.type == BonusType.Everywhere)
        {
            return $"{info.stringArg} everywhere";
        }
        else if (info.type == BonusType.OOO)
        {
            return "oOo";
        }
        else if (info.type == BonusType.Palindrome)
        {
            return "Palindrome";
        }
        else if (info.type == BonusType.Equalizer)
        {
            return "Equalizer";
        }
        else if (info.type == BonusType.Emergency)
        {
            return $"Emergency {info.stringArg}";
        }
        else if (info.type == BonusType.FrequentVowel)
        {
            return "Frequent vowel";
        }
        else if (info.type == BonusType.Triple)
        {
            return "Triple";
        }
        else if (info.type == BonusType.Length)
        {
            return $"{info.intArg} letters";
        }
        else if (info.type == BonusType.MoreVowels)
        {
            return "More vowels";
        }
        else if (info.type == BonusType.Initial)
        {
            return "The initial";
        }
        else if (info.type == BonusType.Double)
        {
            return "Double";
        }
        else if (info.type == BonusType.ShortWord)
        {
            return "Short word";
        }
        else if (info.type == BonusType.RareLetters)
        {
            return "Rare letters";
        }
        else if (info.type == BonusType.WorstCopy)
        {
            return "Worst copy";
        }
        else if (info.type == BonusType.BestCopy)
        {
            return "Best copy";
        }
        else if (info.type == BonusType.Consonants)
        {
            return "Consonants";
        }
        else if (info.type == BonusType.Constant)
        {
            return "Constant";
        }
        else if (info.type == BonusType.Diversity)
        {
            return "Diversity";
        }
        else if (info.type == BonusType.Effects)
        {
            return "Effects";
        }
        else if (info.type == BonusType.LongWord)
        {
            return "Long words";
        }
        else if (info.type == BonusType.Sacrifice)
        {
            return "Sacrifice";
        }
        else
        {
            throw new System.Exception("Missing branch!");
        }
    }

    public string GetDescription()
    {
        if (info.type == BonusType.TwoLetters)
        {
            return $"If the word contains {Util.DecorateArgument(info.stringArg)}, improves these letters by one level.";
        }
        else if (info.type == BonusType.Charged)
        {
            return $"Gives the Electric effect to letters located after every {Util.DecorateArgument(info.stringArg)}. The next time they are played, Electric letters will loose their effect and give 10 additional points.";
        }
        else if (info.type == BonusType.RelativeNumbers)
        {
            return "Improves letters which level is strictly less than 1. Gives 25 points per letter improved.";
        }
        else if (info.type == BonusType.Concentrate)
        {
            return "Lower the level of the least improved letter in the word, and improves twice the level of the most improved letter in word. (Will choose the leftmost letters first)";
        }
        else if (info.type == BonusType.Forbidden)
        {
            return $"Improves {Util.DecorateArgument(info.stringArg)} by two levels. If the word contain this letter, -500 points.";
        }
        else if (info.type == BonusType.Everywhere)
        {
            return $"Improves every {Util.DecorateArgument(info.stringArg)} in the word. If the word doesn't contain any, -500 points.";
        }
        else if (info.type == BonusType.OOO)
        {
            return $"Improves the letters located directly after an {Util.DecorateArgument("O")}.";
        }
        else if (info.type == BonusType.Palindrome)
        {
            return "If the first letter and the last letter of the word are identical, improves this letter and gives 15 points.";
        }
        else if (info.type == BonusType.Equalizer)
        {
            return "Improves the least improved letter in the word (will improve the leftmost if multiple letters).";
        }
        else if (info.type == BonusType.Emergency)
        {
            return $"If the word contains {Util.DecorateArgument(info.stringArg)} and the level of the letter is even, improves its level by 3 and gives 30 points.";
        }
        else if (info.type == BonusType.FrequentVowel)
        {
            return $"Looks for the vowel that appeared the most in the word, then adds 6 points per time it appeared.";
        }
        else if (info.type == BonusType.Triple)
        {
            return $"If the word contains three identical letters, improve the level of the letter.";
        }
        else if (info.type == BonusType.Length)
        {
            return $"If the word is exactly {info.intArg} letters long, gives half the points of the word (not counting other bonuses).";
        }
        else if (info.type == BonusType.MoreVowels)
        {
            return "If the word has strictly more vowels than consonants, improves every letter of the word.";
        }
        else if (info.type == BonusType.Initial)
        {
            return "Improves the first letter of the word.";
        }
        else if (info.type == BonusType.Double)
        {
            return "If the word contains two identical adjacent letters, improve the level of the letter.";
        }
        else if (info.type == BonusType.ShortWord)
        {
            return "If the word is 4 letters long, improves the 4 letters.";
        }
        else if (info.type == BonusType.RareLetters)
        {
            return $"Improves twice the level of every {Util.DecorateArgument('Q')}, {Util.DecorateArgument('J')}, {Util.DecorateArgument('X')} and {Util.DecorateArgument('Z')} in the word, and give 10 points if one is present";
        }
        else if (info.type == BonusType.WorstCopy)
        {
            return "Looks for the least improved letter in the word, then adds 6 times the points it would give to the score.";
        }
        else if (info.type == BonusType.BestCopy)
        {
            return "Looks for the most improved letter in the word, then adds its level to the score.";
        }
        else if (info.type == BonusType.Consonants)
        {
            return $"Improves a random consonant in the word.";
        }
        else if (info.type == BonusType.Constant)
        {
            return $"If all the letters of the word have the same level, improves the first 5 letters and give as many points the word would give alone.";
        }
        else if (info.type == BonusType.Diversity)
        {
            return "If all the letters of the word are different, improves all the vowels of the word.";
        }
        else if (info.type == BonusType.Effects)
        {
            return "Improves all letters with an effect in the word.";
        }
        else if (info.type == BonusType.LongWord)
        {
            return "Allows you to write words up to 16 letters long.";
        }
        else if (info.type == BonusType.Sacrifice)
        {
            return "Gives 40 points, but the first letter or the word becomes Doomed. Doomed letters will loose one level every time they score.";
        }
        else
        {
            throw new System.Exception("Missing branch!");
        }
    }

    public void OnClick()
    {
        BonusPopup.i.ShowPopup(GetName(), GetDescription(), popupAction, popupActionText);
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
            res = OnScore(word.ToUpper());
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

[System.Serializable]
public struct BonusInfo : System.ICloneable
{
    public BonusType type;
    public string stringArg;
    public int intArg;

    public override bool Equals(object obj)
    {
        if (obj is BonusInfo other)
        {
            return other.type == this.type
                && other.stringArg == this.stringArg
                && other.intArg == this.intArg;
        }
        else return false;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}

[System.Serializable]
public enum BonusType {
    TwoLetters,
    Consonants,
    BestCopy,
    WorstCopy,
    RareLetters,
    ShortWord,
    Double,
    Initial,
    MoreVowels,
    Length,
    Triple,
    FrequentVowel,
    Emergency,
    Equalizer,
    Palindrome,
    OOO,
    Everywhere,
    Forbidden,
    Concentrate,
    RelativeNumbers,
    Charged,
    Constant,
    Diversity,
    Effects,
    LongWord,
    Sacrifice,
}


[StructLayout(LayoutKind.Sequential)]
public struct BonusAction
{
    public bool isAffected;
    public int score;
}
