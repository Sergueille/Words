using UnityEngine;
using TMPro;

using BonusSpawner = Util.Spawner<System.Func<BonusInfo>>;

public class Bonus : MonoBehaviour, System.ICloneable
{
    public BonusInfo info;

    [SerializeField] private TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
    [SerializeField] private Transform scoreTextUp;
    [SerializeField] private Transform scoreTextDown;

    [SerializeField] private Transform moveableTransform;

    [SerializeField] private MovementDescr xMove;
    [SerializeField] private MovementDescr yMove;

    public bool unknown;

    public System.Action popupAction;
    public string popupActionText;

    private bool isOscillating = false;

    private Vector2 previousPos;

    public void Init(BonusInfo info)
    {
        this.info = info;
        unknown = false;
        nameText.text = GetName();
        previousPos = transform.position;
        moveableTransform.localPosition = Vector2.zero;
    }

    public void InitUnknown()
    {
        unknown = true;
        nameText.text = "??";
    }

    private void Update()
    {
        if (isOscillating)
        {
            transform.eulerAngles = new Vector3(0, 0, Mathf.Sin(Time.time * 15.0f) * 4);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    public static BonusInfo GetRandomBonus() 
    {
        System.Func<BonusInfo> bonusGetter = Util.GetRandomWithSpawners(new BonusSpawner[] {
            new BonusSpawner {
                weight = 2.5f,
                data = () => {
                    string pair = Util.GetRandomElement(new string[] {
                        "AR", "AT", "AP", "AS", "AL", "AM", "AC", "AB", "AN", "EA", "ET", "ES", "ED", "EL", "EM", "EC", "EN", "RA", "RE", 
                        "RI", "RO", "RS", "TA", "TE", "TR", "TI", "TO", "TH", "UR", "US", "UL", "UN", "IA", "IE", "IT", "IO", "IS", "ID", 
                        "IL", "IC", "OR", "OT", "OU", "OP", "OS", "OG", "OL", "OM", "OC", "ON", "PA", "PE", "PR", "PI", "PO", "PH", "SE", 
                        "ST", "SU", "SI", "SS", "SH", "DE", "DI", "GE", "HA", "HE", "HI", "HO", "LA", "LE", "LY", "LI", "LO", "LL", "MA", 
                        "ME", "MI", "MO", "CA", "CE", "CT", "CI", "CO", "CH", "VE", "BL", "NA", "NE", "NT", "NI", "NO", "NS", "ND", "NG", 
                        "NC", 
                    });

                    return new BonusInfo {
                        type = BonusType.TwoLetters,
                        stringArg = pair,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.8f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Consonants,
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.BestCopy
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.WorstCopy,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.8f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.ShortWord,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.8f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.RareLetters,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.8f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Double,
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Initial,
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    int rand = Random.Range(7, 10);

                    return new BonusInfo {
                        type = BonusType.Length,
                        intArg = rand,
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Triple,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.9f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.FrequentVowel,
                    };
                }
            },
            new BonusSpawner {
                weight = 1,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'V', 'B', 'K' });

                    return new BonusInfo {
                        type = BonusType.Emergency,
                        stringArg = letter.ToString(),
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Equalizer,
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Palindrome,
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.OOO,
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'N', 'R', 'I', 'H' });

                    return new BonusInfo {
                        type = BonusType.Everywhere,
                        stringArg = letter.ToString(),
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'N', 'R', 'I', 'H' });

                    return new BonusInfo {
                        type = BonusType.Forbidden,
                        stringArg = letter.ToString(),
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Concentrate,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.7f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.RelativeNumbers,
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'P', 'G', 'Y' });

                    return new BonusInfo {
                        type = BonusType.Charged,
                        stringArg = letter.ToString(),
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.SuperW,
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Diversity,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.7f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Effects,
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.LongWord,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.75f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Sacrifice,
                    };
                }
            },
            new BonusSpawner {
                weight = 0.8f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.LastMinute,
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'M', 'W' });

                    return new BonusInfo {
                        type = BonusType.Absorbent,
                        stringArg = letter.ToString(),
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Gap,
                    };
                }
            },
            new BonusSpawner {
                weight = GameManager.i.gi.gameMode == GameMode.Demanding || GameManager.i.gi.gameMode == GameMode.Insane ? 0.0f : 0.9f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.Change,
                    };
                }
            },
            new BonusSpawner {
                weight = GameManager.i.gi.gameMode == GameMode.Demanding || GameManager.i.gi.gameMode == GameMode.Insane ? 0.0f : 0.9f,
                data = () => {
                    return new BonusInfo {
                        type = BonusType.NoChange,
                    };
                }
            },
        });

        return bonusGetter();
    }

    
    public static BonusInfo GetSuperBonus() 
    {
        System.Func<BonusInfo> bonusGetter = Util.GetRandomWithSpawners(new BonusSpawner[] {
            
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    string pair = Util.GetRandomElement(new string[] {
                        "ER", "IN"
                    });

                    return new BonusInfo {
                        type = BonusType.TwoLetters,
                        stringArg = pair,
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'S', 'T', 'E', 'A' });

                    return new BonusInfo {
                        type = BonusType.Charged,
                        stringArg = letter.ToString(),
                    };
                }
            },
            new BonusSpawner {
                weight = 0.8f,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'P', 'D', 'M' });

                    return new BonusInfo {
                        type = BonusType.Emergency,
                        stringArg = letter.ToString(),
                    };
                }
            },
            new BonusSpawner {
                weight = 1.0f,
                data = () => {
                    char letter = Util.GetRandomElement(new char[] { 'S', 'T', 'E', 'A' });

                    return new BonusInfo {
                        type = BonusType.Everywhere,
                        stringArg = letter.ToString(),
                    };
                }
            },
        });

        return bonusGetter();
    }

    public BonusAction OnScore(string word)
    {
        if (info.type == BonusType.TwoLetters)
        {
            bool affected = false;
            for (int i = 0; i < word.Length - 1; i++) {
                if (word[i] == info.stringArg[0] && word[i + 1] == info.stringArg[1]) {
                    GameManager.i.ImproveLetter(info.stringArg[0]);
                    GameManager.i.ImproveLetter(info.stringArg[1]);
                    affected = true;
                }
            }

            return new BonusAction {
                improvedLetters = affected,
                score = 0,
            };
        }
        else if (info.type == BonusType.Charged)
        {
            int improvementCount = 0;
            bool next = false;
            Letter baseLetter = GameManager.i.GetLetterFromChar(info.stringArg[0]);

            foreach (char c in word) {
                Letter l = GameManager.i.GetLetterFromChar(c);

                if (next) {
                    l.effect = Letter.Effect.Electric;
                    improvementCount++;
                    next = false;
                }

                if (l == baseLetter) {
                    next = true;
                }
            }

            return new BonusAction {
                improvedLetters = improvementCount > 0,
                score = 0,
            };
        }
        else if (info.type == BonusType.RelativeNumbers)
        {
            int score = 0;

            foreach (char c in word) {
                Letter l = GameManager.i.GetLetterFromChar(c);

                if (l.Level <= 0) {
                    score += -10 * (l.Level - 1);
                    l.Level++;
                }
            }

            return new BonusAction {
                improvedLetters = score != 0,
                score = score,
            };
        }
        else if (info.type == BonusType.Concentrate)
        {
            char highest = '\0';
            int highestLevel = -9999999;
            char lowest = '\0';
            int lowestLevel = 99999999;

            foreach (char c in word) {
                Letter l = GameManager.i.GetLetterFromChar(c);

                if (l.Level > highestLevel) {
                    highestLevel = l.Level;
                    highest = c;
                }

                if (l.Level < lowestLevel) {
                    lowestLevel = l.Level;
                    lowest = c;
                }
            }

            GameManager.i.ImproveLetter(highest);
            GameManager.i.ImproveLetter(highest);
            GameManager.i.GetLetterFromChar(lowest).Level -= 1;

            return new BonusAction {
                improvedLetters = true,
                score = 0,
            };
        }
        else if (info.type == BonusType.Forbidden)
        {
            bool found = false;
            foreach (char c in word) {
                found |= c == info.stringArg[0];
            }

            GameManager.i.ImproveLetter(info.stringArg[0]);
            GameManager.i.ImproveLetter(info.stringArg[0]);

            return new BonusAction {
                improvedLetters = true,
                score = found ? -500 : 00,
            };
        }
        else if (info.type == BonusType.Everywhere)
        {
            bool improved = false;
            
            foreach (char c in word) {
                if (GameManager.i.AreCharsEqual(c, info.stringArg[0])) {
                    improved = true;
                    GameManager.i.ImproveLetter(c);
                }
            }

            return new BonusAction {
                improvedLetters = improved,
                score = improved ? 0 : -500,
            };
        }
        else if (info.type == BonusType.OOO)
        {
            bool improved = false;
            bool lastWasO = false;
            foreach (char c in word) {
                if (lastWasO) {
                    improved = true;                                    
                    GameManager.i.ImproveLetter(c);
                }

                lastWasO = GameManager.i.AreCharsEqual(c, 'O');
            }

            return new BonusAction {
                improvedLetters = improved,
                score = 0,
            };
        }
        else if (info.type == BonusType.Palindrome)
        {
            if (GameManager.i.AreCharsEqual(word[0], word[word.Length - 1]))
            {
                GameManager.i.ImproveLetter(word[0]);

                return new BonusAction {
                    improvedLetters = true,
                    score = 15,
                };
            }
            else
            {
                return new BonusAction {
                    improvedLetters = false,
                    score = 0,
                };
            }
        }
        else if (info.type == BonusType.Equalizer)
        {
            int least = 100000000;
            for (int i = 0; i < word.Length; i++)
            {
                Letter letter = GameManager.i.GetLetterFromChar(word[i]);
                if (letter.Level < least)
                {
                    least = letter.Level;
                }
            }
            
            for (int i = 0; i < word.Length; i++)
            {
                Letter letter = GameManager.i.GetLetterFromChar(word[i]);
                if (letter.Level == least) {
                    letter.Level += 1;
                }
            }

            return new BonusAction {
                improvedLetters = true,
                score = 0,
            };
        }
        else if (info.type == BonusType.Emergency)
        {
            Letter l = GameManager.i.GetLetterFromChar(info.stringArg[0]);

            bool contains = false;

            foreach (char c in word)
            {   
                if (GameManager.i.AreCharsEqual(c, info.stringArg[0]))
                {
                    contains = true;
                }
            }

            if (contains)
            {
                if (l.Level >= 5)
                {
                    l.Level -= 5;
                    foreach (char cc in word)
                    {
                        if (!GameManager.i.AreCharsEqual(cc, info.stringArg[0]))
                        {
                            GameManager.i.GetLetterFromChar(cc).Level += 1;
                        }
                    }
                }
                else
                {
                    l.Level += 3;
                }
            }

            return new BonusAction
            {
                improvedLetters = contains,
                score = 0,
            };
        }
        else if (info.type == BonusType.FrequentVowel)
        {
            int[] counts = new int[26];

            foreach (char c in word)
            {
                counts[c - 'A']++;
            }

            int best = 0;
            int bestScore = 0;
            for (int i = 0; i < 26; i++)
            {
                if ((char)(i + 'A') != 'E' && (char)(i + 'A') != 'S' && best < counts[i])
                {
                    best = counts[i];
                    bestScore = GameManager.i.GetLetterFromChar((char)(i + 'A')).Level;
                }
            }

            return new BonusAction {
                improvedLetters = false,
                score = bestScore * best,
            };
        }
        else if (info.type == BonusType.Triple)
        {
            int[] counts = new int[26];
            bool improved = false;

            foreach (char c in word)
            {
                for (int i = 0; i < 26; i++)
                {
                    if (GameManager.i.AreCharsEqual((char)(i + 'A'), c)) {
                        counts[i]++;
                    }
                }
            }

            for (int i = 0; i < 26; i++)
            {
                if (counts[i] >= 3)
                {
                    improved = true;
                    GameManager.i.ImproveLetter((char)(i + 'A'));
                }
            }

            return new BonusAction {
                improvedLetters = improved,
                score = 0,
            };
        }
        else if (info.type == BonusType.Length)
        {
            if (word.Length == info.intArg)
            {
                int score = 0;
                foreach (char c in word)
                {
                    score += GameManager.i.GetLetterFromChar(c).GetScore();
                }

                score /= 2;

                return new BonusAction {
                    improvedLetters = false,
                    score = score,
                };
            }
            else
            {
                return new BonusAction {
                    improvedLetters = false,
                    score = 0,
                };
            }
        }
        else if (info.type == BonusType.Initial)
        {
            if (word.Length > 0) 
            {
                GameManager.i.ImproveLetter(word[0]);
                return new BonusAction {
                    improvedLetters = true,
                    score = 0,
                };
            }
            else
            {
                return new BonusAction {
                    improvedLetters = false,
                    score = 0,
                };
            }
        }
        else if (info.type == BonusType.Double)
        {
            bool improved = false;

            char last = '\0';
            foreach (char c in word)
            {
                if (GameManager.i.AreCharsEqual(last, c))
                {
                    improved = true;
                    GameManager.i.ImproveLetter(c);
                }

                last = c;
            }    
        
            return new BonusAction {
                improvedLetters = improved,
                score = 0,
            };
        }
        else if (info.type == BonusType.ShortWord)
        {
            if (word.Length == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    GameManager.i.ImproveLetter(word[i]);
                }

                return new BonusAction {
                    improvedLetters = true,
                    score = 0,
                };
            }
            else
            {
                return new BonusAction {
                    improvedLetters = false,
                    score = 0,
                };
            }
        }
        else if (info.type == BonusType.RareLetters)
        {
            bool improved = false;

            foreach (char c in word)
            {
                if (GameManager.i.AreCharsEqual(c, 'Q')
                    || GameManager.i.AreCharsEqual(c, 'J')
                    || GameManager.i.AreCharsEqual(c, 'Z')
                    || GameManager.i.AreCharsEqual(c, 'X'))
                {
                    improved = true;
                    GameManager.i.ImproveLetter(c);
                    GameManager.i.ImproveLetter(c);
                }
            }    
        
            return new BonusAction {
                improvedLetters = improved,
                score = improved ? 10 : 0,
            };
        }
        else if (info.type == BonusType.WorstCopy)
        {
            int worstLevel = 100000000;
            Letter worstLetter = null;

            foreach (char c in word)
            {
                Letter l = GameManager.i.GetLetterFromChar(c);

                if (l.Level < worstLevel) 
                {
                    worstLetter = l;
                    worstLevel = l.Level;
                }
            }

            return new BonusAction {
                improvedLetters = false,
                score = 6 * worstLetter.GetScore(),
            };
        }
        else if (info.type == BonusType.BestCopy)
        {
            int bestLevel = 0;

            foreach (char c in word)
            {
                Letter l = GameManager.i.GetLetterFromChar(c);

                if (l.Level > bestLevel) bestLevel = l.Level;
            }

            return new BonusAction {
                improvedLetters = false,
                score = bestLevel,
            };
        }
        else if (info.type == BonusType.Consonants)
        {
            int consonantCount = 0;
            foreach (char c in word)
            {
                if (!Util.IsVowel(c)) consonantCount++;
            }

            int randCons = Random.Range(0, consonantCount);
            int i = 0;
            foreach (char c in word)
            {
                if (!Util.IsVowel(c)) 
                {
                    if (i == randCons) {
                        GameManager.i.ImproveLetter(c);
                        return new BonusAction {
                            improvedLetters = true,
                            score = 0,
                        };
                    }

                    i++;
                }                            
            }

            return new BonusAction {
                improvedLetters = false,
                score = 0,
            };
        }
        else if (info.type == BonusType.SuperW)
        {
            bool okay = false;
            foreach (char c in word) {
                if (c == 'W') {
                    okay = true;
                    break;
                }
            }

            if (okay) {
                GameManager.i.GetLetterFromChar('w').Level += 3;
            }
            
            return new BonusAction {
                improvedLetters = okay,
                score = 0,
            };
        }
        else if (info.type == BonusType.Diversity)
        {
            bool[] used = new bool[26];

            foreach (char c in word)
            {
                if (used[c - 'A']) 
                {
                    return new BonusAction {
                        improvedLetters = false,
                        score = 0,
                    };
                }
                else
                {
                    used[c - 'A'] = true;
                }
            }

            foreach (char c in word)
            {
                if (Util.IsConsonant(c))
                {
                    GameManager.i.ImproveLetter(c);
                }
            }

            return new BonusAction {
                improvedLetters = true,
                score = 0,
            };
        }
        else if (info.type == BonusType.Effects)
        {
            bool affected = false;

            foreach (char c in word)
            {
                if (GameManager.i.GetLetterFromChar(c).effect != Letter.Effect.None)
                {
                    GameManager.i.ImproveLetter(c);
                    affected = true;
                }
            }

            return new BonusAction {
                improvedLetters = affected,
                score = 0,
            };
        }
        else if (info.type == BonusType.LongWord)
        {
            return new BonusAction {
                improvedLetters = word.Length > 12,
                score = 0,
            };
        }
        else if (info.type == BonusType.Sacrifice)
        {
            int score = 0;

            foreach (char c in word)
            {
                score += GameManager.i.GetLetterFromChar(c).GetScore();
            }

            GameManager.i.GetLetterFromChar(word[0]).Level -= (int)Mathf.Ceil(score / 10.0f);

            return new BonusAction {
                improvedLetters = true,
                score = score,
            };
        }
        else if (info.type == BonusType.LastMinute)
        {
            bool affected = GameManager.i.gi.levelWords == GameManager.i.wordsPerLevel - 1;

            int score = 0;

            if (affected) {
                foreach (char c in word)
                {
                    score += GameManager.i.GetLetterFromChar(c).GetScore();
                }
            }

            score /= 2;

            return new BonusAction {
                improvedLetters = false,
                score = score,
            };
        }
        else if (info.type == BonusType.Absorbent)
        {
            bool affected = false;
            for (int i = 0; i < word.Length; i++) {
                if (word[i] == info.stringArg[0]) {
                    if (i > 0) GameManager.i.GetLetterFromChar(word[i - 1]).Level -= 1;
                    if (i < word.Length - 1) GameManager.i.GetLetterFromChar(word[i + 1]).Level -= 1;

                    GameManager.i.GetLetterFromChar(info.stringArg[0]).Level += 3;
                    affected = true;
                }
            }

            return new BonusAction {
                improvedLetters = affected,
                score = 0,
            };
        }
        else if (info.type == BonusType.Gap)
        {
            int count = 0;
            for (int i = 0; i < word.Length - 2; i++) {
                if (word[i] == word[i + 2]) {
                    GameManager.i.GetLetterFromChar(word[i + 1]).Level += 2;
                    count++;
                }
            }

            return new BonusAction {
                improvedLetters = count > 0,
                score = 0,
            };
        }
        else if (info.type == BonusType.Change)
        {
            bool affected = Word.IsWordAllowed(word.ToLower()) && Word.GetWord(word.ToLower()).timesUsed == 0;

            int score = 0;

            if (affected) {
                foreach (char c in word)
                {
                    score += GameManager.i.GetLetterFromChar(c).GetScore();
                }
            }

            score /= 2;

            return new BonusAction {
                improvedLetters = false,
                score = score,
            };
        }
        else if (info.type == BonusType.NoChange)
        {
            int timesUsed = Word.IsWordAllowed(word.ToLower()) ? Word.GetWord(word.ToLower()).timesUsed : 0;

            return new BonusAction {
                improvedLetters = false,
                score = timesUsed * 10,
            };
        }
        else
        {
            throw new System.Exception("Missing branch!");
        }
    }

    public string GetName()
    {
        if (info.type == BonusType.TwoLetters)
        {
            return info.stringArg;
        }
        else if (info.type == BonusType.Charged)
        {
            return $"Charged {info.stringArg}";
        }
        else if (info.type == BonusType.RelativeNumbers)
        {
            return "Relative numbers";
        }
        else if (info.type == BonusType.Concentrate)
        {
            return "Concentrate";
        }
        else if (info.type == BonusType.Forbidden)
        {
            return $"Forbidden {info.stringArg}";
        }
        else if (info.type == BonusType.Everywhere)
        {
            return $"{info.stringArg} everywhere";
        }
        else if (info.type == BonusType.OOO)
        {
            return "Super O";
        }
        else if (info.type == BonusType.Palindrome)
        {
            return "Palindrome";
        }
        else if (info.type == BonusType.Equalizer)
        {
            return "Equalizer";
        }
        else if (info.type == BonusType.Emergency)
        {
            return $"Emergency {info.stringArg}";
        }
        else if (info.type == BonusType.FrequentVowel)
        {
            return "Frequent letter";
        }
        else if (info.type == BonusType.Triple)
        {
            return "Triple";
        }
        else if (info.type == BonusType.Length)
        {
            return $"{info.intArg} letters";
        }
        else if (info.type == BonusType.Initial)
        {
            return "The initial";
        }
        else if (info.type == BonusType.Double)
        {
            return "Double";
        }
        else if (info.type == BonusType.ShortWord)
        {
            return "Short word";
        }
        else if (info.type == BonusType.RareLetters)
        {
            return "Rare letters";
        }
        else if (info.type == BonusType.WorstCopy)
        {
            return "Worst copy";
        }
        else if (info.type == BonusType.BestCopy)
        {
            return "Best copy";
        }
        else if (info.type == BonusType.Consonants)
        {
            return "Consonants";
        }
        else if (info.type == BonusType.SuperW)
        {
            return "WWW";
        }
        else if (info.type == BonusType.Diversity)
        {
            return "Diversity";
        }
        else if (info.type == BonusType.Effects)
        {
            return "Effects";
        }
        else if (info.type == BonusType.LongWord)
        {
            return "Long words";
        }
        else if (info.type == BonusType.Sacrifice)
        {
            return "Sacrifice";
        }
        else if (info.type == BonusType.LastMinute)
        {
            return "Last minute";
        }
        else if (info.type == BonusType.Absorbent)
        {
            return $"Absorbent {info.stringArg}";
        }
        else if (info.type == BonusType.Gap)
        {
            return $"Gaps";
        }
        else if (info.type == BonusType.Change)
        {
            return $"Change";
        }
        else if (info.type == BonusType.NoChange)
        {
            return $"No change";
        }
        else
        {
            throw new System.Exception("Missing branch!");
        }
    }

    public string GetDescription()
    {
        if (info.type == BonusType.TwoLetters)
        {
            return $"Each time {Util.DecorateArgument(info.stringArg)} appears in a word, adds 1 point to each of these letters.";
        }
        else if (info.type == BonusType.Charged)
        {
            return $"Gives the Electric effect to letters located after every {Util.DecorateArgument(info.stringArg)}. The next time they are played, Electric letters will lose their effect but grant 10 additional points.";
        }
        else if (info.type == BonusType.RelativeNumbers)
        {
            return "Improves letters whose score is strictly less than 1, and also scores 10 points for each letter point below 1.";
        }
        else if (info.type == BonusType.Concentrate)
        {
            return "Subtracts 1 point from the least improved letter in the word and adds 2 points to the most improved letter in the word (starting with the leftmost letters first).";
        }
        else if (info.type == BonusType.Forbidden)
        {
            return $"Adds 2 points to {Util.DecorateArgument(info.stringArg)}. If the word contains this letter, scores -500 points.";
        }
        else if (info.type == BonusType.Everywhere)
        {
            return $"Adds 1 point to every {Util.DecorateArgument(info.stringArg)} in the word. If the word doesn't contain any, scores -500 points.";
        }
        else if (info.type == BonusType.OOO)
        {
            return $"Adds 1 point to any letter directly following an {Util.DecorateArgument("O")}.";
        }
        else if (info.type == BonusType.Palindrome)
        {
            return "If the first and last letters of the word are identical, adds 1 point to this letter and scores 15 points.";
        }
        else if (info.type == BonusType.Equalizer)
        {
            return "Adds 1 point to the least improved letters in the word (applies to all if multiple letters have the same score).";
        }
        else if (info.type == BonusType.Emergency)
        {
            return $"If the word contains {Util.DecorateArgument(info.stringArg)}, and its score is at least 5, improve every other letter in the word and {Util.DecorateArgument(info.stringArg)} looses 5 points. If its score is less than 5, add 3 points to {Util.DecorateArgument(info.stringArg)}";
        }
        else if (info.type == BonusType.FrequentVowel)
        {
            return $"Finds the most frequent letter in the word and scores its points for each occurrence. Doesn't work for {Util.DecorateArgument("E")} and {Util.DecorateArgument("S")}.";
        }
        else if (info.type == BonusType.Triple)
        {
            return $"If the word contains three identical letters, adds 1 point to the letter.";
        }
        else if (info.type == BonusType.Length)
        {
            return $"If the word is exactly {info.intArg} letters long, scores half the word's points (excluding other bonuses).";
        }
        else if (info.type == BonusType.Initial)
        {
            return "Adds 1 point to the first letter of the word.";
        }
        else if (info.type == BonusType.Double)
        {
            return "If the word contains two identical adjacent letters, adds 1 point to the letter.";
        }
        else if (info.type == BonusType.ShortWord)
        {
            return "If the word is 4 letters long, adds 1 point to each of the 4 letters.";
        }
        else if (info.type == BonusType.RareLetters)
        {
            return $"Adds 2 points to each {Util.DecorateArgument('Q')}, {Util.DecorateArgument('J')}, {Util.DecorateArgument('X')} and {Util.DecorateArgument('Z')} in the word, and scores 10 points if any are present.";
        }
        else if (info.type == BonusType.WorstCopy)
        {
            return "Finds the least improved letter in the word, then scores 6 times the points it would score.";
        }
        else if (info.type == BonusType.BestCopy)
        {
            return "Finds the most improved letter in the word, then scores its points.";
        }
        else if (info.type == BonusType.Consonants)
        {
            return $"Adds 1 point to a random consonant in the word.";
        }
        else if (info.type == BonusType.SuperW)
        {
            return $"If the word contains {Util.DecorateArgument('W')}, adds 3 points to that letter.";
        }
        else if (info.type == BonusType.Diversity)
        {
            return "If all the letters of the word are different, adds 1 point to each consonant.";
        }
        else if (info.type == BonusType.Effects)
        {
            return "Adds 1 point to all letters with an effect in the word.";
        }
        else if (info.type == BonusType.LongWord)
        {
            return "Allows words up to 16 letters long.";
        }
        else if (info.type == BonusType.Sacrifice)
        {
            return "Score the points the word would give alone, but the first letter of the word looses a tenth of the points given (rounded up).";
        }
        else if (info.type == BonusType.LastMinute)
        {
            return "When submitting the last word of a level, scores half the points the word would normally give.";
        }
        else if (info.type == BonusType.Absorbent)
        {
            return $"Adds 3 points to each {Util.DecorateArgument(info.stringArg)}, but subtracts 1 point from the letters directly adjacent to it";
        }
        else if (info.type == BonusType.Gap)
        {
            return $"If two identical letters are separated by a single letter, adds 1 point to the letter between them.";
        }
        else if (info.type == BonusType.Change)
        {
            return $"If the word was never used before, scores half the points the word would normally give.";
        }
        else if (info.type == BonusType.NoChange)
        {
            return $"Scores 10 points for each time the word was previously used.";
        }
        else
        {
            throw new System.Exception("Missing branch!");
        }
    }

    public void OnClick()
    {
        if (unknown) {
            BonusPopup.i.ShowPopup("???", "You haven't discovered this. Take this bonus at least once to discover it.", popupAction, popupActionText);
        }
        else {
            BonusPopup.i.ShowPopup(GetName(), GetDescription(), popupAction, popupActionText);
        }
    }

    public BonusAction ScoreWithoutInterface(string word) 
    {
        BonusAction res;
        if (word.Length == 0) // Do not evaluate function on empty word (it may cause errors)
        {
            res = new BonusAction {
                improvedLetters = false, score = 0,
            };
        }
        else
        {
            res = OnScore(word.ToUpper());
        }

        return res;
    }

    public BonusAction ScoreWithInterface(string word, bool withInterface)
    {
        BonusAction res = ScoreWithoutInterface(word);

        isOscillating = (res.improvedLetters || res.score != 0) && withInterface;

        if (withInterface && res.score != 0)
        {
            scoreText.text = Util.ToStringWithPlus(res.score);

            LeanTween.move(scoreText.gameObject, scoreTextUp.position, 0.4f).setEaseOutElastic();
        }
        else
        {   
            if (scoreText.transform.position != scoreTextDown.position)
            {
                LeanTween.move(scoreText.gameObject, scoreTextDown.position, 0.4f).setEaseInQuad();
            }
        }

        return res;
    }

    public void ResetInterface() 
    {
        isOscillating = false;
        if (scoreText.transform.position != scoreTextDown.position)
        {
            LeanTween.move(scoreText.gameObject, scoreTextDown.position, 0.4f).setEaseInQuad();
        }
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    public void RecordPreviousPosition()
    {
        previousPos = transform.position;    
    }

    public void AnimateFromLastPosition() {
        moveableTransform.position = previousPos;

        xMove.DoWithBounds(x => {
            moveableTransform.localPosition = new Vector3(
                x, moveableTransform.localPosition.y, 0
            );
        }, moveableTransform.localPosition.x, 0);

        yMove.DoWithBounds(y => {
            moveableTransform.localPosition = new Vector3(
                moveableTransform.localPosition.x, y, 0
            );
        }, moveableTransform.localPosition.y, 0);
    }
}

[System.Serializable]
public struct BonusInfo : System.ICloneable
{
    public BonusType type;
    public string stringArg;
    public int intArg;

    public override bool Equals(object obj)
    {
        if (obj is BonusInfo other)
        {
            return other.type == this.type
                && other.stringArg == this.stringArg
                && other.intArg == this.intArg;
        }
        else return false;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}

[System.Serializable]
public enum BonusType {
    TwoLetters,
    Consonants,
    BestCopy,
    WorstCopy,
    RareLetters,
    ShortWord,
    Double,
    Initial,
    Length,
    Triple,
    FrequentVowel,
    Emergency,
    Equalizer,
    Palindrome,
    OOO,
    Everywhere,
    Forbidden,
    Concentrate,
    RelativeNumbers,
    Charged,
    SuperW,
    Diversity,
    Effects,
    LongWord,
    Sacrifice,
    LastMinute,
    Absorbent,
    Gap,
    Change,
    NoChange,
    MaxValue,
}


public struct BonusAction
{
    public bool improvedLetters;
    public int score;
}
