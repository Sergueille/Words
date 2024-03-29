using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Bonus : MonoBehaviour
{
    public BonusInfo info;

    [SerializeField] private TextMeshProUGUI nameText;

    public void Init()
    {
        info = GetRandomBonus();
        nameText.text = info.name;
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
                        description = $"If the word contains {Util.DecorateArgument(pair)}, improves these letters",
                        onScore = (word) => {
                            if (word.Contains(pair))
                            {
                                return new BonusAction {
                                    isAffected = true,
                                    score = 0,
                                    lettersToImprove = new char[] { pair[0], pair[1] },
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
                        name = "Vowels",
                        description = "Improves a random vowel in the word",
                        onScore = (word) => {
                            int vowelCount = 0;
                            foreach (char c in word)
                            {
                                if (Util.IsVowel(c)) vowelCount++;
                            }

                            int randVowel = Random.Range(0, vowelCount);
                            int i = 0;
                            foreach (char c in word)
                            {
                                if (Util.IsVowel(c)) 
                                {
                                    if (i == randVowel) {
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
}

public struct BonusInfo
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
