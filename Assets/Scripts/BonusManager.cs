using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    public static BonusManager i;

    public int choiceCount = 4;

    [SerializeField] private Transform bonusParent;
    [SerializeField] private GameObject bonusPrefab;

    private bool finished;
    
    private void Awake()
    {
       i = this;
    }
    
    public void Do()
    {
        StartCoroutine(Coroutine());

        IEnumerator<WaitUntil> Coroutine() 
        {
            PanelsManager.i.SelectPanel("Bonus", false);
            SaveManager.SaveRun(GameInfo.State.Bonus);

            finished = false;

            for (int i = 0; i < choiceCount; i++) 
            {
                Bonus bonus = Instantiate(bonusPrefab, bonusParent).GetComponent<Bonus>();

                bonus.popupAction = () => {
                    BonusPopup.i.HidePopup();
                    finished = GameManager.i.AddBonus(bonus);
                };
                bonus.popupActionText = "Select";

                bonus.Init();
            }

            yield return new WaitUntil(() => finished);

            foreach (Transform child in bonusParent)
            {
                Destroy(child.gameObject);
            }

            GameManager.i.StartNewLevel();
        }
    }

    public void Skip()
    {
        finished = true;
    }
}
