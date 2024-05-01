using System.Collections.Generic;
using UnityEngine;

public class PanelsManager : MonoBehaviour
{
    public static PanelsManager i;

    public GameObject[] panels;

    private string currentPanelName;
    
    private void Awake()
    {
        i = this;
        SelectPanel(panels[0].name, true);
    }

    public void SelectPanel(string name, bool immediate)
    {
        currentPanelName = name;

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
        return currentPanelName;
    }
}
