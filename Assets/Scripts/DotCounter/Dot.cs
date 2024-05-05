using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dot : MonoBehaviour
{   
    [System.NonSerialized]
    public Color inactiveColor;
    
    [System.NonSerialized]
    public Color activeColor;

    [SerializeField]
    private float activeScale;
    
    [SerializeField]
    private float inactiveScale;
    
    [SerializeField]
    private float transitionDuration;

    [SerializeField]
    private Image dotImage;

    private bool isActive;

    private void Start()
    {
        isActive = false;
    }

    private void Update()
    {
        if (isActive)
        {
            dotImage.color = activeColor;
        }
        else {
            dotImage.color = inactiveColor;
        }
    }

    public void SetActive(bool newValue)
    {
        isActive = newValue;

        if (isActive)
        {
            LeanTween.scale(dotImage.gameObject, activeScale * Vector3.one, transitionDuration).setEaseOutElastic();
        }
        else
        {
            LeanTween.scale(dotImage.gameObject, inactiveScale * Vector3.one, transitionDuration).setEaseOutElastic();
        }
    }
}
