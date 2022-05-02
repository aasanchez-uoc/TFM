using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static T GetLooped<T>(this IReadOnlyList<T> list, int index)
    {
        while (index < 0)
        {
            index += list.Count;
        }
        if (index >= list.Count)
        {
            index %= list.Count;
        }
        return list[index];
    }

}
