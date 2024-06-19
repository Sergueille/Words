using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Animations;

public class InputLetter : MonoBehaviour
{
    public TextMeshProUGUI displayText;

    [SerializeField] private Color appearColor;
    [SerializeField] private MovementDescr goToScoreMovement;


    [NonSerialized]
    public char letter;
    public Letter Letter {
        get => GameManager.i.GetLetterFromChar(letter);
    }

    public void TriggerCreationAnimation() {
        displayText.text = char.ToUpper(letter).ToString();

        displayText.color = appearColor;
        displayText.transform.localScale = Vector3.one * 2.0f;
        Util.LeanTweenTextColor(displayText, new Color(1, 1, 1, 1), 0.4f).setEaseOutExpo();
        LeanTween.scale(displayText.gameObject, Vector3.one, 0.4f).setEaseOutExpo();
        Util.LeanTweenShake(displayText.gameObject, 20, 0.4f);
    }

    public void DestroyWithAnimation(bool goToScore) {
        transform.SetParent(GameManager.i.canvasTransform, true);

        if (goToScore) {
            goToScoreMovement.DoWithBounds(
                (pos) => transform.localPosition = pos,
                transform.localPosition,
                GameManager.i.totalScoreText.transform.localPosition
            ).setOnComplete(() => {
                Destroy(gameObject);
            });

            goToScoreMovement.DoReverse(t => transform.localScale = Vector3.one * t);
        }
        else {
            Destroy(gameObject);
        }
    }
}
