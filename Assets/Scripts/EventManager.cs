using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager i;

    public int choiceCount = 3;
    
    public GameObject eventPrefab;
    public Transform eventParent;
    public TextMeshProUGUI titleText;
    
    private int[] previousTypeIds;

    private void Awake()
    {
        i = this;
    }
    
    public void Do(bool curse, System.Action onFinished) 
    {
        StartCoroutine(Coroutine());

        IEnumerator<object> Coroutine() {
            GameManager.i.UpdateLevelText(true);

            foreach (Transform child in eventParent)
            {
                Destroy(child.gameObject);
            }
            
            PanelsManager.i.SelectPanel("Event", false);

            if (curse) {
                titleText.text = "Pick a curse:";
                ColorManager.i.SetTheme("curse", false);
                SaveManager.SaveRun(GameInfo.State.Curse);
            }
            else {
                titleText.text = "Pick a blessing:";
                ColorManager.i.SetTheme("blessing", false);
                SaveManager.SaveRun(GameInfo.State.Blessing);
            }

            bool finished = false;

            previousTypeIds = new int[choiceCount]; // Array to prevent getting twice the same event
            for (int i = 0; i < choiceCount; i++) {
                previousTypeIds[i] = -1;
            }

            for (int i = 0; i < choiceCount; i++) {
                Event.EventInfo info = curse ? Event.GetRandomCurse() : Event.GetRandomBlessing();

                if (previousTypeIds.Contains(info.typeID)) { // If already selected one, retry
                    i--;
                    continue;
                }

                Event ev = Instantiate(eventPrefab, eventParent).GetComponent<Event>();

                previousTypeIds[i] = info.typeID;

                ev.Init(info, curse, () => {
                    finished = true;
                });
            }

            yield return new WaitUntil(() => finished);
            yield return new WaitForSeconds(GameManager.i.bigDelay);

            onFinished();
        }
    }

}
