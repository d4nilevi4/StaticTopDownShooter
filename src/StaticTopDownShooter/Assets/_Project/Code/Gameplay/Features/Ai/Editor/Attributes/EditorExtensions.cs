using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoOpArmy.WiseFeline
{
    public static class EditorExtensions
    {
        public static IList<T> Swap<T>(this IList<T> list, T itemA, T itemB)
        {
            if (!list.Contains(itemA) || !list.Contains(itemB))
            {
                throw new System.Exception("one or both items are not in the list");
            }

            int indexA = list.IndexOf(itemA);
            int indexB = list.IndexOf(itemB);
            list[indexA] = itemB;
            list[indexB] = itemA;
            return list;
        }
    }
}
