using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameEndManager : MonoBehaviour
{
    public static GameEndManager i;
    
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI wordCountText;
    public TextMeshProUGUI mostUsedLetterText;
    public TextMeshProUGUI bestWordText;

    private void Awake()
    {
        i = this;
    }

    private void Update()
    {
        if (PanelsManager.i.GetCurrentPanelName() == "GameEnd") 
        {
            UpdateText();
        }
    }
    
    public void Do() {
        bestWordText.text = "Searching for the best word...";

        StartCoroutine(GameManager.i.FindBestWord(
            ColorManager.i.ColorChangedThisFrame,
            (word, score) => {
                bestWordText.text = $"The best word for this level was {Util.DecorateArgument(word)} ({Util.DecorateArgument(score)} points)";
            }
        ));

        GameManager.i.continueBtn.gameObject.SetActive(false); // Hide continue button in main menu

        PanelsManager.i.SelectPanel("GameEnd", false);
    }

    private void UpdateText()
    {
        Stats s = GameManager.i.gi.gameStats;
        bestScoreText.text = $"Word with best score: {Util.DecorateArgument(s.bestScoreWord)} ({Util.DecorateArgument(s.bestScore)} points)";
        wordCountText.text = $"{Util.DecorateArgument(s.wordCount)} words written";

        char mostUsedLetter = '\0';
        int mostUsedLetterUses = 0;
        for (int i = 0; i < GameManager.i.gi.letters.Length; i++) {
            Letter l = GameManager.i.gi.letters[i];
            if (l.timesUsed >= mostUsedLetterUses) {
                mostUsedLetterUses = l.timesUsed;
                mostUsedLetter = l.letter;
            }
        }

        mostUsedLetterText.text = $"The most used letter is {Util.DecorateArgument(mostUsedLetter)} ({Util.DecorateArgument(mostUsedLetterUses)} times)";
    }
}
