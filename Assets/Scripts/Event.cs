
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using EventSpawner = Util.Spawner<System.Func<Event.EventInfo>>;

public class Event : MonoBehaviour
{
    public EventInfo info;

    public TextMeshProUGUI nameText;

    private System.Action onCall;

    public void Init(bool isCurse, System.Action onDo) {
        info = isCurse ? GetRandomCurse() : GetRandomBlessing();
        nameText.text = info.name;
        this.onCall = onDo;
    }

    public static EventInfo GetRandomCurse() {
        return Util.GetRandomWithSpawners(new EventSpawner[] {
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Vowel decrementation",
                    description = "Decrement the level of every vowel.",
                    onCall = () => {
                        for (char a = 'a'; a <= 'z'; a++) {
                            if (Util.IsVowel(a)) {
                                GameManager.i.GetLetterFromChar(a).Level--;
                            }
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Consonant decrementation",
                    description = "Decrement the level of every consonant.",
                    onCall = () => {
                        for (char a = 'a'; a <= 'z'; a++) {
                            if (Util.IsConsonant(a)) {
                                GameManager.i.GetLetterFromChar(a).Level--;
                            }
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 1.5f,
                data = () => new EventInfo {
                    name = "Poisoning",
                    description = "Give the Poisonous effect to the most improved letter with no effect. Each time the letter scores, a random letter looses a level.",
                    onCall = () => {
                        int i = 1;
                        while (i <= 26)
                        {
                            Letter l = GameManager.i.GetNthMostImprovedLetter(i);
                            if (l.effect == Letter.Effect.None)
                            {
                                l.effect = Letter.Effect.Poisonous; 
                                break;
                            }

                            i--;
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Doom",
                    description = "Give the Doomed effect to the third most improved letter. Each time the letter scores, it looses a level.",
                    onCall = () => {
                        GameManager.i.GetNthMostImprovedLetter(3).effect = Letter.Effect.Doomed; 
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Shuffle",
                    description = "Level of the letters are shuffled (most improved letters will receive the lowest levels).",
                    onCall = () => {
                        for (int i = 0; i < 13; i++) {
                            Letter a = GameManager.i.GetNthMostImprovedLetter(i + 1);
                            Letter b = GameManager.i.GetNthMostImprovedLetter(26 - i);

                            int tmp = a.Level;
                            a.Level = b.Level;
                            b.Level = tmp;
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Tax",
                    description = "The most improved letter level will be multiplied by 0.7 (rounded down).",
                    onCall = () => {
                        Letter l = GameManager.i.GetNthMostImprovedLetter(1);
                        l.Level = Mathf.FloorToInt(0.7f * l.Level); 
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Average",
                    description = "Computes the average level of all letters (rounded down), then each letter will have this level.",
                    onCall = () => {
                        int sum = 0;
                        for (char a = 'a'; a <= 'z'; a++) {
                            sum += GameManager.i.GetLetterFromChar(a).Level;
                        }

                        int average = sum / 26;

                        for (char a = 'a'; a <= 'z'; a++) {
                            GameManager.i.GetLetterFromChar(a).Level = average;
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 0.7f,
                data = () => new EventInfo {
                    name = "Left destruction",
                    description = "Destroys the two leftmost bonuses.",
                    onCall = () => {
                        if (GameManager.i.bonuses.Count > 0) {
                            GameManager.i.RemoveBonus(GameManager.i.bonuses[0]);
                        }

                        if (GameManager.i.bonuses.Count > 0) {
                            GameManager.i.RemoveBonus(GameManager.i.bonuses[0]);
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 0.7f,
                data = () => new EventInfo {
                    name = "Right destruction",
                    description = "Destroys the two rightmost bonuses.",
                    onCall = () => {
                        if (GameManager.i.bonuses.Count > 0) {
                            GameManager.i.RemoveBonus(GameManager.i.bonuses[^1]);
                        }

                        if (GameManager.i.bonuses.Count > 0) {
                            GameManager.i.RemoveBonus(GameManager.i.bonuses[^1]);
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 1.5f,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'E', 'S', 'T', 'I', 'A', 'R' });
                    return new EventInfo {
                        name = $"Doomed {letter}",
                        description = $"Gives the Doomed effect to {Util.DecorateArgument(letter)}. Each time the letter scores, it looses a level",
                        onCall = () => {
                            GameManager.i.GetLetterFromChar(letter).effect = Letter.Effect.Doomed;
                        }
                    };
                }
            },
            new EventSpawner {
                weight = 1.5f,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'E', 'S', 'T', 'R' });
                    return new EventInfo {
                        name = $"Locked {letter}",
                        description = $"Gives the Locked effect to {Util.DecorateArgument(letter)}. The level of the letter won't change anymore.",
                        onCall = () => {
                            GameManager.i.GetLetterFromChar(letter).effect = Letter.Effect.Locked;
                        }
                    };
                }
            },
            new EventSpawner {
                weight = 0.8f,
                data = () => {
                    return new EventInfo {
                        name = $"Randomization",
                        description = $"Removes all bonuses, and give 3 random bonuses.",
                        onCall = () => {
                            while (GameManager.i.bonuses.Count > 0) {
                                GameManager.i.RemoveBonus(GameManager.i.bonuses[0]);
                            }

                            for (int i = 0; i < 3; i++) {
                                Bonus b = Instantiate(GameManager.i.bonusPrefab).GetComponent<Bonus>();
                                b.Init(Bonus.GetRandomBonus());
                                GameManager.i.AddBonus(b);
                            }
                        }
                    };
                }
            },
        })();
    }

    public static EventInfo GetRandomBlessing() {
        return Util.GetRandomWithSpawners(new EventSpawner[] {
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Doubled letter",
                    description = "Gives the Doubled effect on the third most improved letter. The letter will score twice as many points.",
                    onCall = () => {
                        GameManager.i.GetNthMostImprovedLetter(3).effect = Letter.Effect.Doubled; 
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Incrementation",
                    description = "Improves every letter by one level.",
                    onCall = () => {
                        for (char a = 'a'; a <= 'z'; a++) {
                            GameManager.i.GetLetterFromChar(a).Level++;
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Vowel incrementation",
                    description = "Improves every vowel by 5 levels.",
                    onCall = () => {
                        for (char a = 'a'; a <= 'z'; a++) {
                            if (Util.IsVowel(a))
                            {
                                GameManager.i.GetLetterFromChar(a).Level += 5;
                            }
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Multiplication",
                    description = "Multiplies the level of the most improved letter by 1.4 (rounded up)",
                    onCall = () => {
                        Letter l = GameManager.i.GetNthMostImprovedLetter(1);
                        l.Level = Mathf.CeilToInt(1.4f * l.Level);
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Cleaning",
                    description = "Removes every negative effect, and letters with level that is less than 3 will have level 3.",
                    onCall = () => {
                        for (char a = 'a'; a <= 'z'; a++) {
                            Letter l = GameManager.i.GetLetterFromChar(a);

                            if (l.HasNegativeEffect()) {
                                l.effect = Letter.Effect.None;
                            }

                            if (l.Level < 3) {
                                l.Level = 3;
                            }
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Burn",
                    description = "Gives the Burning effect to the fourth most improved letter. The letter will gain a level when used, but will loose 1 if not used in a word.",
                    onCall = () => {
                        GameManager.i.GetNthMostImprovedLetter(4).effect = Letter.Effect.Burning; 
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Left copy",
                    description = "Gives a copy of the leftmost bonus, if enough place.",
                    onCall = () => {
                        if (GameManager.i.bonuses.Count > 0) {
                            GameManager.i.CloneBonus(GameManager.i.bonuses[0]);
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Right copy",
                    description = "Gives a copy of the rightmost bonus, if enough place.",
                    onCall = () => {
                        if (GameManager.i.bonuses.Count > 0) {
                            GameManager.i.CloneBonus(GameManager.i.bonuses[^1]);
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Luck",
                    description = "Take two random blessing (including this blessing).",
                    onCall = () => {
                        GetRandomBlessing().onCall();
                        GetRandomBlessing().onCall();
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Time travel",
                    description = "Go back to the previous level with your current bonuses. (Random elements will be different)",
                    onCall = () => {
                        GameManager.i.gi.currentLevel--;
                    }
                }
            },
        })();
    }

    public void OnClick() {
        BonusPopup.i.ShowPopup(info.name, info.description, () => {
            info.onCall();
            Keyboard.i.UpdateAllKeys(true);
            BonusPopup.i.HidePopup();
            onCall();
        }, "Take");
    }

    public struct EventInfo {
        public string name;
        public string description;
        public bool isCurse;
        public System.Action onCall;
    }
}
