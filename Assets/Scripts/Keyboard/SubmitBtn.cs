using System.Collections.Generic;
using UnityEngine;

public class SubmitBtn : MonoBehaviour
{
    public RectTransform icon;

    public void OnPress() 
    {
        LeanTween.cancel(icon.gameObject);

        icon.localScale = 1.5f * Vector3.one;
        LeanTween.scale(icon.gameObject, Vector2.one, 0.4f).setEaseOutExpo();

        GameManager.i.SubmitWord();
    }
}
