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
    public Color doomedColor;
    public float levelTextShakeAmount;

    public ParticleSystem poisonParticles;
    public ParticleSystem electricParticles;
    public ParticleSystem fireParticles;
    public Image lockedIcon;

    public System.Action<char> onPress;

    private int lastLevel = -1;
    private Vector3 levelTextInitialPosition;

    [System.NonSerialized] public bool isSelected = false;

    private IEnumerator<object> Start()
    {        
        poisonParticles.gameObject.SetActive(false);
        electricParticles.gameObject.SetActive(false);
        fireParticles.gameObject.SetActive(false);
        lockedIcon.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        levelTextInitialPosition = levelText.transform.localPosition;
    }

    private void Update()
    {
        if (isSelected)
        {
            background.color = selectedColor; 
        }
        else 
        {
            if (Letter != null && Letter.effect == Letter.Effect.Doomed)
            {
                background.color = doomedColor; 
            }
            else
            {
                background.color = baseColor; 
            }
        }
    }

    public void UpdateUI(bool particles) 
    {
        letterText.text = letter.ToString();

        if (Letter.effect == Letter.Effect.Doubled)
        {
            levelText.text = $"{Letter.Level}x2";
        }
        else if (Letter.effect == Letter.Effect.Polymorphic)
        {
            levelText.text = $"??";
        }
        else
        {
            levelText.text = Letter.Level.ToString();
        }

        if (particles && Letter.Level != lastLevel)
        {
            if (Letter.Level < lastLevel)
            {
                levelText.transform.localPosition = levelTextInitialPosition + Vector3.right * levelTextShakeAmount;
                LeanTween.moveLocalX(levelText.gameObject, levelTextInitialPosition.x, 0.8f).setEaseOutElastic();
            }   
            else if (Letter.Level > lastLevel)
            {
                ParticlesManager.i.CircleParticles(transform.position, 3.0f);
            }
        }

        poisonParticles.gameObject.SetActive(Letter.effect == Letter.Effect.Poisonous);
        electricParticles.gameObject.SetActive(Letter.effect == Letter.Effect.Electric);
        fireParticles.gameObject.SetActive(Letter.effect == Letter.Effect.Burning);
        lockedIcon.gameObject.SetActive(Letter.effect == Letter.Effect.Locked);

        lastLevel = Letter.Level;
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
