using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public int maxInputLength = 17;
    
    public float intenseScoreMultiplier = 1.5f;
    public float scoreExpMultiplier = 20.0f;
    public float scoreExpSpeed = 0.15f;
    public int thousandLevel = 23;
    public int fastTime = 40;
    public int gameOverTime = 60;

    public RectTransform canvasTransform;

    public Transform inputParent;
    public GameObject inputLetterPrefab;
    public Transform cameraTransform;
    
    public TextMeshProUGUI versionText;

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
    public TextMeshProUGUI levelText;
    public DotCounter wordsCounter;
    public Transform bonusParent;
    public GameObject bonusPrefab;
    public GameObject bonusFullPanel;
    public DotCounter blessingCounter;
    public Button continueBtn;
    public Transform valueSelectParent;
    public GameObject timer;
    public Material timerMaterial;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI successScoreText;

    public TextMeshProUGUI[] progressTexts;
    public TextMeshProUGUI[] completionProgressTexts;
    public TextMeshProUGUI[] bonusesProgressTexts;
    public TextMeshProUGUI[] cursesProgressTexts;
    public TextMeshProUGUI[] benedictionsProgressTexts;

    public Transform bonusListParent;
    public Transform curseListParent;
    public Transform blessingListParent;

    public Transform cursor;
    public Image cursorImage;
    public float cursorBlinkSpeed = 1.2f;
    public float lastCursorChangeTime; // Make sure the cursor is visible for a short period of time after all inputs
    [System.NonSerialized] public int cursorPosition; // Index of letter AFTER the cursor

    private bool submissionAnimation = false;

    private bool isBonusPopupVisible = false;

    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeAmount;

    public float verySmallDelay = 0.1f;
    public float smallDelay = 0.7f;
    public float bigDelay = 1.2f;

    public ParticleSystem confettis;

    private bool shownThousandScreen = false;

    private int lastSecond;
    public bool isPlaying; // Is player is a run (not in main menu)?

    private void Awake()
    {
        i = this;
        inputWord = "";
    }

    private void Start() 
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        versionText.text = Application.version;

        Word.Init();
        HideError(true);

        // Load user progression
        SaveManager.LoadProgression();

        if (progression.alreadyPlayed) {
            SettingsManager.i.LoadSettingsFromFile();
        }
        else {
            SettingsManager.i.SetDefaultSettings();
        }
        SettingsManager.i.InstantiateSettingsUI();

        Keyboard.i.Init();

        continueBtn.gameObject.SetActive(progression.startedRun);

        // Create trash gameInfo so the keyboard doesn't throw errors 
        gi = new GameInfo {
            letters = new Letter[26],
            gameStats = new Stats(),
            bonuses = new BonusInfo[MAX_BONUS],
        };

        UpdateProgressUI();
    }

    public void StartNewRun(GameMode mode)
    {
        if (PanelsManager.i.isTransitioning) return;

        PanelsManager.i.CircleTransition(() => {
            PanelsManager.i.SelectPanel("Main", false);
            PanelsManager.i.ToggleGameUI(true);
            isPlaying = true;
            bonusFullPanel.SetActive(false);

            ClearInput(true);

            progression.alreadyPlayed = true;
            progression.startedRun = true;
            continueBtn.gameObject.SetActive(true);
            
            SaveManager.SaveProgression();

            gi.gameMode = mode;

            for (int i = 0; i < Word.wordArray.Length; i++) {
                Word.wordArray[i].timesUsed = 0;
            }

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

            // Reset stats
            gi.gameStats = new Stats();

            HideError(true);

            Keyboard.i.UpdateAllKeys(false);

            foreach (Transform t in bonusParent) 
            {
                Destroy(t.gameObject);
            }

            SetBlessingPoints(0, true);

            shownThousandScreen = false;

            gi.currentLevel = 0;
            StartNewLevel();

            Debug.Log(mode);

            if (mode == GameMode.Tutorial) {
                Tutorial.i.StartTutorial();
            }
        });
    }

    // Start run with same mode as previous one
    public void RestartRun()
    {
        GameManager.i.StartNewRun(gi.gameMode);
    }

    public void LoadRun()
    {
        if (PanelsManager.i.isTransitioning) return;

        HideError(true);
        UpdateCurrentScoreText(0);
        bonusFullPanel.SetActive(false);
        shownThousandScreen = false;

        PanelsManager.i.CircleTransition(() => {
            SaveManager.LoadRun();
            Keyboard.i.UpdateAllKeys(false);
            PanelsManager.i.ToggleGameUI(true);
            isPlaying = true;
        });
    }

    public void LevelCompleted()
    {
        // Show thousand screen
        if (!shownThousandScreen && gi.currentLevel == thousandLevel) {
            shownThousandScreen = true;

            successScoreText.text = gi.gameMode == GameMode.Intense || gi.gameMode == GameMode.Insane
                ? (1000 * intenseScoreMultiplier).ToString()
                : 1000.ToString();

            PanelsManager.i.SelectPanel("Success", false);
            ColorManager.i.SetTheme("success", false);
            confettis.Play();
            return;
        }

        // Receive blessing
        if (gi.blessingPoints >= blessingPointsLimit) {
            SetBlessingPoints(0);
            EventManager.i.Do(false, LevelCompleted);
            return;
        }
        
        gi.currentLevel++;
        UpdateLevelText(false);

        // Update best level in progress
        if (gi.currentLevel > progression.bestLevels.Get((int)gi.gameMode))
            progression.bestLevels.Set((int)gi.gameMode, gi.currentLevel);

        SaveManager.SaveProgression();

        if (gi.currentLevel % 2 == 1) {
            BonusManager.i.Do();
        }
        else if (gi.currentLevel % 4 == 0) {
            EventManager.i.Do(true, StartNewLevel);
        }
        else {
            ImprovementManager.i.Do();
        }
    }

    public void StartNewLevel()
    {
        gi.levelWords = 0;
        UpdateTotalScore(0, true);
        InitLevel();
        gi.levelTime = 0;
        SaveManager.SaveRun(GameInfo.State.Ingame);
    } 

    public void InitLevel()
    {
        PanelsManager.i.SelectPanel("Main", false);
        ColorManager.i.SetTheme("default", false);
        wordsCounter.SetValue(gi.levelWords + 1);
        ClearInput(true);
        UpdateLevelText(true);
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

    private void Update() 
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

        // Make cursor blink
        cursorImage.enabled = 
            Time.time < lastCursorChangeTime + cursorBlinkSpeed 
         || Time.time % cursorBlinkSpeed > cursorBlinkSpeed / 2;

        gi.levelTime += Time.deltaTime;

        bool showTimer = isPlaying && (gi.gameMode == GameMode.Fast || gi.gameMode == GameMode.Insane);
        timer.SetActive(showTimer);
        if (showTimer) {
            int remainingSeconds = Mathf.FloorToInt(fastTime - gi.levelTime);

            timerText.text = remainingSeconds.ToString();

            if (remainingSeconds > 0) {
                timerMaterial.SetFloat("_Angle", remainingSeconds * Mathf.PI * 2 / fastTime);
            }
            else {
                timerMaterial.SetFloat("_Angle", remainingSeconds * Mathf.PI * 2 / (gameOverTime - fastTime));
            }

            timerMaterial.SetColor("_ColorA", ColorManager.i.GetColor(ColorManager.ThemeColor.BackgroundLighter));
            timerMaterial.SetColor("_ColorB", ColorManager.i.GetColor(ColorManager.ThemeColor.PrimaryDarker));
            timerMaterial.SetColor("_ColorC", ColorManager.i.GetColor(ColorManager.ThemeColor.Secondary));

            if (remainingSeconds < lastSecond && remainingSeconds < 0) {
                int randLetter = Random.Range(0, 26);
                gi.letters[randLetter].Level--;
                Keyboard.i.UpdateAllKeys(true);
            }

            if (gi.levelTime > gameOverTime) {
                GameEndManager.i.Do();
            }

            lastSecond = remainingSeconds;
        }
    }


    public void UpdatePreviewInterface()
    {        
        int currentScore;
        
        if (CheckIfWordAllowed(inputWord))
        {
            currentScore = ComputeWordScore(inputWord, true, false, false);
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

        if (word.Length == 0)
        {
            HideError(false);
        }
        else if (word.Length < minWordLength) 
        {
            ShowError($"The word should have at least {minWordLength} letters");
        }
        else if (!Word.IsWordAllowed(word)) 
        {
            ShowError("This isn't a word");
        }
        else if ((gi.gameMode == GameMode.Demanding || gi.gameMode == GameMode.Insane) && Word.words[word].timesUsed > 0) 
        {
            ShowError("You already used this word");
        }
        else if (word.Length > maxLen) 
        {
            ShowError($"The word should have at most {maxLen} letters");
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
                int score = ComputeWordScore(inputWord, true, true, false);
                yield return StartCoroutine(TriggerBonuses());
                
                UpdateCurrentScoreText(0);

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

                // Increment word use count
                Word.words[inputWord.ToLower()].timesUsed++;

                ClearInput();
                
                int steps = System.Math.Min(Mathf.FloorToInt(smallDelay * 30), score);

                for (int i = 0; i < steps; i++) {
                    int s = Mathf.CeilToInt(i / (float)(steps - 1) * score + gi.totalScore);
                    totalScoreText.text = s.ToString();

                    yield return new WaitForSeconds(smallDelay / steps);
                } 

                gi.gameStats.wordCount++;

                Keyboard.i.UpdateAllKeys(true);

                submissionAnimation = false;

                gi.totalScore += score;

                if (gi.totalScore >= GetLevelTargetScore())
                {
                    Util.PingText(totalScoreText);
                    ParticlesManager.i.CircleParticles(totalScoreText.transform.position, 3.5f);
                    ShakeCamera();

                    yield return new WaitForSeconds(smallDelay);
                    
                    int remainingWords = wordsPerLevel - gi.levelWords - 1;

                    int newBlessingPoints = gi.blessingPoints + remainingWords;
                    if (newBlessingPoints > blessingPointsLimit) newBlessingPoints = blessingPointsLimit;
                    SetBlessingPoints(newBlessingPoints);

                    yield return new WaitForSeconds(bigDelay);

                    LevelCompleted();
                    yield break;
                }
                else if (gi.levelWords >= wordsPerLevel - 1)
                {
                    yield return new WaitForSeconds(bigDelay);    

                    GameEndManager.i.Do();
                    yield break;
                }

                Util.LeanTweenShake(totalScoreText.gameObject, 25, 0.4f);
                gi.levelWords++;

                InitLevel();

                SaveManager.SaveRun(GameInfo.State.Ingame);
            }
            else {
                Util.PingText(errorText);
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
            
            if (currentScore != 0) {
                currentScoreText.text = Util.ToStringWithPlus(currentScore);
            }
            else {
                currentScoreText.text = currentScore.ToString();
            }
        }

        lastCurrentScore = currentScore;
    }

    public void UpdateLevelText(bool silent = false) {
        levelText.text = $"{gi.currentLevel}/{thousandLevel}";

        if (!silent) {
            Util.PingText(levelText);
        }
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
        if (inputWord.Length >= maxInputLength) {
            return;
        }

        InputLetter newLetter = Instantiate(inputLetterPrefab, inputParent).GetComponent<InputLetter>();
        newLetter.letter = c;
        newLetter.TriggerCreationAnimation();
        newLetter.transform.SetSiblingIndex(cursorPosition);

        inputWord = inputWord.Insert(cursorPosition, c.ToString());

        UpdatePreviewInterface();

        cursorPosition++;

        lastCursorChangeTime = Time.time;
    }

    /// <summary>
    /// Clears the input, triggering an animation
    /// </summary>
    public void ClearInput(bool immediate = false) 
    {
        inputWord = "";
        cursorPosition = 0;

        if (immediate) {
            foreach (Transform child in inputParent) {
                if (child != cursor)
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
                if (i == cursorPosition) continue;

	            inputParent.GetChild(0).GetComponent<InputLetter>().DestroyWithAnimation(true);
                yield return new WaitForSeconds(verySmallDelay);
            }
        }
    }

    /// <summary>
    /// Sets cursor position according to the mouse position
    /// </summary>
    public void SetCursorPosition() {
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * canvasTransform.rect.width;

        int position = 0;
        int gotCursor = 0;
        while (position + gotCursor < inputParent.childCount) {
            Transform t = inputParent.GetChild(position + gotCursor);
            if (t != cursor) {
                if (t.localPosition.x > mouseX) break;

                position++;
            }
            else {
                gotCursor++;
            }
        }

        cursor.SetSiblingIndex(position);
        cursorPosition = position;
    }

    /// <summary>
    /// Removes a letter from the input word, triggering an animation
    /// </summary>
    public void EraseLastLetter() 
    {
        if (cursorPosition > 0) 
        {
            InputLetter lastLetter = inputParent.GetChild(cursorPosition - 1).GetComponent<InputLetter>();
            lastLetter.DestroyWithAnimation(false);

            inputWord = inputWord.Remove(cursorPosition - 1, 1);

            UpdatePreviewInterface();

            lastCursorChangeTime = Time.time;
            cursorPosition--;
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

    private int ComputeWordScore(string word, bool showBonusInterface, bool lettersActuallyScore, bool immediate)
    {
        int total = 0;

        // Create a copy of the letters, so that the modifications can be reverted
        Letter[] lettersCopy = new Letter[26];
        for (int i = 0; i < 26; i++) {
            lettersCopy[i] = gi.letters[i].Clone() as Letter;
        }

        // Get the points from the letters
        foreach (char c in word)
        {
            Letter l = GetLetterFromChar(c);
            total += l.GetScore();
        }

        // Trigger bonuses
        foreach (Bonus bonus in bonuses)
        {
            BonusAction a;
            if (immediate) {
                a = bonus.ScoreWithoutInterface(word);
            }
            else {
                a = bonus.ScoreWithInterface(word, showBonusInterface);
            }

            total += a.score;
        }

        // Put the old letters back!
        gi.letters = lettersCopy;

        if (lettersActuallyScore) {     
            // Trigger letters
            foreach (char c in word)
            {
                Letter l = GetLetterFromChar(c);
                l.OnPlayed();
            }
        }

        return total;
    }

    private IEnumerator<YieldInstruction> TriggerBonuses()
    {
        foreach (Bonus bonus in bonuses)
        {
            BonusAction a = bonus.ScoreWithInterface(inputWord, true);

            if (a.score != 0) {
                Util.PingText(bonus.scoreText);
            }

            if (a.improvedLetters) {

                Keyboard.i.UpdateAllKeys(true);
                ParticlesManager.i.CircleParticles(bonus.transform.position, 2.5f);
                yield return new WaitForSeconds(smallDelay);
            }

            bonus.ResetInterface();
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
        if (gi.currentLevel == thousandLevel) {
            if (gi.gameMode == GameMode.Intense || gi.gameMode == GameMode.Insane) {
                return 1500;   
            }
            else {
                return 1000;    
            }
        }

        float mult = gi.gameMode == GameMode.Intense || gi.gameMode == GameMode.Insane ? intenseScoreMultiplier : 1.0f;
        return Mathf.FloorToInt(mult * scoreExpMultiplier * Mathf.Exp(gi.currentLevel * scoreExpSpeed));
    }

    public void RemoveBonus(Bonus bonus)
    {
        for (int i = 0; i < bonuses.Count; i++) {
            bonuses[i].RecordPreviousPosition();
        }

        bonuses.Remove(bonus);
        Destroy(bonus.gameObject);
        LayoutRebuilder.ForceRebuildLayoutImmediate(bonusParent.gameObject.GetComponent<RectTransform>());

        Util.DoNextFrame(gameObject, () => {
            for (int i = 0; i < bonuses.Count; i++) {
                bonuses[i].AnimateFromLastPosition();
            }
        });

        UpdatePreviewInterface();
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

            // Record last bonus positions
            for (int i = 0; i < bonuses.Count; i++) {
                bonuses[i].RecordPreviousPosition();
            }

            bonus.transform.SetParent(bonusParent, false);
            Util.LeanTweenShake(bonus.gameObject, 40, 0.5f);

            bonus.popupActionText = "Remove";
            bonus.popupAction = () => {
                BonusPopup.i.HidePopup();
                RemoveBonus(bonus);
            };

            // Animate other bonuses
            LayoutRebuilder.ForceRebuildLayoutImmediate(bonusParent.gameObject.GetComponent<RectTransform>());
            for (int i = 0; i < bonuses.Count; i++) {
                bonuses[i].AnimateFromLastPosition();
            }

            progression.usedBonus.Set((int)bonus.info.type, true);

            SaveManager.SaveProgression();

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

    public void SetBlessingPoints(int newValue, bool immediate = false) 
    {
        gi.blessingPoints = newValue;
        blessingCounter.SetValue(gi.blessingPoints, immediate);
    }

    /// <summary>
    /// Return the word with the highest score.
    /// </summary>
    public IEnumerator<object> FindBestWord(System.Action<string, int> onFind)
    {
        int bestScore = -99999;
        string bestWord = "";

        int i = 0;
        foreach (KeyValuePair<string, GameWord> pair in Word.words)
        {
            if (!CheckIfWordAllowed(pair.Key)) 
                continue;

            int wordScore = ComputeWordScore(pair.Key, false, false, true);

            if (wordScore > bestScore)
            {
                bestScore = wordScore;
                bestWord = pair.Key;
            }

            if (i % 1000 == 0)
                yield return new WaitForEndOfFrame();

            i++;
        }

        onFind(bestWord, bestScore);
    }

    public void ReturnToMenu()
    {
        if (PanelsManager.i.isTransitioning || submissionAnimation) return;
        
        UpdateProgressUI();

        Tutorial.i.EndTutorial();
        ImprovementManager.i.Cancel();

        BonusPopup.i.HidePopup();

        ColorManager.i.SetTheme("menu", false);

        if (isPlaying)
            SaveManager.SaveRun(gi.state);

        PanelsManager.i.CircleTransition(() => {
            PanelsManager.i.SelectPanel("MainMenu", false);
            PanelsManager.i.ToggleGameUI(false);
            isPlaying = false;
        });
    }

    private void OnApplicationQuit() // Unity message, is not called by code
    {
        if (isPlaying)
            SaveManager.SaveRun(gi.state);
    } 

    public int GetMaxBonusCount()
    {
        if (gi.gameMode == GameMode.Intense || gi.gameMode == GameMode.Insane)
        {
            return 5;
        }
        else 
        {
            return 4;
        }
    }

    public void UpdateProgressUI() {
        foreach (TextMeshProUGUI t in progressTexts) {
            t.text = $"Progress ({Util.GetPercentage(progression.GetOverallProgression())}%)";
        }
        
        foreach (TextMeshProUGUI t in completionProgressTexts) {
            t.text = $"Game modes completed ({Util.GetPercentage(progression.GetCompletionProgression())}%)";
        }
        
        foreach (TextMeshProUGUI t in bonusesProgressTexts) {
            t.text = $"Bonuses discovered ({Util.GetPercentage(progression.GetBonusProgression())}%)";
        }
        
        foreach (TextMeshProUGUI t in cursesProgressTexts) {
            t.text = $"Curses discovered ({Util.GetPercentage(progression.GetCurseProgression())}%)";
        }
        
        foreach (TextMeshProUGUI t in benedictionsProgressTexts) {
            t.text = $"Blessings discovered ({Util.GetPercentage(progression.GetBenedictionProgression())}%)";
        }
    }

    public void CreateBonusProgressionUI() {
        foreach (Transform t in bonusListParent) {
            Destroy(t.gameObject);
        }

        for (int i = 0; i < (int)BonusType.MaxValue; i++) {
            Bonus b = Instantiate(bonusPrefab, bonusListParent).GetComponent<Bonus>();

            bool discovered = GameManager.i.progression.usedBonus.Get(i);
            if (discovered) {
                b.Init(new BonusInfo {
                    intArg = 99,
                    stringArg = "X",
                    type = (BonusType)i,
                });

                b.popupAction = () => {};
                b.popupActionText = "";
            }
            else {
                b.InitUnknown();
            }

        }
    }

    public void CreateEventProgressionUI(bool curse) {
        Transform parent = curse ? curseListParent : blessingListParent;
        int maxVal = curse ? Event.CURSE_COUNT : Event.BLESSING_COUNT;

        foreach (Transform t in parent) {
            Destroy(t.gameObject);
        }

        for (int i = 0; i < maxVal; i++) {
            Event ev = Instantiate(EventManager.i.eventPrefab, parent).GetComponent<Event>();

            bool discovered;
            if (curse) {
                discovered = GameManager.i.progression.usedCurses.Get(i);
            }
            else {
                discovered = GameManager.i.progression.usedBenedictions.Get(i);
            }

            if (discovered) {
                Event.EventInfo info;
                do {
                    info = curse ? Event.GetRandomCurse() : Event.GetRandomBlessing();
                } while (info.typeID != i); // HACK: This is one of the words hack I've ever written

                ev.Init(info, curse, () => {});
            }
            else {
                ev.InitUnknown();
            }

            ev.hideTakeButtonOnPopup = true;

        }
    }

    public void ResetEverything() {
        SaveManager.ResetAll();
        SettingsManager.i.DeleteFile();
        Application.Quit();
    }
}

/// <summary>
/// Holds all data for a run, is serialized for save file.
/// See also Progression and Settings for other saved values
/// </summary>
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

    // Counts separated by semicolons
    public string wordUseCounts;

    public Random.State randomState;
    
    [SerializeField]
    public Stats gameStats;

    public int levelWords;
    public int totalScore;
    
    [SerializeField]
    public Constraint constraint;
    
    public int blessingPoints;

    public float levelTime;

    public string currentPanelName;
}

[System.Serializable]
public enum GameMode {
    Tutorial, Standard, Demanding, Intense, Fast, Insane, MaxValue
}

