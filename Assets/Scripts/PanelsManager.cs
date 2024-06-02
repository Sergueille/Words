using System.Collections.Generic;
using UnityEngine;

public class PanelsManager : MonoBehaviour
{
    public static PanelsManager i;

    public GameObject[] panels;

    public CanvasGroup topUI;
    public CanvasGroup bottomUI;

    public MovementDescrWithAmplitude transition;
    public MovementDescrWithAmplitude scaleTransition;

    private void Awake()
    {
        i = this;
        SelectPanel(panels[0].name, true);
        ToggleGameUI(false);
    }

    public void SelectPanel(GameObject panel)
    {
        SelectPanel(panel.name, false);
    }

    public void SelectPanel(string name, bool immediate)
    {
        bool actuallyChanged = GameManager.i.gi.currentPanelName != name;

        foreach (GameObject panel in panels)
        {
            if (panel.name == name)
            {
                ShowPanel(panel, immediate || !actuallyChanged);
            }
            else
            {
                HidePanel(panel, immediate || !actuallyChanged);
            }
        }

        GameManager.i.gi.currentPanelName = name;
    }

    private void HidePanel(GameObject panel, bool immediate)
    {
        panel.SetActive(false);
    }

    private void ShowPanel(GameObject panel, bool immediate)
    {
        panel.SetActive(true);
        RectTransform tr = panel.GetComponent<RectTransform>();

        if (!immediate) {
            transition.DoReverse(t => {
                tr.sizeDelta = Vector2.one * t;
            });

            scaleTransition.DoReverse(t => {
                tr.localScale = Vector2.one * (1 + t);
            });
        }
    }

    public string GetCurrentPanelName()
    {
        return GameManager.i.gi.currentPanelName;
    }

    public void ToggleGameUI(bool enabled)
    {
        topUI.alpha = enabled ? 1 : 0;
        bottomUI.alpha = enabled ? 1 : 0;

        topUI.interactable = enabled;
        bottomUI.interactable = enabled;
    }
}
