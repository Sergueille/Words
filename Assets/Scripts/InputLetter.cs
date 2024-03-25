using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputLetter : MonoBehaviour
{
    public TextMeshProUGUI displayText;

    [NonSerialized]
    public char letter;
    public Letter Letter {
        get => GameManager.i.GetLetterFromChar(letter);
    }

    public void TriggerCreationAnimation() {
        displayText.text = char.ToUpper(letter).ToString();
    }

    public void DestroyWithAnimation() {
        Destroy(gameObject);
    }
}
