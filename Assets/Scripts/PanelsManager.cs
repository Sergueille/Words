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

    public RectTransform transitionCircle;
    public MovementDescrWithAmplitude circleTransition;
    public MovementDescrWithAmplitude circleOutTransition;

    public bool isTransitioning;

    private void Awake()
    {
        i = this;
        SelectPanel(panels[0].name, true);
        ToggleGameUI(false);

        isTransitioning = false;

        transitionCircle.gameObject.SetActive(true);
        transitionCircle.localScale = circleTransition.amplitude * Vector3.one;
        circleOutTransition.DoReverse(t => transitionCircle.localScale = t * Vector3.one)
            .setOnComplete(() => transitionCircle.gameObject.SetActive(false));
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
            isTransitioning = true;
            transition.DoReverse(t => {
                tr.sizeDelta = Vector2.one * t;
            }).setOnComplete(() => isTransitioning = false);

            scaleTransition.DoReverse(t => {
                tr.localScale = Vector2.one * (1 + t);
            });
        }
    }

    public string GetCurrentPanelName()
    {
        return GameManager.i.gi.currentPanelName;
    }

    public void CircleTransition(System.Action callback)
    {
        transitionCircle.localScale = Vector3.zero;
        transitionCircle.gameObject.SetActive(true);
        isTransitioning = true;

        circleTransition.Do(t => transitionCircle.localScale = t * Vector3.one)
            .setOnComplete(() => {
                callback();

                circleOutTransition.DoReverse(t => transitionCircle.localScale = t * Vector3.one).setOnComplete(() => {
                    transitionCircle.gameObject.SetActive(false);
                    isTransitioning = false;
                });
            });
    }

    public void ToggleGameUI(bool enabled)
    {
        topUI.alpha = enabled ? 1 : 0;
        bottomUI.alpha = enabled ? 1 : 0;
        bottomUI.gameObject.SetActive(enabled);

        topUI.interactable = enabled;
        bottomUI.interactable = enabled;
    }
}
