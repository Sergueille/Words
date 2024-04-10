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
    public string rareLetters = "JQXZ";
    
    public float scoreExpMultiplier = 20.0f;
    public float scoreExpSpeed = 0.15f;

    public Transform inputParent;
    public GameObject inputLetterPrefab;

    public List<Bonus> bonuses;

    [System.NonSerialized]
    public Letter[] letters;

    private InputLetter[] inputLetters;
    private string inputWord;
    private int totalScore = -1;
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
    public Transform bonusParent;
    public GameObject bonusFullPanel;

    private bool submissionAnimation = false;

    private bool isBonusPopupVisible = false;

    public Stats gameStats;
    
    private void Awake()
    {
        i = this;
        letters = new Letter[26];

        for (int i = 0; i < 26; i++) {
            letters[i] = new Letter {
                letter = (char)((int)'A' + i),
                level = rareLetters.Contains((char)((int)'A' + i)) ? 2 : 1,
            };
        }

        inputWord = "";

        bonuses = new List<Bonus>();

        bonusFullPanel.SetActive(false);
    }

    private void Start() 
    {
        Word.Init();
        StartGame();
    }

    public void StartGame()
    {
        PanelsManager.i.SelectPanel("Main", false);

        gameStats = new Stats();

        // Reset letters
        for (int i = 0; i < 26; i++) {
            letters[i] = new Letter {
                letter = (char)((int)'A' + i),
                level = rareLetters.Contains((char)((int)'A' + i)) ? 2 : 1,
            };
        }

        Keyboard.i.UpdateAllKeys();

        HideError(true);
        bonuses.Clear();
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
        UpdateCurrentScoreText(0);
        UpdateTotalScore(0, true);
        UpdateTargetScoreText(false);
    } 

    public Letter GetLetterFromChar(char c)
    {
        char lower = char.ToLower(c);
        return letters[(int)lower - (int)'a'];
    }

    public void Update() 
    {
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


    public void UpdatePreviewInterface()
    {
        int currentScore = 0;
        
        if (CheckIfWordAllowed())
        {
            currentScore = ComputeWordScore(true);
        }
        else
        {
            currentScore = 0;
            ResetBonusesInterface();
        }

        UpdateCurrentScoreText(currentScore);
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
        if (!submissionAnimation)
        {
            submissionAnimation = true;
            StartCoroutine(Coroutine());
        }

        IEnumerator<WaitForSeconds> Coroutine() 
        {
            if (CheckIfWordAllowed())
            {
                int score = ComputeWordScore(false);
                
                UpdateTotalScore(totalScore + score, false);
                UpdateCurrentScoreText(0);

                ImproveLettersFromBonuses();

                gameStats.wordCount++;

                // Check if best score
                if (score >= gameStats.bestScore) {
                    gameStats.bestScore = score;
                    gameStats.bestScoreWord = inputWord;
                }

                // Bump letter use count
                foreach (char c in inputWord) {
                    GetLetterFromChar(c).timesUsed++;
                }

                ClearInput();

                if (totalScore >= GetLevelTargetScore())
                {
                    Util.PingText(totalScoreText);

                    yield return new WaitForSeconds(0.8f);

                    LevelCompleted();
                    submissionAnimation = false;
                    yield break;
                }
                else if (levelWords >= wordsPerLevel - 1)
                {
                    yield return new WaitForSeconds(0.8f);                
                    GameEndManager.i.Do();
                    yield break;
                }

                levelWords++;
                wordsCounter.SetValue(levelWords + 1);
                ChangeConstraint();
            }

            submissionAnimation = false;
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
            
            if (currentScore > 0) {
                currentScoreText.text = $"+{currentScore}";
            }
            else {
                currentScoreText.text = currentScore.ToString();
            }
        }

        lastCurrentScore = currentScore;
    }

    private void UpdateTotalScore(int newValue, bool immediate) 
    {
        if (totalScore == newValue) return;

        totalScore = newValue;

        if (!immediate)
        {
            Util.LeanTweenShake(totalScoreText.gameObject, 25, 0.4f);
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

        UpdatePreviewInterface();
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

            UpdatePreviewInterface();
        }
    }

    private void ChangeConstraint()
    {
        currentConstraint = Constraint.GetRandomConstraint();
        constraintText.text = currentConstraint.GetDescription();
    }

    private int ComputeWordScore(bool showBonusInterface)
    {
        int total = 0;

        foreach (char c in inputWord)
        {
            Letter l = GetLetterFromChar(c);
            total += l.level;
        }

        foreach (Bonus bonus in bonuses)
        {
            BonusAction a = bonus.UpdateScoreInterface(inputWord, showBonusInterface);

            if (a.isAffected)
            {
                total += a.score;
            }
        }

        return total;
    }

    private void ResetBonusesInterface()
    {
        foreach (Bonus bonus in bonuses)
        {
            bonus.UpdateScoreInterface(inputWord, false);
        }
    }

    private void ImproveLettersFromBonuses()
    {
        Debug.Log("A");

        foreach (Bonus bonus in bonuses)
        {
            BonusAction a = bonus.UpdateScoreInterface(inputWord, false);

            if (a.isAffected && a.lettersToImprove != null)
            {
                Debug.Log(a.lettersToImprove.Length);

                foreach (char c in a.lettersToImprove)
                {
                    GetLetterFromChar(c).level++;
                }
            }
        }

        Keyboard.i.UpdateAllKeys();
    }

    private int GetLevelTargetScore()
    {
        return Mathf.FloorToInt(scoreExpMultiplier * Mathf.Exp(currentLevel * scoreExpSpeed));
    }

    public void RemoveBonus(Bonus bonus)
    {
        bonuses.Remove(bonus);
        Destroy(bonus.gameObject);
    }

    public void ShowBonusPopup(bool visible)
    {
        if (visible)
        {
            if (isBonusPopupVisible) return;

            bonusFullPanel.transform.localScale = Vector3.one;
            bonusFullPanel.SetActive(true);
            Util.LeanTweenShake(bonusFullPanel, 15, 0.4f);

            isBonusPopupVisible = true;
        }
        else
        {
            if (!isBonusPopupVisible) return;

            LeanTween.scale(bonusFullPanel, Vector3.zero, 0.2f).setOnComplete(() => {
                bonusFullPanel.SetActive(false);
            });

            isBonusPopupVisible = false;
        }
    }

    /// <summary>
    /// Add an already instantiated bonus to list, making all UI changes. Returns true if successful
    /// </summary>
    public bool AddBonus(Bonus bonus)
    {
        BonusPopup.i.HidePopup();
        ShowBonusPopup(false);

        if (bonuses.Count >= maxBonus) 
        {
            ShowBonusPopup(true);
            return false;
        }
        else
        {
            bonuses.Add(bonus);
            bonus.transform.SetParent(GameManager.i.bonusParent, false);
            Util.LeanTweenShake(bonus.gameObject, 40, 0.5f);

            bonus.popupActionText = "Remove";
            bonus.popupAction = () => {
                BonusPopup.i.HidePopup();
                GameManager.i.RemoveBonus(bonus);
            };

            return true;
        }
    }
}


