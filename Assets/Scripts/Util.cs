
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static T GetRandomElement<T>(T[] elements)
    {
        return elements[Random.Range(0, elements.Length)];
    }
}
