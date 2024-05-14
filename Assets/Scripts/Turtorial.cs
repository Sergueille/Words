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
        public System.Action<bool> predicate = null;
    }

    public RectTransform popupParent;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI mainText;
    public Button popupButton;
    public Transform defaultPosition;
    public RectTransform highlightGraphic;
    public RectTransform canvasTransform;
    public float marginAroundPopup;

    public MovementDescr popupTransition;
    public MovementDescr highlightTransition;


    private TutorialEntry[] entries;

    private int currentStep = 0;

    private void Awake()
    {
        i = this;
        popupParent.gameObject.SetActive(false);
        highlightGraphic.gameObject.SetActive(false);
    }


    public void StartTutorial()
    {
        entries = new TutorialEntry[] {
            new TutorialEntry {
                title = "The tutorial",
                description = "This tutorial will try to explain how to play this game.",
            },
            new TutorialEntry {
                title = "Basic rules",
                description = "In this game, you have to beat each level by reaching the required score.",
                highlight = GameManager.i.targetScoreText.gameObject,
            },
            new TutorialEntry {
                title = "Basic rules",
                description = "To do that, you can enter 3 words.",
                highlight = GameManager.i.wordsCounter.gameObject,
            },
            new TutorialEntry {
                title = "Basic rules",
                description = "Each letter will score 1 (or 2) points. You can see it on the keyboard.",
                highlight = Keyboard.i.keyboardParent.gameObject,
            },
            new TutorialEntry {
                title = "Basic rules",
                description = "Each word must satisfy a constraint, shown here.",
                highlight = GameManager.i.constraintText.gameObject,
                positionDirection = PositionDirection.Down,
            },
            new TutorialEntry {
                title = "First level",
                description = "Now, try to beat the first level.",
            },
        };

        Debug.Log("Hey!");

        popupParent.gameObject.SetActive(true);
        highlightGraphic.gameObject.SetActive(true);

        currentStep = 0;
        ShowStep(true);
    }

    public void NextStep()
    {
        currentStep++;
        ShowStep(false);
    }

    private void ShowStep(bool immediate)
    {
        TutorialEntry entry = entries[currentStep];

        // Set popup content
        titleText.text = entry.title;
        mainText.text = entry.description;

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
            targetPopupPosition = defaultPosition.localPosition;
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

            // Make sure popup size will be up to date
            LayoutRebuilder.ForceRebuildLayoutImmediate(canvasTransform);

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
