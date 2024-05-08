using System.Collections.Generic;
using UnityEngine;

public class PanelsManager : MonoBehaviour
{
    public static PanelsManager i;

    public GameObject[] panels;

    public CanvasGroup topUI;
    public CanvasGroup bottomUI;

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
        GameManager.i.gi.currentPanelName = name;

        foreach (GameObject panel in panels)
        {
            if (panel.name == name)
            {
                ShowPanel(panel, immediate);
            }
            else
            {
                HidePanel(panel, immediate);
            }
        }
    }

    private void HidePanel(GameObject panel, bool immediate)
    {
        panel.SetActive(false);
    }

    private void ShowPanel(GameObject panel, bool immediate)
    {
        panel.SetActive(true);
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
