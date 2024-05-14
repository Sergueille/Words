using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySelector : MonoBehaviour
{
    public GameObject prefab;
    public RectTransform UIParent;
    public string[] titles;
    public string[] descriptions;

    public MovementDescr transition;

    public Button playBtn;

    public int selectedDifficulty = 0;

    private void Start()
    {
        for (int i = 0; i < (int)GameMode.MaxValue; i++)
        {
            DifficultyUI ui = Instantiate(prefab, UIParent).GetComponent<DifficultyUI>();
            ui.title.text = titles[i];
            ui.description.text = descriptions[i];
        }

        SelectDifficulty((GameMode)selectedDifficulty);
    }

    public void SelectNext()
    {
        SelectDifficulty((GameMode)(
            (selectedDifficulty + 1) % (int)GameMode.MaxValue
        ));
    }

    public void SelectPrevious()
    {
        SelectDifficulty((GameMode)(
            (selectedDifficulty - 1 + (int)GameMode.MaxValue) % (int)GameMode.MaxValue
        ));
    }

    public void SelectDifficulty(GameMode mode)
    {
        int modeId = (int)mode;
        selectedDifficulty = modeId;

        float width = UIParent.sizeDelta.x;
        float diffWidth = width / (int)GameMode.MaxValue;
        float centerPosition = (modeId + 0.5f) * diffWidth;
        float targetPosition = - centerPosition + width * 0.5f;

        transition.DoWithBounds(t => UIParent.anchoredPosition = new Vector3(t, 0, 0), UIParent.anchoredPosition.x, targetPosition);
    }

    public void StartRun()
    {
        GameManager.i.StartNewRun((GameMode)selectedDifficulty);
    }
}
