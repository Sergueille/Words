using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputLetter : MonoBehaviour
{
    public TextMeshProUGUI displayText;

    [SerializeField] private Color appearColor;
    [SerializeField] private MovementDescr goToScoreMovement;

    [SerializeField] private float goToScorePositionDelta;


    [System.NonSerialized]
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

        Vector2 delta = Random.insideUnitCircle * goToScorePositionDelta;

        if (goToScore) {
            goToScoreMovement.DoWithBounds(
                (pos) => transform.localPosition = pos,
                transform.localPosition,
                GameManager.i.totalScoreText.transform.localPosition + new Vector3(delta.x, delta.y, 0)
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
