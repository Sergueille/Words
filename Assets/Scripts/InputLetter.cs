using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputLetter : MonoBehaviour
{
    public TextMeshProUGUI displayText;

    [SerializeField] private Color appearColor;

    [NonSerialized]
    public char letter;
    public Letter Letter {
        get => GameManager.i.GetLetterFromChar(letter);
    }

    public void TriggerCreationAnimation() {
        displayText.text = char.ToUpper(letter).ToString();

        displayText.color = appearColor;
        displayText.transform.localScale = Vector3.one * 1.5f;
        Util.LeanTweenTextColor(displayText, new Color(1, 1, 1, 1), 0.2f).setEaseOutExpo();
        LeanTween.scale(displayText.gameObject, Vector3.one, 0.2f).setEaseOutExpo();
        Util.LeanTweenShake(displayText.gameObject, 20, 0.2f);
    }

    public void DestroyWithAnimation() {
        Destroy(gameObject);
    }
}
