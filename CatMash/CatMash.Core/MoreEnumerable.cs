using System;
using System.Collections.Generic;
using System.Linq;

namespace CatMash
{
    public static class MoreEnumerable
    {
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (var elt in sequence)
                action(elt);
        }

        public static T RandomElement<T>(this IList<T> list) => list[new Random().Next(list.Count - 1)];

        // Linq's implementation is not optimized for arrays. @see: https://github.com/microsoft/referencesource/blob/master/System.Core/System/Linq/Enumerable.cs#L1109
        public static T Last<T>(this T[] array) => array[^1];
    }
}