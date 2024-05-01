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
    public Image background;
    public Color baseColor;
    public Color selectedColor;

    public System.Action<char> onPress;

    private int lastLevel = -1;

    [System.NonSerialized] public bool isSelected = false;

    private void Update()
    {
        if (isSelected)
        {
            background.color = selectedColor; 
        }
        else 
        {
            background.color = baseColor; 
        }
    }

    public void UpdateUI() 
    {
        letterText.text = letter.ToString();
        levelText.text = Letter.level.ToString();

        if (lastLevel != -1 && Letter.level != lastLevel)
        {
            ParticlesManager.i.CircleParticles(transform.position, 3.0f);
        }

        lastLevel = Letter.level;
    }

    public void OnPress() 
    {
        letterText.transform.localScale = 1.5f * Vector3.one;
        LeanTween.cancel(letterText.gameObject);
        LeanTween.scale(letterText.gameObject, Vector2.one, 0.4f).setEaseOutExpo();

        onPress(letter);
    }

    public void Select(bool selected) 
    {
        isSelected = selected;

        Color newColor = selected ? selectedColor : baseColor;
        Util.LeanTweenImageColor(background, newColor, 0.3f).setEaseOutExpo();
        Util.LeanTweenShake(gameObject, 20, 0.3f);
    }
}
