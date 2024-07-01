using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public const string SETTINGS_FILE_NAME = "settings"; 

    public static SettingsManager i;

    public GameObject uiPrefab;
    public Transform settingsUIParent;

    public Settings settings;
    
    
    private void Awake()
    {
        i = this;
    }

    public void SetDefaultSettings() {
        settings = new Settings {
            keyboardLayout = Settings.KeyboardLayout.Qwerty,
        };

        ApplySettings();
    }

    public void LoadSettingsFromFile() {
        string txt = System.IO.File.ReadAllText(SaveManager.GetPath(SETTINGS_FILE_NAME));
        settings = JsonUtility.FromJson<Settings>(txt);
        ApplySettings();
    }

    public void InstantiateSettingsUI() {
        int count = settingsUIParent.childCount;
        for (int i = 0; i < count; i++) {
            Destroy(settingsUIParent.GetChild(i).gameObject);
        }

        FieldInfo[] fields = typeof(Settings).GetFields();

        for (int fieldID = 0; fieldID < fields.Length; fieldID++) {
            Type t = fields[fieldID].FieldType;

            if (!t.IsEnum) {
                Debug.Log($"Type for {fields[fieldID].Name} should be an enum");
                Debug.Log(t.Name);
                continue;
            }

            int valueCount = t.GetEnumValues().Length;
            MemberInfo[] infos = t.GetMembers();
            string[] values = new string[valueCount];

            for (int i = 0, j = 0; i < infos.Length; i++) {
                values[j] = GetDisplayNameOfThing(infos[i]);
            
                if (ObjectHasDisplayName(infos[i])) {
                    values[j] = GetDisplayNameOfThing(infos[i]);
                    j++;
                }
            }

            SettingUI ui = Instantiate(uiPrefab, settingsUIParent).GetComponent<SettingUI>();

            int value = (int)fields[fieldID].GetValue(settings);

            int id = fieldID;

            ui.Init(GetDisplayNameOfThing(fields[fieldID]), values, value, val => {   
                object settingsObject = settings;

                fields[id].SetValue(settingsObject, Enum.ToObject(t, val));

                settings = (Settings)settingsObject;
                ApplySettings();
            });
        }
    }
    
    public string GetDisplayNameOfThing(ICustomAttributeProvider obj) {
        string name = "No display name!";
        foreach (object att in obj.GetCustomAttributes(false)) {
            if (att is DisplayName) {
                name = (att as DisplayName).name;
            }
        }

        return name;
    }
    
    public bool ObjectHasDisplayName(ICustomAttributeProvider obj) {
        foreach (object att in obj.GetCustomAttributes(false)) {
            if (att is DisplayName) {
                return true;
            }
        }

        return false;
    }

    public void ApplySettings() {
        string txt = JsonUtility.ToJson(settings, true);
        System.IO.File.WriteAllText(SaveManager.GetPath(SETTINGS_FILE_NAME), txt);
        
        Keyboard.i.Init();
        InstantiateSettingsUI();
    }
    
    [System.Serializable]
    public struct Settings {
        public enum KeyboardLayout {
            [DisplayName("QWERTY")] Qwerty, 
            [DisplayName("QWERTZ")] Qwertz, 
            [DisplayName("AZERTY")] Azerty, 
            [DisplayName("Colemak")] Colemak, 
            [DisplayName("Dvorak")] Dvorak
        }

        [DisplayName("Keyboard layout")]
        public KeyboardLayout keyboardLayout;
    }
}



[AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
public class DisplayName : Attribute
{
    public string name;

    public DisplayName(string name) {
        this.name = name;
    } 
}
