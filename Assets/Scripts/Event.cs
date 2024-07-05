
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
                    description = "Decrement the level of every vowel.",
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
                    description = "Decrement the level of every consonant.",
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
                    description = "Give the Poisonous effect to the most improved letter with no effect. Each time the letter scores, a random letter looses a level.",
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
                    description = "Level of the letters are shuffled (most improved letters will receive the lowest levels).",
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
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Tax",
                    description = "The most improved letter level will be multiplied by 0.7 (rounded down).",
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
                    description = "Computes the average level of all letters (rounded down), then each letter will have this level.",
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
                    char letter = Util.GetRandomElement(new char[] { 'E', 'S', 'T', 'I', 'A', 'R' });
                    return new EventInfo {
                        name = $"Doomed {letter}",
                        description = $"Gives the Doomed effect to {Util.DecorateArgument(letter)}. Each time the letter scores, it looses a level",
                        typeID = 9,
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
                        description = $"Removes all bonuses, and give 3 random bonuses.",
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
        })();
    }

    public static EventInfo GetRandomBlessing() {
        return Util.GetRandomWithSpawners(new EventSpawner[] {
            new EventSpawner {
                weight = 1.0f,
                data = () => new EventInfo {
                    name = "Doubled letter",
                    description = "Gives the Doubled effect on the third most improved letter. The letter will score twice as many points.",
                    typeID = 0,
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
                    description = "Improves every vowel by 5 levels.",
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
                    description = "Multiplies the level of the most improved letter by 1.4 (rounded up)",
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
                    description = "Removes every negative effect, and letters with level that is less than 3 will have level 3.",
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
                    description = "Gives the Burning effect to the fourth most improved letter. The letter will gain a level when used, but will loose 1 if not used in a word.",
                    typeID = 5,
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
                    description = "Gives a copy of the rightmost bonus, if enough place.",
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
                    description = "Take two random blessing (including this blessing).",
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
                    description = "Go back 2 levels before without loosing anything, so scores will be lower for some time (Random elements will be different)",
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
        })();
    }

    public void OnClick() {
        if (isUnknown) {
            BonusPopup.i.ShowPopup("???", "You haven't discovered this. Take at least once to discover it.", () => {}, "");
        }
        else {
            BonusPopup.i.ShowPopup(info.name, info.description, () => {
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
