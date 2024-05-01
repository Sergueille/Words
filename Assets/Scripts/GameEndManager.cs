using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameEndManager : MonoBehaviour
{
    public static GameEndManager i;
    
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI wordCountText;
    public TextMeshProUGUI mostUsedLetterText;

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
        PanelsManager.i.SelectPanel("GameEnd", false);
    }

    private void UpdateText()
    {
        Stats s = GameManager.i.gameStats;
        bestScoreText.text = $"Word with best score: {Util.DecorateArgument(s.bestScoreWord)} ({Util.DecorateArgument(s.bestScore)} points)";
        wordCountText.text = $"{Util.DecorateArgument(s.wordCount)} words written";

        char mostUsedLetter = '\0';
        int mostUsedLetterUses = 0;
        for (int i = 0; i < GameManager.i.letters.Length; i++) {
            Letter l = GameManager.i.letters[i];
            if (l.timesUsed >= mostUsedLetterUses) {
                mostUsedLetterUses = l.timesUsed;
                mostUsedLetter = l.letter;
            }
        }

        mostUsedLetterText.text = $"The most used letter is {Util.DecorateArgument(mostUsedLetter)} ({Util.DecorateArgument(mostUsedLetterUses)} times)";
    }
}
