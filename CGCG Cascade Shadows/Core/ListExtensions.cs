using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FR.Core;
public static class ListExtensions
{
    /// <param name="filter">Elements not passing the filter will be removed.</param>
    public static IEnumerable<T> FastFilterAndRemove<T>(this List<T> list, Predicate<T> filter)
    {
        List<int> deadIndices = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            if (filter(list[i]))
                yield return list[i];
            else
                deadIndices.Add(i);
        }

        for (int i = deadIndices.Count - 1; i >= 0; i--)
        {
            list[deadIndices[i]] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
        }
    }

    public static bool FastRemove<T>(this List<T> list, T value) where T : class
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == value)
            {
                list[i] = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                return true;
            }
        }
        return false;
    }

    public static void FastRemoveAt<T>(this List<T> list, int index)
    {
        list[index] = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
    }
}