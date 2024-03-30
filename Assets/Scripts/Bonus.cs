using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Bonus : MonoBehaviour
{
    public BonusInfo info;

    [SerializeField] private TextMeshProUGUI nameText;

    public System.Action popupAction;
    public string popupActionText;

    private bool isOscillating = false; 

    public void Init()
    {
        info = GetRandomBonus();
        nameText.text = info.name;
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
        BonusSpawner[] spawns = {
            new BonusSpawner {
                weight = 1,
                get = () => {
                    string pair = Util.GetRandomElement(new string[] {
                        "ZZ", "EQ", "EK", "RW", "TF", "TM", "TB", "TN", "YG", "UF", "UK", "IX", "OZ", "PN", "DM", "FY", "GM", "HS", "HM", "HN", "KY", "KN", "LF", "LG", "LK", "LB", "WR", "WS", "WL", "WN", "XY", "XO", "XP", "XC", "BY", "NZ", "NJ", "NW", 
                    });

                    return new BonusInfo {
                        name = "The pair",
                        description = $"If the word contains {Util.DecorateArgument(pair)}, improves these letters by two levels",
                        onScore = (word) => {
                            if (word.Contains(pair))
                            {
                                return new BonusAction {
                                    isAffected = true,
                                    score = 0,
                                    lettersToImprove = new char[] { pair[0], pair[1], pair[0], pair[1] },
                                };
                            }
                            else
                            {
                                return new BonusAction {
                                    isAffected = false,
                                    score = 0,
                                    lettersToImprove = null,
                                };
                            }
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                get = () => {
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
                                        return new BonusAction {
                                            isAffected = true,
                                            score = 0,
                                            lettersToImprove = new char[] { c },
                                        };
                                    }

                                    i++;
                                }                            
                            }

                            return new BonusAction {
                                isAffected = false,
                                score = 0,
                                lettersToImprove = null,
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                get = () => {
                    return new BonusInfo {
                        name = "Best copy",
                        description = "Looks for the most improved letter in the word, then adds its level to the score",
                        onScore = (word) => {
                            int bestLevel = 0;

                            foreach (char c in word)
                            {
                                Letter l = GameManager.i.GetLetterFromChar(c);

                                if (l.level > bestLevel) bestLevel = l.level;
                            }

                            return new BonusAction {
                                isAffected = true,
                                score = bestLevel,
                                lettersToImprove = null,
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                get = () => {
                    return new BonusInfo {
                        name = "Worst copy",
                        description = "Looks for the least improved letter in the word, then adds twice its level to the score",
                        onScore = (word) => {
                            int worstLevel = 100000000;

                            foreach (char c in word)
                            {
                                Letter l = GameManager.i.GetLetterFromChar(c);

                                if (l.level < worstLevel) worstLevel = l.level;
                            }

                            return new BonusAction {
                                isAffected = true,
                                score = worstLevel,
                                lettersToImprove = null,
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                get = () => {
                    return new BonusInfo {
                        name = "Short word",
                        description = "If the word is 4 letters long, improves the 4 letters.",
                        onScore = (word) => {
                            if (word.Length == 4)
                            {
                                char[] lettersToImprove = new char[4];

                                for (int i = 0; i < 4; i++)
                                {
                                    lettersToImprove[i] = word[i];
                                }

                                return new BonusAction {
                                    isAffected = true,
                                    score = 0,
                                    lettersToImprove = lettersToImprove,
                                };
                            }
                            else
                            {
                                return new BonusAction {
                                    isAffected = false,
                                    score = 0,
                                    lettersToImprove = null,
                                };
                            }
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                get = () => {
                    return new BonusInfo {
                        name = "Rare letters",
                        description = $"Improves the level of every {Util.DecorateArgument('q')}, {Util.DecorateArgument('j')} and {Util.DecorateArgument('z')} in the word, then give 5 points if found any of them",
                        onScore = (word) => {
                            List<char> chars = new List<char>();

                            foreach (char c in word)
                            {
                                if (c == 'Q' || c == 'J' || c == 'Z')
                                {
                                    chars.Add(c);
                                }
                            }    
                        
                            return new BonusAction {
                                isAffected = chars.Count > 0,
                                score = chars.Count > 0 ? 5 : 0,
                                lettersToImprove = chars.ToArray(),
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                get = () => {
                    return new BonusInfo {
                        name = "Double",
                        description = $"If the word contains two identical adjacent letters, improve the level of the letter.",
                        onScore = (word) => {
                            List<char> chars = new List<char>();

                            char last = '\0';
                            foreach (char c in word)
                            {
                                if (last == c)
                                {
                                    chars.Add(c);
                                }

                                last = c;
                            }    
                        
                            return new BonusAction {
                                isAffected = chars.Count > 0,
                                score = 0,
                                lettersToImprove = chars.ToArray(),
                            };
                        }
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                get = () => {
                    return new BonusInfo {
                        name = "The initial",
                        description = $"Improves the first letter of the word",
                        onScore = (word) => {  
                            if (word.Length > 0) 
                            {
                                return new BonusAction {
                                    isAffected = true,
                                    score = 0,
                                    lettersToImprove = new char[1] { word[0] },
                                };
                            }
                            else
                            {
                                return new BonusAction {
                                    isAffected = true,
                                    score = 0,
                                    lettersToImprove = null,
                                };
                            }
                        }
                    };
                }
            }
        };

        float totalWeight = 0;
        foreach (BonusSpawner spawn in spawns)
        {   
            totalWeight += spawn.weight;
        }

        float randomSpawn = Random.Range(0.0f, totalWeight);
        totalWeight = 0;
        foreach (BonusSpawner spawn in spawns)
        {
            totalWeight += spawn.weight;
            if (randomSpawn <= totalWeight) // Instantiate bonus
            {
                return spawn.get();
            }
        }

        throw new System.Exception("Unreachable");
    }

    public void OnClick()
    {
        BonusPopup.i.ShowPopup(info, popupAction, popupActionText);
    }

    public BonusAction UpdateScoreInterface(string word, bool withInterface)
    {
        BonusAction res = info.onScore(word.ToUpper());

        isOscillating = res.isAffected && withInterface;

        return res;
    }
}

public class BonusInfo
{
    public string name;
    public string description;
    public System.Func<string, BonusAction> onScore; // TODO: make sure it's upper
}

public struct BonusAction
{
    public bool isAffected;
    public int score;
    public char[] lettersToImprove;
}

class BonusSpawner
{
    public System.Func<BonusInfo> get;
    public float weight; // Chance to appear
}
