using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [SerializeField] private Image improveImage;
    [SerializeField] private Color improveImageColor;
    private int lastLevel;

    private void Awake()
    {
        improveImage.gameObject.SetActive(false);
        lastLevel = 1;
    }

    public void UpdateUI() {
        letterText.text = letter.ToString();
        levelText.text = Letter.level.ToString();

        if (lastLevel != -1 && Letter.level != lastLevel)
        {
            lastLevel = Letter.level;

            improveImage.transform.localScale = Vector3.one;
            LeanTween.scale(improveImage.gameObject, Vector3.one * 2, 0.7f).setEaseOutExpo();

            Color targetColor = new Color(improveImageColor.r, improveImageColor.g, improveImageColor.b, 0);
            improveImage.color = improveImageColor;
            Util.LeanTweenImageColor(improveImage, targetColor, 0.7f).setOnComplete(() => improveImage.gameObject.SetActive(false));
            improveImage.gameObject.SetActive(true);
        }
    }

    public void OnPress() {
        onPress(letter);
    }
}
