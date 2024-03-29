using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    public static BonusManager i;

    public int choiceCount = 4;

    [SerializeField] private Transform bonusParent;
    [SerializeField] private GameObject bonusPrefab;
    
    private void Awake()
    {
       i = this;
    }
    
    public void Do(System.Action onFinished)
    {
        StartCoroutine(Coroutine());

        IEnumerator<WaitUntil> Coroutine() 
        {
            PanelsManager.i.SelectPanel("Bonus", false);

            for (int i = 0; i < choiceCount; i++) 
            {
                Bonus bonus = Instantiate(bonusPrefab, bonusParent).GetComponent<Bonus>();
                bonus.Init();
            }

            bool finished = false;

            yield return new WaitUntil(() => finished);

            onFinished();
        }
    }
}
