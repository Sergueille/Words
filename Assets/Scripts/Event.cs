
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using EventSpawner = Util.Spawner<Event.EventInfo>;

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
        return Util.GetRandomWithSpawners<Event.EventInfo>(new EventSpawner[] {
            new EventSpawner {
                weight = 1.0f,
                data = new EventInfo {
                    name = "Vowel decrementation",
                    description = "Decrement the level of every vowel.",
                    onCall = () => {
                        for (char a = 'a'; a <= 'z'; a++) {
                            if (Util.IsVowel(a)) {
                                GameManager.i.GetLetterFromChar(a).level--;
                            }
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = new EventInfo {
                    name = "Consonnant decrementation",
                    description = "Decrement the level of every consonnant.",
                    onCall = () => {
                        for (char a = 'a'; a <= 'z'; a++) {
                            if (Util.IsConsonant(a)) {
                                GameManager.i.GetLetterFromChar(a).level--;
                            }
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 1.2f,
                data = new EventInfo {
                    name = "Poisoning",
                    description = "Give the Poisonous effect to the most improved letter. Each time the letter scores, a random letter looses a level.",
                    onCall = () => {
                        GameManager.i.GetNthMostImprovedLetter(1).effect = Letter.Effect.Poisonous; 
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = new EventInfo {
                    name = "Doom",
                    description = "Give the Doomed effect to the third most improved letter. Each time the letter scores, it looses a level.",
                    onCall = () => {
                        GameManager.i.GetNthMostImprovedLetter(3).effect = Letter.Effect.Doomed; 
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = new EventInfo {
                    name = "Shuffle",
                    description = "Level of the letters are shuffled (most impoved letters will receive the lowest levels).",
                    onCall = () => {
                        for (int i = 0; i < 13; i++) {
                            Letter a = GameManager.i.GetNthMostImprovedLetter(i + 1);
                            Letter b = GameManager.i.GetNthMostImprovedLetter(26 - i);

                            int tmp = a.level;
                            a.level = b.level;
                            b.level = tmp;
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = new EventInfo {
                    name = "Tax",
                    description = "The most impoved letter looses 4 levels.",
                    onCall = () => {
                        GameManager.i.GetNthMostImprovedLetter(1).level -= 4; 
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = new EventInfo {
                    name = "Average",
                    description = "Computes the average level of all letters (rouded down), then each letter will have this level.",
                    onCall = () => {
                        int sum = 0;
                        for (char a = 'a'; a <= 'z'; a++) {
                            sum += GameManager.i.GetLetterFromChar(a).level;
                        }

                        int average = sum / 26;

                        for (char a = 'a'; a <= 'z'; a++) {
                            GameManager.i.GetLetterFromChar(a).level = average;
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 0.5f,
                data = new EventInfo {
                    name = "Left destruction",
                    description = "Destroys the leftmost bonus.",
                    onCall = () => {
                        if (GameManager.i.bonuses.Count > 0) {
                            GameManager.i.bonuses.RemoveAt(0);
                        }
                    }
                }
            },
            new EventSpawner {
                weight = 0.5f,
                data = new EventInfo {
                    name = "Right estruction",
                    description = "Destroys the rightmost bonus.",
                    onCall = () => {
                        if (GameManager.i.bonuses.Count > 0) {
                            GameManager.i.bonuses.RemoveAt(GameManager.i.bonuses.Count - 1);
                        }
                    }
                }
            },
        });
    }

    public static EventInfo GetRandomBlessing() {
        return Util.GetRandomWithSpawners<Event.EventInfo>(new EventSpawner[] {
            new EventSpawner {
                weight = 1.0f,
                data = new EventInfo {
                    name = "Polymorphic letter",
                    description = "Gives the Polymorphic effect to the lest improved letter. The letter will not score any point, but will be considered equal to all other letters.",
                    onCall = () => {
                        GameManager.i.GetNthMostImprovedLetter(26).effect = Letter.Effect.Polymorphic; 
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = new EventInfo {
                    name = "Doubled letter",
                    description = "Gives the Doubled effect on the trhird most improved letter. The letter will score twice as many points.",
                    onCall = () => {
                        GameManager.i.GetNthMostImprovedLetter(3).effect = Letter.Effect.Doubled; 
                    }
                }
            },
            new EventSpawner {
                weight = 1.0f,
                data = new EventInfo {
                    name = "Incrementation",
                    description = "Improves every leter by one level.",
                    onCall = () => {
                        for (char a = 'a'; a <= 'z'; a++) {
                            GameManager.i.GetLetterFromChar(a).level++;
                        }
                    }
                }
            },
        });
    }

    public void OnClick() {
        BonusPopup.i.ShowPopup(info.name, info.description, () => {
            info.onCall();
            Keyboard.i.UpdateAllKeys();
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
