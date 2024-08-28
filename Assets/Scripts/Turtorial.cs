using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

public class Tutorial : MonoBehaviour
{
    public static Tutorial i;
    
    public enum PositionDirection {
        Up, Left, Right, Down 
    }

    public class TutorialEntry {
        public string title;
        public string description;
        public GameObject highlight = null;
        public PositionDirection positionDirection = PositionDirection.Up;
        public System.Func<bool> predicate = null;
        public bool preventAction = true;
        public bool hiddenPanel = false;
    }

    public RectTransform popupParent;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI mainText;
    public Button popupButton;
    public Transform defaultPosition;
    public Transform hiddenPosition;
    public RectTransform highlightGraphic;
    public RectTransform canvasTransform;
    public float marginAroundPopup;

    public MovementDescr popupTransition;
    public MovementDescr highlightTransition;


    private TutorialEntry[] entries;

    private int currentStep = 0;
    public bool TutorialPreventsAction { get; private set; }
    public bool IsTutorialActive { get; private set; }

    private void Awake()
    {
        i = this;
        popupParent.gameObject.SetActive(false);
        highlightGraphic.gameObject.SetActive(false);
        TutorialPreventsAction = false;
        IsTutorialActive = false;
    }

    private void Update()
    {
        if (IsTutorialActive)
        {
            TutorialEntry entry = entries[currentStep];
            if (entry.predicate != null)
            {
                if (entry.predicate())
                {
                    NextStep();
                }
            }
        }
    }

    public void StartTutorial()
    {
        entries = new TutorialEntry[] {
            new TutorialEntry {
                title = "Welcome!",
                description = "This tutorial will guide you through the basics of playing the game.",
            },
            new TutorialEntry {
                title = "Basic rules",
                description = "In this game, you have to beat each level by reaching the required score.",
                highlight = GameManager.i.targetScoreText.gameObject,
            },
            new TutorialEntry {
                title = "Basic rules",
                description = "To achieve this, you can enter up to 3 words.",
                highlight = GameManager.i.wordsCounter.gameObject,
            },
            new TutorialEntry {
                title = "Basic rules",
                description = "Each letter in the words you enter will score 1 (or 2) points. You can track this on the keyboard.",
                highlight = Keyboard.i.keyboardParent.gameObject,
            },
            new TutorialEntry {
                title = "Basic rules",
                description = "You can type any word, but it must meet the constraint shown here.",
                highlight = GameManager.i.constraintText.gameObject,
                positionDirection = PositionDirection.Down,
            },
            new TutorialEntry {
                title = "First level",
                description = "Now, try to beat the first level. Use long words to score more points!",
            },
            new TutorialEntry {
                preventAction = false,
                hiddenPanel = true,
                predicate = () => PanelsManager.i.GetCurrentPanelName() == "Bonus",
            },
            new TutorialEntry {
                title = "Great!",
                description = "Great job! You've completed the first level. Now you can choose a bonus!",
            },
            new TutorialEntry {
                title = "Bonuses",
                description = "Bonuses remain with you for the rest of the game, allowing you to improve your letters or score more points",
            },
            new TutorialEntry {
                title = "Bonuses",
                description = "Each bonus has a specific ability. Click on them to see what they do!",
                highlight = BonusManager.i.bonusParent.gameObject,
                predicate = () => BonusPopup.i.isOpen,
                preventAction = false,
            },
            new TutorialEntry {
                preventAction = false,
                hiddenPanel = true,
                predicate = () => PanelsManager.i.GetCurrentPanelName() == "Main" && !ColorManager.i.ColorChangedThisFrame(),
            },
            new TutorialEntry {
                title = "Bonuses",
                description = "You can have up to 4 bonuses. Click on them anytime to review their abilities or remove them. Give it a try!",
                highlight = GameManager.i.bonusParent.gameObject,
                positionDirection = PositionDirection.Down,
                predicate = () => BonusPopup.i.isFullyOpen,
                preventAction = false,
            },
            new TutorialEntry {
                title = "Bonuses",
                description = "We won't remove the bonus just yet.",
                highlight = BonusPopup.i.dismissButton.gameObject,
                positionDirection = PositionDirection.Down,
                predicate = () => !BonusPopup.i.isOpen,
                preventAction = false,
            },
            new TutorialEntry {
                title = "Almost finished",
                description = "One last thing: completing a level using fewer than 3 words increases this counter for each unused word.",
                highlight = GameManager.i.blessingCounter.gameObject,
                positionDirection = PositionDirection.Down,
            },
            new TutorialEntry {
                title = "Almost finished",
                description = "When it's full, expect a surprise!",
                highlight = GameManager.i.blessingCounter.gameObject,
                positionDirection = PositionDirection.Down,
            },
            new TutorialEntry {
                title = "Almost finished",
                description = "The required score for each level will increase exponentially. Your goal is to reach 1000 points. Good luck!",
            },
        };

        TutorialPreventsAction = true;
        IsTutorialActive = true;

        popupParent.gameObject.SetActive(true);
        highlightGraphic.gameObject.SetActive(true);

        popupParent.parent.gameObject.SetActive(true);

        currentStep = 0;
        ShowStep(true);
    }

    public void NextStep()
    {
        currentStep++;

        if (currentStep < entries.Length)
        {
            ShowStep(false);
        }
        else 
        {
            EndTutorial();
        }
    }

    public void EndTutorial() {
        TutorialPreventsAction = false;
        IsTutorialActive = false;
        popupParent.gameObject.SetActive(false);
        highlightGraphic.gameObject.SetActive(false);
    }

    private void ShowStep(bool immediate)
    {
        TutorialEntry entry = entries[currentStep];

        TutorialPreventsAction = entry.preventAction;

        // Set popup content
        if (!entry.hiddenPanel) 
        {
            titleText.text = entry.title;
            mainText.text = entry.description;
        }

        // show button if not callback set
        popupButton.gameObject.SetActive(entry.predicate == null);

        Vector2 targetHighlightPosition;
        Vector2 targetPopupPosition;
        Vector2 targetHighlightSize;

        // Force reset anchors
        highlightGraphic.anchorMin = Vector3.one * 0.5f;
        highlightGraphic.anchorMax = Vector3.one * 0.5f;
        popupParent.anchorMin = Vector3.one * 0.5f;
        popupParent.anchorMax = Vector3.one * 0.5f;

        if (entry.highlight == null) {
            // Place popup on default position

            if (entry.hiddenPanel) {
                targetPopupPosition = hiddenPosition.localPosition;
            }
            else {
                targetPopupPosition = defaultPosition.localPosition;
            }

            targetHighlightPosition = new Vector2(0, 0);
            targetHighlightSize = canvasTransform.sizeDelta;
        }
        else {
            Bounds highlightBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(canvasTransform, entry.highlight.GetComponent<RectTransform>());
            
            // Place highlight on bounds
            highlightGraphic.anchorMin = Vector3.one * 0.5f;
            highlightGraphic.anchorMax = Vector3.one * 0.5f;
            targetHighlightPosition = highlightBounds.center;
            targetHighlightSize = highlightBounds.size;

            if (entry.hiddenPanel) {
                targetPopupPosition = hiddenPosition.localPosition;
            }
            else {
                // Make sure popup size will be up to date
                LayoutRebuilder.ForceRebuildLayoutImmediate(popupParent);

                // Place popup
                Vector2 distanceWithCenter = (Vector3)popupParent.sizeDelta * 0.5f + Vector3.one * marginAroundPopup + highlightBounds.size * 0.5f;
                Vector2 boundsCenter = highlightBounds.center;
                if (entry.positionDirection == PositionDirection.Up) {
                    targetPopupPosition = new Vector3(
                        boundsCenter.x,
                        boundsCenter.y + distanceWithCenter.y,
                        0
                    );
                }
                else if (entry.positionDirection == PositionDirection.Down) {
                    targetPopupPosition = new Vector3(
                        boundsCenter.x,
                        boundsCenter.y - distanceWithCenter.y,
                        0
                    );
                }
                else if (entry.positionDirection == PositionDirection.Right) {
                    targetPopupPosition = new Vector3(
                        boundsCenter.x + distanceWithCenter.x,
                        boundsCenter.y,
                        0
                    );
                }
                else if (entry.positionDirection == PositionDirection.Left) {
                    targetPopupPosition = new Vector3(
                        boundsCenter.x - distanceWithCenter.x,
                        boundsCenter.y,
                        0
                    );
                }
                else throw new InvalidOperationException("Unreachable!");
            }
        }
        

        if (immediate) {
            popupParent.anchoredPosition = targetPopupPosition;
            highlightGraphic.anchoredPosition = targetHighlightPosition;
            highlightGraphic.sizeDelta = targetHighlightSize;
        }
        else {
            popupTransition.DoWithBounds(pos => {
                popupParent.anchoredPosition = pos;
            }, popupParent.anchoredPosition, targetPopupPosition);

            highlightTransition.DoWithBounds(pos => {
                highlightGraphic.anchoredPosition = pos;
            }, highlightGraphic.anchoredPosition, targetHighlightPosition);
            
            highlightTransition.DoWithBounds(size => {
                highlightGraphic.sizeDelta = size;
            }, highlightGraphic.sizeDelta, targetHighlightSize);
        }
    }
}
