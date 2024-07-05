using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySelector : MonoBehaviour
{
    public GameObject prefab;
    public RectTransform UIParent;
    public string[] titles;
    public string[] descriptions;

    public RectTransform UIPanel;

    public MovementDescr transition;

    public Button playBtn;

    public int selectedDifficulty = 0;

    private DifficultyUI[] uis;

    private void Start()
    {
        uis = new DifficultyUI[(int)GameMode.MaxValue];

        for (int i = 0; i < (int)GameMode.MaxValue; i++)
        {
            DifficultyUI ui = Instantiate(prefab, UIParent).GetComponent<DifficultyUI>();
            ui.title.text = titles[i];
            ui.description.text = descriptions[i];

            uis[i] = ui;
        }
        
        if (GameManager.i.progression.alreadyPlayed) 
        {
            selectedDifficulty = 1;
        }
        else 
        {
            selectedDifficulty = 0;
        }

        UIPanel.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(UIParent);
        SelectDifficulty((GameMode)selectedDifficulty, true);
        UIPanel.gameObject.SetActive(false);
    }

    public void PrepareUI() 
    {
        SelectDifficulty((GameMode)selectedDifficulty, true);

        for (int i = 0; i < (int)GameMode.MaxValue; i++) {
            int bestLevel = GameManager.i.progression.bestLevels.Get(i);

            uis[i].neverPlayedLabel.SetActive(bestLevel == 0 && i != 0);
            uis[i].scoreText.gameObject.SetActive(bestLevel > 0 && i != 0);
            uis[i].scoreText.text = bestLevel.ToString();
            uis[i].scoreLabel.SetActive(bestLevel > 0 && i != 0);
            uis[i].completedLabel.SetActive(bestLevel >= GameManager.i.thousandLevel && i != 0);
        }
    }

    public void SelectNext()
    {
        SelectDifficulty((GameMode)(
            (selectedDifficulty + 1) % (int)GameMode.MaxValue
        ), false);
    }

    public void SelectPrevious()
    {
        SelectDifficulty((GameMode)(
            (selectedDifficulty - 1 + (int)GameMode.MaxValue) % (int)GameMode.MaxValue
        ), false);
    }

    public void SelectDifficulty(GameMode mode, bool immediate)
    {
        int modeId = (int)mode;
        selectedDifficulty = modeId;

        float width = UIParent.sizeDelta.x;
        float diffWidth = width / (int)GameMode.MaxValue;
        float centerPosition = (modeId + 0.5f) * diffWidth;
        float targetPosition = - centerPosition + width * 0.5f;

        if (immediate)
        {
            UIParent.anchoredPosition = new Vector3(targetPosition, 0, 0);
        }
        else
        {
            transition.DoWithBounds(t => UIParent.anchoredPosition = new Vector3(t, 0, 0), UIParent.anchoredPosition.x, targetPosition);
        }
    }

    public void StartRun()
    {
        GameManager.i.StartNewRun((GameMode)selectedDifficulty);
    }
}
