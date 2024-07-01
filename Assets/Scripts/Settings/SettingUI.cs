using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] GameObject valuePrefab;

    private string[] values;
    private System.Action<int> onSet;

    public void Init(string displayName, string[] values, int currentValue, System.Action<int> onSet) {
        nameText.text = displayName;
        valueText.text = values[currentValue];

        this.values = values;   
        this.onSet = onSet;
    }

    public void OnClick() {
        Transform parent = GameManager.i.valueSelectParent;

        int count = parent.childCount;
        for (int i = 0; i < count; i++) {
            Destroy(parent.GetChild(i).gameObject);
        }

        PanelsManager.i.SelectPanel("ValueSelect", false);

        for (int i = 0; i < values.Length; i++) {
            TextMeshProUGUI txt = Instantiate(valuePrefab, parent).GetComponent<TextMeshProUGUI>();

            txt.text = values[i];

            EventTrigger et = txt.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry t = new();
            int newValue = i;
            t.callback.AddListener(e => {
                onSet(newValue);
                PanelsManager.i.SelectPanel("Settings", false);
            });
            t.eventID = EventTriggerType.PointerClick;
            et.triggers.Add(t);
        }
    }
}
