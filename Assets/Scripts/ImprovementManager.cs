using System.Collections.Generic;
using UnityEngine;

public class ImprovementManager : MonoBehaviour
{
    public static ImprovementManager i;

    public int choiceCount = 6;
    public int improvementsCount = 2;

    [SerializeField] private Transform lettersParent;
    [SerializeField] private GameObject letterPrefab;

    private List<char> lettersToImprove;

    private Coroutine doCoroutine;
    
    private void Awake()
    {
       i = this;
    }
    
    public void Do()
    {
        doCoroutine = StartCoroutine(Coroutine());

        IEnumerator<object> Coroutine() 
        {
            GameManager.i.UpdateLevelText(true);

            foreach (Transform child in lettersParent)
            {
                Destroy(child.gameObject);
            }
            
            PanelsManager.i.SelectPanel("ImproveLetter", false);
            ColorManager.i.SetTheme("improvement", false);
            SaveManager.SaveRun(GameInfo.State.Improvement);

            bool finished = false;
            lettersToImprove = new List<char>();
            for (int i = 0; i < choiceCount; i++) 
            {
                char c = (char)('A' + (char)Random.Range(0, 26));
                Key key = Instantiate(letterPrefab, lettersParent).GetComponent<Key>();
                key.letter = c;
                key.onPress = c => {
                    if (finished) return;

                    key.Select(!key.isSelected);

                    if (key.isSelected) {
                        lettersToImprove.Add(c); 
                    }
                    else {
                        lettersToImprove.Remove(c); 
                    }
                };
                key.UpdateUI(false);
            }

            yield return new WaitUntil(() => lettersToImprove.Count >= improvementsCount);
            finished = true;

            yield return new WaitForSeconds(GameManager.i.smallDelay);

            foreach (char c in lettersToImprove)
            {
                GameManager.i.GetLetterFromChar(c).Level += 1;
            }

            Keyboard.i.UpdateAllKeys(true);

            GameManager.i.StartNewLevel();
        }
    }

    public void Cancel() {
        if (doCoroutine != null) {
            StopCoroutine(doCoroutine);
        }
    }
}
