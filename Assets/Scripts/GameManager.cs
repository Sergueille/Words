using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{   
    public static GameManager i;
    
    public int minWordLength = 3;
    public int maxWordLength = 12;
    public int wordsPerLevel = 3;
    public int maxBonus = 4;
    
    public float scoreExpMultiplier = 20.0f;
    public float scoreExpSpeed = 0.15f;

    public Transform inputParent;
    public GameObject inputLetterPrefab;

    [System.NonSerialized] public List<Bonus> bonuses;

    private Letter[] letters;

    private InputLetter[] inputLetters;
    private string inputWord;
    private int totalScore;
    private Constraint currentConstraint;
    private int currentLevel;
    private int levelWords = 0;

    private int lastCurrentScore = -1;


    public TextMeshProUGUI errorText;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI totalScoreText;
    public TextMeshProUGUI targetScoreText;
    public TextMeshProUGUI constraintText;
    public DotCounter wordsCounter;

    
    private void Awake()
    {
        i = this;
        letters = new Letter[26];

        for (int i = 0; i < 26; i++) {
            letters[i] = new Letter {
                letter = (char)((int)'A' + i),
                level = 1,
            };
        }

        inputWord = "";

        bonuses = new List<Bonus>();
    }

    private void Start() 
    {
        Word.Init();
        HideError(true);

        currentLevel = -1;
        StartLevel();
    }

    private void LevelCompleted()
    {
        System.Action onFinished = () => {
            PanelsManager.i.SelectPanel("Main", false);
            StartLevel();
        };

        if (currentLevel % 2 == 0) {
            BonusManager.i.Do(onFinished);
        }
        else {
            ImprovementManager.i.Do(onFinished);
        }

    }

    private void StartLevel()
    {
        currentLevel++;
        levelWords = 0;
        wordsCounter.SetValue(1);
        ChangeConstraint();
        UpdateTotalScore(0, false);
        UpdateTargetScoreText(false);
    } 

    public Letter GetLetterFromChar(char c)
    {
        char lower = char.ToLower(c);
        return letters[(int)lower - (int)'a'];
    }

    public void Update() 
    {
        int currentScore = 0;
        
        if (CheckIfWordAllowed())
        {
            currentScore = ComputeWordScore();
        }
        else
        {
            currentScore = 0;
        }

        UpdateCurrentScoreText(currentScore);

        // Record keys on computer
        if (Input.anyKeyDown)
        {
            foreach(char c in Input.inputString)
            {
                if (c >= 'a' && c <= 'z')
                    InputLetter(c);
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            EraseLastLetter();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SubmitWord();
        }
    }


    /// <summary>
    /// Will trigger error animation     
    /// </summary>
    public bool CheckIfWordAllowed()
    {        
        if (!Word.IsWordAllowed(inputWord)) 
        {
            ShowError("This isn't a word");
        }
        else if (inputWord.Length > maxWordLength) 
        {
            ShowError($"The word should have at most {maxWordLength} letters");
        }
        else if (inputWord.Length < minWordLength) 
        {
            ShowError($"The word should have at least {minWordLength} letters");
        }
        else if (!currentConstraint.IsWordAllowed(inputWord))
        {
            ShowError("The constraint isn't respected");
        }
        else  
        {
            HideError(false);
            return true;
        }

        return false;
    }


    public void SubmitWord() 
    {
        StartCoroutine(Coroutine());

        IEnumerator<WaitForSeconds> Coroutine() {
            if (CheckIfWordAllowed())
            {
                int score = ComputeWordScore();
                
                UpdateTotalScore(totalScore + score, false);
                UpdateCurrentScoreText(0);
                ClearInput();

                if (totalScore >= GetLevelTargetScore())
                {
                    yield return new WaitForSeconds(0.5f);
                    LevelCompleted();
                    yield break;
                }
                else if (levelWords >= wordsPerLevel)
                {
                    // TODO: game over!
                    Debug.Log("Game Over!");
                }
                
                levelWords++;
                wordsCounter.SetValue(levelWords + 1);
                ChangeConstraint();
            }
        }
    }

    private void UpdateCurrentScoreText(int currentScore) 
    {
        if (lastCurrentScore == currentScore)
            return;

        if (currentScore == 0)
        {
            LeanTween.scale(currentScoreText.gameObject, Vector3.zero, 0.2f).setEaseOutQuad();
        }
        else
        {
            LeanTween.scale(currentScoreText.gameObject, Vector3.one, 0.2f).setEaseOutQuad();

            Util.LeanTweenShake(currentScoreText.gameObject, 15.0f, 0.5f);
            currentScoreText.text = $"+{currentScore}";
        }

        lastCurrentScore = currentScore;
    }

    private void UpdateTotalScore(int newValue, bool immediate) 
    {
        if (totalScore == newValue) return;

        totalScore = newValue;

        if (!immediate)
        {
            Util.LeanTweenShake(totalScoreText.gameObject, 20, 0.4f);
        }

        totalScoreText.text = totalScore.ToString();
    }

    private void UpdateTargetScoreText(bool immediate) 
    {
        targetScoreText.text = $"/{GetLevelTargetScore().ToString()}";
    }


    /// <summary>
    /// Shows the error text, triggering an animation
    /// </summary>
    private void ShowError(string error) 
    {
        errorText.text = error;
    }

    /// <summary>
    /// Hides the error text, triggering an animation
    /// </summary>
    private void HideError(bool immediate) 
    {
        errorText.text = "";
    }

    /// <summary>
    /// Adds a letter to the input word, triggering an animation
    /// </summary>
    public void InputLetter(char c) 
    {
        InputLetter newLetter = Instantiate(inputLetterPrefab, inputParent).GetComponent<InputLetter>();
        newLetter.letter = c;
        newLetter.TriggerCreationAnimation();

        inputWord = inputWord.Insert(inputWord.Length, c.ToString());
    }

    /// <summary>
    /// Clears the input, triggering an animation
    /// </summary>
    public void ClearInput() 
    {
        foreach (Transform child in inputParent) 
        {
	        Destroy(child.gameObject);
        }

        inputWord = "";
    }

    /// <summary>
    /// Removes a letter from the input word, triggering an animation
    /// </summary>
    public void EraseLastLetter() 
    {
        if (inputWord.Length > 0) 
        {
            InputLetter lastLetter = inputParent.GetChild(inputParent.childCount - 1).GetComponent<InputLetter>();
            lastLetter.DestroyWithAnimation();

            inputWord = inputWord.Remove(inputWord.Length - 1);
        }
    }

    private void ChangeConstraint()
    {
        currentConstraint = Constraint.GetRandomConstraint();
        constraintText.text = currentConstraint.GetDescription();
    }

    private int ComputeWordScore()
    {
        int total = 0;

        foreach (char c in inputWord)
        {
            Letter l = GetLetterFromChar(c);
            total += l.level;
        }

        return total;
    }

    private int GetLevelTargetScore()
    {
        return Mathf.FloorToInt(scoreExpMultiplier * Mathf.Exp(currentLevel * scoreExpSpeed));
    }
}


