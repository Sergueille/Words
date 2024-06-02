using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BonusPopup : MonoBehaviour
{
    public static BonusPopup i;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI btnText;
    [SerializeField] private System.Action onValidate;

    [SerializeField] private GameObject popupParent;
    [SerializeField] private CanvasGroup canvasGroup;

    public bool isOpen = false;
    
    
    private void Awake()
    {
        i = this;
        popupParent.SetActive(false);
    }

    public void ShowPopup(string title, string description, System.Action onValidate, string btnText)
    {
        isOpen = true;
        nameText.text = title;
        descriptionText.text = description;

        this.onValidate = onValidate;
        this.btnText.text = btnText;
        
        popupParent.transform.localScale = Vector3.zero;
        LeanTween.scale(popupParent, Vector3.one, 0.3f).setEaseOutBack();
        canvasGroup.alpha = 0;
        LeanTween.alphaCanvas(canvasGroup, 1, 0.3f);

        popupParent.SetActive(true);
    }   

    public void HidePopup()
    {
        isOpen = false;
        LeanTween.scale(popupParent, Vector3.zero, 0.2f).setEaseInQuad();
        LeanTween.alphaCanvas(canvasGroup, 0, 0.2f).setOnComplete(() => {
            popupParent.SetActive(false);
        });
    }

    public void OnClick()
    {
        onValidate();
    }
}
