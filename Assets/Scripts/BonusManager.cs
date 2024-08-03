using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    public static BonusManager i;

    public int choiceCount = 4;

    public Transform bonusParent;
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
            GameManager.i.UpdateLevelText(true);

            foreach (Transform child in bonusParent)
            {
                Destroy(child.gameObject);
            }

            PanelsManager.i.SelectPanel("Bonus", false);
            ColorManager.i.SetTheme("bonus", false);
            SaveManager.SaveRun(GameInfo.State.Bonus);

            finished = false;

            List<BonusInfo> created = new List<BonusInfo>();

            for (int i = 0; i < choiceCount; i++) 
            {
                Bonus bonus = Instantiate(bonusPrefab, bonusParent).GetComponent<Bonus>();

                bonus.popupAction = () => {
                    BonusPopup.i.HidePopup();
                    finished = GameManager.i.AddBonus(bonus);
                };
                bonus.popupActionText = "Select";

                bonus.Init(CreateNotRedundantBonus(created));
                created.Add(bonus.info);
            }

            yield return new WaitUntil(() => finished);
            GameManager.i.StartNewLevel();
        }
    }

    public void Skip()
    {
        if (!Tutorial.i.IsTutorialActive)
            finished = true;
    }

    private BonusInfo CreateNotRedundantBonus(List<BonusInfo> alreadyCreated)
    {
        while (true) {
            BonusInfo random = Bonus.GetRandomBonus();

            bool ok = true;
            foreach (Bonus b in GameManager.i.bonuses)
            {
                if (b.info.Equals(random))
                {
                    ok = false;
                    break;
                }
            }

            foreach (BonusInfo b in alreadyCreated)
            {
                if (b.Equals(random))
                {
                    ok = false;
                    break;
                }
            }

            if (ok)
            {
                return random;
            }
        }
    }
}
