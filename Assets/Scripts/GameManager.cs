using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{   
    public const int MAX_BONUS = 5;

    public static GameManager i;

    [System.NonSerialized]
    public GameInfo gi;

    [System.NonSerialized]
    public Progression progression;

    public int minWordLength = 3;
    public int maxWordLength = 12;
    public int maxWordLengthWithBonus = 16;
    public int wordsPerLevel = 3;
    public int blessingPointsLimit = 6;
    public string rareLetters = "JQXZ";
    
    public float intenseScoreMultiplier = 1.5f;
    public float scoreExpMultiplier = 20.0f;
    public float scoreExpSpeed = 0.15f;
    public int thousandLevel = 23;

    public RectTransform canvasTransform;

    public Transform inputParent;
    public GameObject inputLetterPrefab;
    public Transform cameraTransform;

    private InputLetter[] inputLetters;
    private string inputWord;
    public Constraint currentConstraint;

    private int lastCurrentScore = -1;

    /// <summary>
    /// This is the list of Bonus GameObjects. It is reconstructed from gi.bonuses on load.
    /// </summary>
    public List<Bonus> bonuses;

    public TextMeshProUGUI errorText;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI totalScoreText;
    public TextMeshProUGUI targetScoreText;
    public TextMeshProUGUI constraintText;
    public DotCounter wordsCounter;
    public Transform bonusParent;
    public GameObject bonusPrefab;
    public GameObject bonusFullPanel;
    public DotCounter blessingCounter;

    private bool submissionAnimation = false;

    private bool isBonusPopupVisible = false;

    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeAmount;

    public float verySmallDelay = 0.1f;
    public float smallDelay = 0.7f;
    public float bigDelay = 1.2f;
    
    private void Awake()
    {
        i = this;
        inputWord = "";
    }

    private void Start() 
    {
        Word.Init();
        HideError(true);
        Keyboard.i.Init();

        // Load user progression
        SaveManager.LoadProgression();

        // Create trash gameInfo so the keyboard doesn't throw errors 
        gi = new GameInfo {
            letters = new Letter[26],
            gameStats = new Stats(),
            bonuses = new BonusInfo[MAX_BONUS],
        };
    }

    public void StartNewRun(GameMode mode)
    {
        PanelsManager.i.CircleTransition(() => {
            PanelsManager.i.SelectPanel("Main", false);
            PanelsManager.i.ToggleGameUI(true);
            bonusFullPanel.SetActive(false);

            ClearInput(true);

            progression.alreadyPlayed = true;
            progression.startedRun = true;
            SaveManager.SaveProgression();

            gi.gameMode = mode;

            bonuses = new List<Bonus>();

            // Reset letters
            for (int i = 0; i < 26; i++) {
                int score;
                if (rareLetters.Contains((char)('A' + i))) {
                    score = 2;
                }
                else {
                    score = 1;
                }

                gi.letters[i] = new Letter {
                    letter = (char)((int)'A' + i),
                    Level = score,
                };
            }

            HideError(true);

            Keyboard.i.UpdateAllKeys(false);

            foreach (Transform t in bonusParent) 
            {
                Destroy(t.gameObject);
            }

            SetBlessingPoints(0);

            gi.currentLevel = -1;
            StartNewLevel();

            if (mode == GameMode.Tutorial) {
                Tutorial.i.StartTutorial();
            }
        });
    }

    public void LoadRun()
    {
        HideError(true);
        UpdateCurrentScoreText(0);
        bonusFullPanel.SetActive(false);

        PanelsManager.i.CircleTransition(() => {
            SaveManager.LoadRun();
            Keyboard.i.UpdateAllKeys(false);
            PanelsManager.i.ToggleGameUI(true);
        });
    }

    public void LevelCompleted()
    {
        if (gi.blessingPoints >= blessingPointsLimit) {
            SetBlessingPoints(0);
            EventManager.i.Do(false, LevelCompleted);
            return;
        }

        if (gi.currentLevel % 2 == 0) {
            BonusManager.i.Do();
        }
        else if (gi.currentLevel % 4 == 3 || gi.gameMode == GameMode.Cursed) {
            EventManager.i.Do(true, StartNewLevel);
        }
        else {
            ImprovementManager.i.Do();
        }
    }

    public void StartNewLevel()
    {
        gi.currentLevel++;
        gi.levelWords = 0;
        UpdateTotalScore(0, true);
        InitLevel();
        SaveManager.SaveRun(GameInfo.State.Ingame);
    } 

    public void InitLevel()
    {
        PanelsManager.i.SelectPanel("Main", false);
        ColorManager.i.SetTheme("default", false);
        wordsCounter.SetValue(gi.levelWords + 1);
        ChangeConstraint();
        UpdateTotalScore(gi.totalScore, true);
        UpdateCurrentScoreText(0);
        UpdateTargetScoreText(false);
    } 

    public Letter GetLetterFromChar(char c)
    {
        char lower = char.ToLower(c);
        return gi.letters[(int)lower - (int)'a'];
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

        // TEST
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.O)) 
        {
            SaveManager.LoadRun();
        }

        // Update constraint text
        constraintText.text = currentConstraint.GetDescription();
    }


    public void UpdatePreviewInterface()
    {        
        int currentScore;
        
        if (CheckIfWordAllowed(inputWord))
        {
            currentScore = ComputeWordScore(inputWord, true, false);
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
    public bool CheckIfWordAllowed(string word)
    {        
        int maxLen = GetMaxWordLength();

        if (!Word.IsWordAllowed(word)) 
        {
            ShowError("This isn't a word");
        }
        else if (gi.gameMode == GameMode.Demanding && Word.words[word].timesUsed > 0) 
        {
            ShowError("You already used this word");
        }
        else if (word.Length > maxLen) 
        {
            ShowError($"The word should have at most {maxLen} letters");
        }
        else if (word.Length < minWordLength) 
        {
            ShowError($"The word should have at least {minWordLength} letters");
        }
        else if (!currentConstraint.IsWordAllowed(word))
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

    public int GetMaxWordLength()
    {
        foreach (Bonus b in bonuses)
        {
            if (b.info.type == BonusType.LongWord)
            {
                return maxWordLengthWithBonus;
            }
        }

        return maxWordLength;
    }

    public void SubmitWord() 
    {
        if (!submissionAnimation)
        {
            submissionAnimation = true;
            StartCoroutine(Coroutine());
        }

        IEnumerator<YieldInstruction> Coroutine() 
        {
            // Prevent the user from submitting a word during the tutorial
            if (Tutorial.i.TutorialPreventsAction) {
                submissionAnimation = false;
                yield break;
            }

            if (CheckIfWordAllowed(inputWord))
            {
                // Increment word une count
                Word.words[inputWord].timesUsed++;

                int score = ComputeWordScore(inputWord, false, true);
                yield return StartCoroutine(TriggerBonuses());
                
                UpdateTotalScore(gi.totalScore + score, false);
                UpdateCurrentScoreText(0);

                gi.gameStats.wordCount++;

                // Check if best score
                if (score >= gi.gameStats.bestScore) {
                    gi.gameStats.bestScore = score;
                    gi.gameStats.bestScoreWord = inputWord;
                }

                // Bump letter use count
                foreach (char c in inputWord) {
                    GetLetterFromChar(c).timesUsed++;
                }

                // Trigger OnNotPlayed
                for (char a = 'a'; a <= 'z'; a++) {
                    if (!inputWord.Contains(a)) {
                        GetLetterFromChar(a).OnNotPlayed();
                    }
                }

                ClearInput();
                Keyboard.i.UpdateAllKeys(true);

                submissionAnimation = false;

                if (gi.totalScore >= GetLevelTargetScore())
                {
                    Util.PingText(totalScoreText);
                    ParticlesManager.i.CircleParticles(totalScoreText.transform.position, 3.5f);
                    ShakeCamera();

                    yield return new WaitForSeconds(smallDelay);
                    
                    int remainingWords = wordsPerLevel - gi.levelWords - 1;
                    SetBlessingPoints(gi.blessingPoints + remainingWords);

                    yield return new WaitForSeconds(bigDelay);

                    LevelCompleted();
                    yield break;
                }
                else if (gi.levelWords >= wordsPerLevel - 1)
                {
                    yield return new WaitForSeconds(bigDelay);    
                    ColorManager.i.SetTheme("game_over", false); 
                    
                    progression.startedRun = false;
                    SaveManager.SaveProgression();   

                    GameEndManager.i.Do();
                    yield break;
                }

                Util.LeanTweenShake(totalScoreText.gameObject, 25, 0.4f);
                gi.levelWords++;

                InitLevel();

                SaveManager.SaveRun(GameInfo.State.Ingame);
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
        gi.totalScore = newValue;

        if (!immediate && gi.totalScore != newValue)
        {
            Util.LeanTweenShake(totalScoreText.gameObject, 25, 0.4f);
        }

        totalScoreText.text = gi.totalScore.ToString();
    }

    private void UpdateTargetScoreText(bool immediate)
    {
        targetScoreText.text = $"/{GetLevelTargetScore()}";
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
    public void ClearInput(bool immediate = false) 
    {
        inputWord = "";

        if (immediate) {
            foreach (Transform child in inputParent) {
                Destroy(child.gameObject);
            }
        }
        else {
            StartCoroutine(Coroutine());
        }

        IEnumerator<object> Coroutine() {
            int count = inputParent.childCount;

            for (int i = 0; i < count; i++)
            {
	            inputParent.GetChild(0).GetComponent<InputLetter>().DestroyWithAnimation(true);
                yield return new WaitForSeconds(verySmallDelay);
            }
        }
    }

    /// <summary>
    /// Removes a letter from the input word, triggering an animation
    /// </summary>
    public void EraseLastLetter() 
    {
        if (inputWord.Length > 0) 
        {
            InputLetter lastLetter = inputParent.GetChild(inputParent.childCount - 1).GetComponent<InputLetter>();
            lastLetter.DestroyWithAnimation(false);

            inputWord = inputWord.Remove(inputWord.Length - 1);

            UpdatePreviewInterface();
        }
    }

    private void ChangeConstraint()
    {
        Constraint newConstraint = Constraint.GetRandomConstraint();

        if (currentConstraint.IsEqualTo(newConstraint)) { // Prevent the game from showing twice the same constraint
            ChangeConstraint();
        }
        else {
            currentConstraint = newConstraint;
        }
    }

    private int ComputeWordScore(string word, bool showBonusInterface, bool lettersActuallyScore)
    {
        int total = 0;

        // Get the points from the letters
        foreach (char c in word)
        {
            Letter l = GetLetterFromChar(c);
            total += l.GetScore(lettersActuallyScore);
        }

        // Test bonuses

        // Create a copy of the letters, so that the modifications can be reverted
        Letter[] lettersCopy = new Letter[26];
        for (int i = 0; i < 26; i++) {
            lettersCopy[i] = gi.letters[i].Clone() as Letter;
        }

        foreach (Bonus bonus in bonuses)
        {
            BonusAction a = bonus.ScoreWithInterface(word, showBonusInterface);

            if (a.isAffected)
            {
                total += a.score;
            }
        }

        // Put the old letters back!
        gi.letters = lettersCopy;

        return total;
    }

    private IEnumerator<YieldInstruction> TriggerBonuses()
    {
        foreach (Bonus bonus in bonuses)
        {
            BonusAction a = bonus.ScoreWithInterface(inputWord, false);

            if (a.isAffected) {
                Keyboard.i.UpdateAllKeys(true);
                ParticlesManager.i.CircleParticles(bonus.transform.position, 2.5f);
                // ShakeCamera();
                yield return new WaitForSeconds(smallDelay);
            }
        }
    }

    private void ResetBonusesInterface()
    {
        foreach (Bonus bonus in bonuses)
        {
            bonus.ResetInterface();
        }
    }

    public int GetLevelTargetScore()
    {
        if (gi.currentLevel == thousandLevel) return 1000;

        float mult = gi.gameMode == GameMode.Intense ? intenseScoreMultiplier : 1.0f;
        return Mathf.FloorToInt(mult * scoreExpMultiplier * Mathf.Exp(gi.currentLevel * scoreExpSpeed));
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

        if (bonuses.Count >= GetMaxBonusCount()) 
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

    public bool CloneBonus(Bonus b)
    {
        Bonus clone = Instantiate(b.gameObject).GetComponent<Bonus>();
        clone.info = (BonusInfo)b.info.Clone();
        return AddBonus(clone);
    }
    
    public void CreateBonusUIFromGameInfo()
    {
        foreach (Transform child in bonusParent) 
        {
            Destroy(child.gameObject);
        }

        bonuses.Clear();

        for (int i = 0; i < GameManager.i.gi.bonuses.Length; i++)
        {
            Bonus b = Instantiate(bonusPrefab, bonusParent).GetComponent<Bonus>();
            b.Init(gi.bonuses[i]);
            AddBonus(b);
        }
    }

    public bool AreCharsEqual(char a, char b) {

        a = char.ToLower(a);
        b = char.ToLower(b);

        if (a < 'a' || a > 'z') return a == b;

        bool near = b == a - 1 || b == a || b == a + 1;

        if (GetLetterFromChar(a).effect == Letter.Effect.Polymorphic)
        {
            if (GetLetterFromChar(b).effect == Letter.Effect.Polymorphic) 
            {
                // Both polymorphic
                return b == a - 2 || near || b == a + 2;
            }
            
            return near;
        }
        else if (GetLetterFromChar(b).effect == Letter.Effect.Polymorphic)
        {
            return near;
        }

        return a == b;
    }

    public void ImproveLetter(char c) {
        GetLetterFromChar(c).Level += 1;
    }

    public void ShakeCamera()
    {
        Vector3 startPos = cameraTransform.position;
        LeanTween.value(cameraTransform.gameObject, shakeAmount, 0, shakeDuration).setOnUpdate(val => {
            cameraTransform.position = startPos + new Vector3(Random.Range(-val, val), Random.Range(-val, val), 0);
        }).setEaseOutQuad().setOnComplete(() => cameraTransform.position = startPos);
    }

    public Letter GetNthMostImprovedLetter(int n) 
    {
        Letter[] copy = new Letter[26]; 
        System.Array.Copy(gi.letters, 0, copy, 0, 26);
        System.Array.Sort(copy);

        return copy[26 - n];
    }

    public void SetBlessingPoints(int newValue) 
    {
        gi.blessingPoints = newValue;
        blessingCounter.SetValue(gi.blessingPoints);
    }

    /// <summary>
    /// Return the word with the highest score. Will wait until waitPredicate is false
    /// </summary>
    public IEnumerator<object> FindBestWord(System.Func<bool> waitPredicate, System.Action<string, int> onFind)
    {
        int bestScore = -99999;
        string bestWord = "";

        int i = 0;
        foreach (KeyValuePair<string, GameWord> pair in Word.words)
        {
            if (!CheckIfWordAllowed(pair.Key)) 
                continue;

            int wordScore = ComputeWordScore(pair.Key, false, false);

            if (wordScore > bestScore)
            {
                bestScore = wordScore;
                bestWord = pair.Key;
            }

            if (i % 5000 == 0)
                yield return new WaitForEndOfFrame();

            i++;
        }

        yield return new WaitWhile(waitPredicate);

        onFind(bestWord, bestScore);
    }

    public void ReturnToMenu()
    {
        ColorManager.i.SetTheme("menu", false);

        PanelsManager.i.CircleTransition(() => {
            PanelsManager.i.SelectPanel("MainMenu", false);
            PanelsManager.i.ToggleGameUI(false);
        });
    }

    public int GetMaxBonusCount()
    {
        if (gi.gameMode == GameMode.Intense)
        {
            return 5;
        }
        else 
        {
            return 4;
        }
    }
}

[System.Serializable]
public struct GameInfo {
    [System.Serializable]
    public enum State {
        Ingame, Improvement, Bonus, Curse, Blessing 
    }

    public GameMode gameMode;

    public State state;
    public int currentLevel;

    [SerializeField]
    public BonusInfo[] bonuses;
    
    // Not the actual letters! Just a copy for the save! See GameManager.letters
    [SerializeField]
    public Letter[] letters;

    public Random.State randomState;
    
    [SerializeField]
    public Stats gameStats;

    public int levelWords;
    public int totalScore;
    
    [SerializeField]
    public Constraint constraint;
    
    public int blessingPoints;

    public string currentPanelName;
}

[System.Serializable]
public enum GameMode {
    Tutorial, Standard, Demanding, Intense, Cursed, MaxValue
}

