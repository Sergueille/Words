using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ColorManager : MonoBehaviour
{
    public static ColorManager i;

    [System.NonSerialized]
    public List<ThemedObject> objects;

    public Theme[] themes;
    public string startTheme;
    public float transitionDuration;

    public Material[] colorMaterials;

    private Theme currentTheme;
    
    private float transitionStartTime = -9999.0f;
    private Theme oldTheme;

    private void Awake()
    {
        i = this;
        objects = new List<ThemedObject>();

        SetTheme(startTheme, true);
    }

    private void Update()
    {
        for (int i = 0; i < (int)ThemeColor.LastValue; i++)
        {
            if (colorMaterials[i] != null)
            {
                colorMaterials[i].SetColor("_FaceColor", GetColor((ThemeColor)i));
            }
        }
    }

    [System.Serializable]
    public enum ThemeColor {
        Primary, PrimaryDarker, PrimaryLighter,
        Secondary, SecondaryDarker,
        Background, BackgroundDarker, [FormerlySerializedAs("BackgoundLighter")] BackgroundLighter, [FormerlySerializedAs("BackgoundLighterLighter")] BackgroundLighterLighter,
        Foreground, ForegroundDarker,
        LastValue
    }    

    public Color GetColor(ThemeColor color) {
        Color current = currentTheme.colors[(int)color];

        if (oldTheme == null) {
            return current;
        }
        else {
            Color old = oldTheme.colors[(int)color];   
            return Color.Lerp(old, current, (Time.time - transitionStartTime) / transitionDuration);
        }
    }

    public void SetTheme(string name, bool immediate) {
        oldTheme = currentTheme;

        bool found = false;
        foreach (Theme t in themes) {
            if (t.name == name) {
                currentTheme = t;
                found = true;
            }
        }

        if (!found) {
            Debug.LogError($"Unknown theme name \"{name}\"!");
        }

        if (!immediate) {
            transitionStartTime = Time.time;
        }
    }

    public bool ColorChangedThisFrame()
    {
        return Time.time <= transitionStartTime + transitionDuration + 0.1f;
    }

    [System.Serializable]
    public class Theme {
        public string name;
        public Color[] colors;
    }
}
