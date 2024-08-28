
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using EventSpawner = Util.Spawner<System.Func<Event.EventInfo>>;

public class Event : MonoBehaviour
{
    public const int CURSE_COUNT = 12;
    public const int BLESSING_COUNT = 11;

    public EventInfo info;

    public TextMeshProUGUI nameText;

    private System.Action onCall;

    public bool hideTakeButtonOnPopup = false;
    public bool isUnknown = false;

    public void Init(EventInfo info, bool isCurse, System.Action onDo) {
        this.info = info;
        nameText.text = info.name;
        isUnknown = false;
        this.onCall = () => {
            if (isCurse) {
                GameManager.i.progression.usedCurses.Set(info.typeID, true);
            }
            else {
                GameManager.i.progression.usedBenedictions.Set(info.typeID, true);
            }
            onDo();
        };
    }

    public void InitUnknown()
    {
        isUnknown = true;
        nameText.text = "??";
    }

    public static EventInfo GetRandomCurse() {
        return Util.GetRandomWithSpawners(new EventSpawner[] {
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Vowel decrementation",
                    description = "Removes one point from every vowel.",
                    typeID = 0,
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
                    description = "Removes one point from every consonant.",
                    typeID = 1,
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
                    description = "Gives the Poisonous effect to the most improved letter with no effect. Each time the letter scores, a random letter loses a point.",
                    typeID = 2,
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

                            i++;
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Doom",
                    description = "Gives the Doomed effect to the third most improved letter. Each time the letter scores, it loses a point.",
                    typeID = 3,
                    onCall = () => {
                        GameManager.i.GetNthMostImprovedLetter(3).effect = Letter.Effect.Doomed; 
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Shuffle",
                    description = "The points of the letters are shuffled, causing the most improved letters to receive the lowest points.",
                    typeID = 4,
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
                weight = 1.5f,
                data = () => new EventInfo {
                    name = "Tax",
                    description = "The score of the letter with the highest score will be multiplied by 0.7 (rounded down).",
                    typeID = 5,
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
                    description = "Computes the average score of all letters (rounded down), then gives this score to every letter.",
                    typeID = 6,
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
                    typeID = 7,
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
                    typeID = 8,
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
                    char letter = Util.GetRandomElement(new char[] { 'E', 'S', 'T', 'I', 'A' });
                    return new EventInfo {
                        name = $"Doomed {letter}",
                        description = $"Gives the Doomed effect to {Util.DecorateArgument(letter)}. Each time the letter scores, it loses a point.",
                        typeID = 9,
                        onCall = () => {
                            GameManager.i.GetLetterFromChar(letter).effect = Letter.Effect.Doomed;
                        }
                    };
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'E', 'S', 'T' });
                    return new EventInfo {
                        name = $"Locked {letter}",
                        description = $"Gives the Locked effect to {Util.DecorateArgument(letter)}. The points of the letter will no longer change.",
                        typeID = 10,
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
                        description = $"Removes all bonuses, and gives 3 random bonuses.",
                        typeID = 11,
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
            new EventSpawner {
                weight = 1.0f,
                data = () => {
                    return new EventInfo {
                        name = $"Lock",
                        description = $"Gives the Locked effect to the second most improved letter. The points of the letter will no longer change.",
                        typeID = 12,
                        onCall = () => {
                            GameManager.i.GetNthMostImprovedLetter(2).effect = Letter.Effect.Locked;
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
                    description = "Gives the Doubled effect on the second most improved letter. The letter will score twice as many points.",
                    typeID = 0,
                    onCall = () => {
                        GameManager.i.GetNthMostImprovedLetter(2).effect = Letter.Effect.Doubled; 
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Incrementation",
                    description = "Adds 1 point to every letter.",
                    typeID = 1,
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
                    description = "Adds 5 points to every vowel.",
                    typeID = 2,
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
                    description = "Multiplies the score of the letter with the highest score by 1.4 (rounded up)",
                    typeID = 3,
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
                    description = "Removes every negative effect, and ensures that letters have a score of at least 3 points.",
                    typeID = 4,
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
                    description = "Gives the Burning effect to the second most improved letter. The letter will gain 1 point when used, but will lose 1 if not used in a word.",
                    typeID = 5,
                    onCall = () => {
                        GameManager.i.GetNthMostImprovedLetter(2).effect = Letter.Effect.Burning; 
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Left copy",
                    description = "Gives a copy of the leftmost bonus, if there is enough space.",
                    typeID = 6,
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
                    description = "Gives a copy of the rightmost bonus, if there is enough space.",
                    typeID = 7,
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
                    description = "Receive two random blessings.",
                    typeID = 8,
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
                    description = "Go back 2 levels earlier without losing your letters and bonuses, but random things will be different.",
                    typeID = 9,
                    onCall = () => {
                        GameManager.i.gi.currentLevel -= 2;
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Super bonus",
                    description = "Get a random super bonus.",
                    typeID = 10,
                    onCall = () => {
                        Bonus b = Instantiate(GameManager.i.bonusPrefab).GetComponent<Bonus>();
                        b.Init(Bonus.GetSuperBonus());
                        GameManager.i.AddBonus(b);
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Tripled letter",
                    description = "Gives the Tripled effect on the third most improved letter. The letter will score thrice as many points.",
                    typeID = 11,
                    onCall = () => {
                        GameManager.i.GetNthMostImprovedLetter(3).effect = Letter.Effect.Tripled; 
                    }
                }
            },
        })();
    }

    public void OnClick() {
        if (isUnknown) {
            BonusPopup.i.ShowPopup("???", "You haven't discovered this. Take it at least once to discover it.", () => {}, "");
        }
        else {
            BonusPopup.i.ShowPopup(info.name, info.description, () => {
                Util.PingText(nameText);
                info.onCall();
                Keyboard.i.UpdateAllKeys(true);
                BonusPopup.i.HidePopup();
                onCall();
            }, hideTakeButtonOnPopup ? "" : "Take");
        }
    }

    public struct EventInfo {
        public string name;
        public string description;
        public bool isCurse;
        public System.Action onCall;
        public int typeID;
    }
}
