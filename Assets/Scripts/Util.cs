
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        return $"<color=#555><size=130>{arg}</size></color>";
    }

    public static bool IsVowel(char c)
    {
        c = char.ToLower(c);
        return c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u' || c == 'y';
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
}
