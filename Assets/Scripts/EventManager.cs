using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager i;

    public int choiceCount = 3;
    
    public GameObject eventPrefab;
    public Transform eventParent;
    public TextMeshProUGUI titleText;

    private void Awake()
    {
        i = this;
    }
    
    public void Do(bool curse, System.Action onFinished) 
    {
        StartCoroutine(Coroutine());

        IEnumerator<object> Coroutine() {
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

            for (int i = 0; i < choiceCount; i++) {
                Event ev = Instantiate(eventPrefab, eventParent).GetComponent<Event>();
                ev.Init(curse, () => {
                    finished = true;
                });
            }

            yield return new WaitUntil(() => finished);
            yield return new WaitForSeconds(GameManager.i.bigDelay);

            foreach (Transform child in eventParent)
            {
                Destroy(child.gameObject);
            }

            onFinished();
        }
    }

}
