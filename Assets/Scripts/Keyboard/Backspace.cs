using System.Collections.Generic;
using UnityEngine;

public class Backspace : MonoBehaviour
{
    public RectTransform icon;

    public float initialHoldDelay;
    public float holdDelay;

    private float pointerDownTime;
    private float lastBackspaceTime;
    private bool pointerDown = false;

    public void OnPointerDown() {
        pointerDown = true;
        pointerDownTime = Time.time;
        lastBackspaceTime = Time.time;
        TriggerBackspace();
    }

    public void OnPointerUp() {
        pointerDown = false;
    }

    private void Update() {
        if (pointerDown && Time.time - pointerDownTime > initialHoldDelay) {
            if (Time.time - lastBackspaceTime > holdDelay) {
                TriggerBackspace();
                lastBackspaceTime = Time.time;
            }
        }
    }

    public void TriggerBackspace() 
    {
        LeanTween.cancel(icon.gameObject);

        icon.localScale = 1.5f * Vector3.one;
        LeanTween.scale(icon.gameObject, Vector2.one, 0.4f).setEaseOutExpo();

        GameManager.i.EraseLastLetter();
    }
}
