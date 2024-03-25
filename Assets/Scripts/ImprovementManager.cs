using System.Collections.Generic;
using UnityEngine;

public class ImprovementManager : MonoBehaviour
{
    public static ImprovementManager i;

    private int chosenLetters;
    
    [SerializeField]
    private Transform lettersParent;

    [SerializeField]
    private GameObject letterPrefab;
    
    private void Awake()
    {
       i = this;
    }
    
    public void Do(System.Action onFinished)
    {
        StartCoroutine(Coroutine());

        IEnumerator<WaitUntil> Coroutine() 
        {
            chosenLetters = 0;

            PanelsManager.i.SelectPanel("ImproveLetter", false);

            // TODO: populate UI

            yield return new WaitUntil(() => chosenLetters >= 2);

            onFinished();
        }
    }
}
