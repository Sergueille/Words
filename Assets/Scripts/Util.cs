
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public static class Util
{
    public static T GetRandomElement<T>(T[] elements)
    {
        return elements[Random.Range(0, elements.Length)];
    }

    public static LTDescr LeanTweenImageColor(Image img, Color to, float duration)
    {
        return LeanTween.value(img.gameObject, (color) => img.color = color, img.color, to, duration);
    }

    public static LTDescr LeanTweenTextColor(TextMeshProUGUI txt, Color to, float duration)
    {
        return LeanTween.value(txt.gameObject, (color) => txt.color = color, txt.color, to, duration);
    }

    public static LTDescr LeanTweenShake(GameObject obj, float amount, float duration)
    {
        return LeanTween.value(obj, angle => obj.transform.eulerAngles = angle, new Vector3(0, 0, amount), Vector3.zero, duration).setEaseOutElastic();
    }

    public static string DecorateArgument<T>(T arg)
    {
        Color primaryColor= ColorManager.i.GetColor(ColorManager.ThemeColor.Primary);

        return $"<color=#{ColorUtility.ToHtmlStringRGBA(primaryColor)}><size=130>{arg}</size></color>";
    }

    public static bool IsVowel(char c)
    {
        c = char.ToLower(c);
        return GameManager.i.GetLetterFromChar(c).effect == Letter.Effect.Polymorphic || c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u' || c == 'y';
    }

    public static bool IsConsonant(char c)
    {
        c = char.ToLower(c);
        return GameManager.i.GetLetterFromChar(c).effect == Letter.Effect.Polymorphic || !IsVowel(c);
    }

    public static int CountVowels(string word)
    {
        int res = 0;

        foreach (char s in word)
        {
            if (IsVowel(s))
                res++;
        }

        return res;
    }

    public static int CountConsonant(string word)
    {
        int res = 0;

        foreach (char s in word)
        {
            if (IsConsonant(s))
                res++;
        }

        return res;
    }

    public static void PingText(TextMeshProUGUI text)
    {
        GameObject copy = GameObject.Instantiate(text.gameObject, text.transform.parent);
        Color endColor = text.color;
        endColor.a = 0;

        LeanTween.scale(copy, 3.0f * Vector3.one, 0.5f).setEaseOutQuad();
        LeanTweenTextColor(copy.GetComponent<TextMeshProUGUI>(), endColor, 0.5f).setOnComplete(
            () => {
                GameObject.Destroy(copy);
            }
        );

        LeanTweenShake(text.gameObject, 20, 0.3f);
    }

    public static T GetRandomWithSpawners<T>(Spawner<T>[] spawns)
    {
        float totalWeight = 0.0f;
        foreach (Spawner<T> spawn in spawns)
        {   
            totalWeight += spawn.weight;
        }

        float randomSpawn = Random.Range(0.0f, totalWeight);
        totalWeight = 0;
        foreach (Spawner<T> spawn in spawns)
        {
            totalWeight += spawn.weight;
            if (randomSpawn <= totalWeight) // Instantiate bonus
            {
                return spawn.data;
            }
        }

        throw new System.Exception("Unreachable");
    }

    public static void AppendIntIntoStringBuilder(StringBuilder b, int i) {
        int exp = 0;
        while ((16 << (exp * 4)) <= i) {
            exp++;
        }

        for (int j = exp; j >= 0; j--) {
            int digit = (i >> (j * 4)) & 15;
            b.Append((char)(digit + '0'));
        }
    } 

    public static void DoNextFrame(GameObject owner, System.Action fn) {
        LeanTween.value(owner, 0, 0, 0.01f).setOnComplete(fn);
    }

    public static string GetPercentage(float val) {
        return Mathf.FloorToInt(val * 100).ToString();
    }

    public struct Spawner<T> {
        public float weight;
        public T data;
    }
}

[System.Serializable]
public class MovementDescr
{
    public float duration;
    public LeanTweenType easeType;
    [System.NonSerialized] public LTDescr descr;
    [System.NonSerialized] public int tweenID;

    public virtual LTDescr Do(System.Action<float> callback)
    {
        callback(0);
        descr = LeanTween.value(0, 1, duration).setOnUpdate(callback).setEase(easeType);
        tweenID = descr.id;
        return descr;
    } 

    public virtual LTDescr DoReverse(System.Action<float> callback)
    {
        callback(1);
        descr = LeanTween.value(1, 0, duration).setOnUpdate(callback).setEase(easeType);
        tweenID = descr.id;
        return descr;
    }

    public LTDescr DoWithBounds(System.Action<float> callback, float start, float end)
    {
        callback(start);
        descr = LeanTween.value(0, 1, duration).setOnUpdate(t => callback(start * (1 - t) + (end * t))).setEase(easeType);
        tweenID = descr.id;
        return descr;
    } 

    public LTDescr DoWithBounds(System.Action<Vector3> callback, Vector3 start, Vector3 end)
    {
        callback(start);
        descr = LeanTween.value(0, 1, duration).setOnUpdate(t => callback(start * (1 - t) + (end * t))).setEase(easeType);
        tweenID = descr.id;
        return descr;
    } 

    public bool TryCancel()
    {
        if (descr == null)
        {
            return false;
        }

        LeanTween.cancel(tweenID);
        descr = null;
        tweenID = -1;

        return true;
    }
}

[System.Serializable]
public class MovementDescrWithAmplitude : MovementDescr
{
    public float amplitude;
    
    public override LTDescr Do(System.Action<float> callback)
    {
        callback(0);
        descr = LeanTween.value(0, amplitude, duration).setOnUpdate(callback).setEase(easeType);
        tweenID = descr.id;
        return descr;
    } 

    public override LTDescr DoReverse(System.Action<float> callback)
    {
        callback(amplitude);
        descr = LeanTween.value(amplitude, 0, duration).setOnUpdate(callback).setEase(easeType);
        tweenID = descr.id;
        return descr;
    }
}
