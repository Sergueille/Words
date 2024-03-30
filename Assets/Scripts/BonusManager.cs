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
            bool finished = false;

            for (int i = 0; i < choiceCount; i++) 
            {
                Bonus bonus = Instantiate(bonusPrefab, bonusParent).GetComponent<Bonus>();

                bonus.popupAction = () => {
                    finished = true;
                    GameManager.i.bonuses.Add(bonus);
                    bonus.transform.SetParent(GameManager.i.bonusParent, false);
                    BonusPopup.i.HidePopup();
                    Util.LeanTweenShake(bonus.gameObject, 40, 0.5f);

                    bonus.popupActionText = "Remove";
                    bonus.popupAction = () => {
                        Debug.Log("TODO!"); // TODO
                    };
                };
                bonus.popupActionText = "Select";

                bonus.Init();
            }

            yield return new WaitUntil(() => finished);

            foreach (Transform child in bonusParent)
            {
                Destroy(child.gameObject);
            }

            onFinished();
        }
    }
}
