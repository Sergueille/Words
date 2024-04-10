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
    
    private void Awake()
    {
       i = this;
    }
    
    public void Do(System.Action onFinished)
    {
        StartCoroutine(Coroutine());

        IEnumerator<WaitUntil> Coroutine() 
        {
            PanelsManager.i.SelectPanel("ImproveLetter", false);

            lettersToImprove = new List<char>();
            for (int i = 0; i < choiceCount; i++) 
            {
                char c = (char)('A' + (char)Random.Range(0, 26));
                Key key = Instantiate(letterPrefab, lettersParent).GetComponent<Key>();
                key.letter = c;
                key.onPress = c => {
                    key.Select(!key.isSelected);

                    if (key.isSelected) {
                        lettersToImprove.Add(c); 
                    }
                    else {
                        lettersToImprove.Remove(c); 
                    }
                };
                key.UpdateUI();
            }

            yield return new WaitUntil(() => lettersToImprove.Count >= improvementsCount);

            foreach (char c in lettersToImprove)
            {
                GameManager.i.GetLetterFromChar(c).level += 1;
            }

            Keyboard.i.UpdateAllKeys();

            foreach (Transform child in lettersParent)
            {
                Destroy(child.gameObject);
            }

            onFinished();
        }
    }
}
