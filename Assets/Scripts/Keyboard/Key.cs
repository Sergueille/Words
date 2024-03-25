using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Key : MonoBehaviour
{
    [System.NonSerialized]
    public char letter;
    public Letter Letter {
        get => GameManager.i.GetLetterFromChar(letter);
    }

    public TextMeshProUGUI letterText;
    public TextMeshProUGUI levelText;

    public System.Action<char> onPress;

    public void UpdateTexts() {
        letterText.text = letter.ToString();
        levelText.text = Letter.level.ToString();
    }

    public void OnPress() {
        onPress(letter);
    }
}
